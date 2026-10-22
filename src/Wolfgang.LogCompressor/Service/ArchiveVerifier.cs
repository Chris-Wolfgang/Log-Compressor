using System.Buffers;
using System.Buffers.Binary;
using System.Formats.Tar;
using System.IO.Compression;
using System.IO.Hashing;
using Microsoft.Extensions.Logging;
using Wolfgang.LogCompressor.Abstraction;

namespace Wolfgang.LogCompressor.Service;

/// <summary>
/// Verifies compressed archive integrity by reading the archive contents and
/// checking format-level completeness.
/// </summary>
/// <remarks>
/// Reading alone is NOT sufficient for gzip and brotli: modern .NET's
/// <see cref="GZipStream"/> and <see cref="BrotliStream"/> silently return
/// partial data on a truncated stream instead of throwing (a documented
/// behavioural change), so a torn write would "verify" and the original would
/// be deleted. Found by the property fuzz suite (#68). The gzip paths
/// therefore validate the stream's own trailer (CRC-32 + length) explicitly,
/// and brotli — a format with no checksum — is checked against the expected
/// uncompressed size when the caller knows it.
/// </remarks>
internal sealed class ArchiveVerifier : IArchiveVerifier
{
    private readonly ILogger<ArchiveVerifier> _logger;



    /// <summary>
    /// Initializes a new instance of the <see cref="ArchiveVerifier"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public ArchiveVerifier(ILogger<ArchiveVerifier> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        _logger = logger;
    }



    /// <inheritdoc />
    public async Task<bool> VerifyAsync(string archivePath, string format, long? expectedUncompressedSize = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(archivePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(format);

        if (expectedUncompressedSize is < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(expectedUncompressedSize));
        }

        try
        {
            await VerifyByFormatAsync(archivePath, format, expectedUncompressedSize, cancellationToken).ConfigureAwait(false);
            return true;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // A cancellation is not a verdict on the archive: let it end the run.
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Archive verification failed for {Path}: {Message}", archivePath, ex.Message);
            return false;
        }
    }



    private static async Task VerifyByFormatAsync(string archivePath, string format, long? expectedUncompressedSize, CancellationToken cancellationToken)
    {
        switch (format.ToLowerInvariant())
        {
            case "zip":
                // ZipArchive validates each entry's recorded CRC-32 as the
                // entry stream is drained, and a truncated central
                // directory fails construction — no extra checks needed.
                await VerifyZipAsync(archivePath, cancellationToken).ConfigureAwait(false);
                break;
            case "tar.gz":
                // One read: the tar walk proves structure while the same pass
                // computes the CRC-32/length checked against the gzip trailer.
                await VerifyGZipAsync(archivePath, expectedSize: null, WalkTarAsync, cancellationToken).ConfigureAwait(false);
                break;
            case "tar.br":
                // Brotli carries no checksum; the 512-byte tar structure is
                // the completeness signal (a truncation mid-entry breaks
                // the walk).
                await VerifyTarStreamAsync(archivePath, static s => new BrotliStream(s, CompressionMode.Decompress, leaveOpen: true), cancellationToken).ConfigureAwait(false);
                break;
            case "gz":
                await VerifyGZipAsync(archivePath, expectedUncompressedSize, consume: null, cancellationToken).ConfigureAwait(false);
                break;
            case "br":
                await VerifySizedDecompressionAsync(archivePath, static s => new BrotliStream(s, CompressionMode.Decompress, leaveOpen: true), expectedUncompressedSize, cancellationToken).ConfigureAwait(false);
                break;
            case "tar.zst":
                // ZstdSharp validates each frame's checksum when present, and
                // the tar structure is the completeness signal.
                await VerifyTarStreamAsync(archivePath, static s => new ZstdSharp.DecompressionStream(s, leaveOpen: true), cancellationToken).ConfigureAwait(false);
                break;
            case "tar.lz4":
                await VerifyTarStreamAsync(archivePath, static s => K4os.Compression.LZ4.Streams.LZ4Stream.Decode(s, leaveOpen: true), cancellationToken).ConfigureAwait(false);
                break;
            case "zst":
                // Like brotli: decompress fully and compare against the
                // expected size — zstd's optional content checksum is
                // validated by ZstdSharp when the frame carries one, but
                // presence isn't guaranteed, so the size check is the
                // completeness gate.
                await VerifySizedDecompressionAsync(archivePath, static s => new ZstdSharp.DecompressionStream(s, leaveOpen: true), expectedUncompressedSize, cancellationToken).ConfigureAwait(false);
                break;
            case "lz4":
                await VerifySizedDecompressionAsync(archivePath, static s => K4os.Compression.LZ4.Streams.LZ4Stream.Decode(s, leaveOpen: true), expectedUncompressedSize, cancellationToken).ConfigureAwait(false);
                break;
            default:
                await VerifyReadableAsync(archivePath, cancellationToken).ConfigureAwait(false);
                break;
        }
    }



    private static async Task VerifyZipAsync(string path, CancellationToken cancellationToken)
    {
        var stream = File.OpenRead(path);
        await using (stream.ConfigureAwait(false))
        {
            var archive = await ZipArchive.CreateAsync(stream, ZipArchiveMode.Read, leaveOpen: true, entryNameEncoding: null, cancellationToken).ConfigureAwait(false);
            await using (archive.ConfigureAwait(false))
            {
                // ZipArchive does not check an entry's CRC-32 on read: a flipped byte
                // in the entry data (always visible for stored entries, and often
                // decodable garbage for deflated ones) reads back without error while
                // the central directory stays intact. Compare the CRC-32 and length
                // the archive recorded against the data actually read.
                foreach (var entry in archive.Entries)
                {
                    var crc = new Crc32();
                    long count;

                    var entryStream = await entry.OpenAsync(cancellationToken).ConfigureAwait(false);
                    await using (entryStream.ConfigureAwait(false))
                    {
                        count = await DrainAsync(entryStream, crc, cancellationToken).ConfigureAwait(false);
                    }

                    if (crc.GetCurrentHashAsUInt32() != entry.Crc32 || count != entry.Length)
                    {
                        throw new InvalidDataException($"Zip entry '{entry.FullName}' does not match its recorded CRC-32/length — the archive is corrupt.");
                    }
                }
            }
        }
    }



    private static async Task VerifyTarStreamAsync(string path, Func<Stream, Stream> decompressorFactory, CancellationToken cancellationToken)
    {
        var fileStream = File.OpenRead(path);
        await using (fileStream.ConfigureAwait(false))
        {
            var decompressionStream = decompressorFactory(fileStream);
            await using (decompressionStream.ConfigureAwait(false))
            {
                await WalkTarAsync(decompressionStream, cancellationToken).ConfigureAwait(false);
            }
        }
    }



    private static async Task WalkTarAsync(Stream tarStream, CancellationToken cancellationToken)
    {
        var tarReader = new TarReader(tarStream, leaveOpen: true);
        await using (tarReader.ConfigureAwait(false))
        {
            while (await tarReader.GetNextEntryAsync(cancellationToken: cancellationToken).ConfigureAwait(false) is { } entry)
            {
                if (entry.DataStream != null)
                {
                    await entry.DataStream.CopyToAsync(Stream.Null, cancellationToken).ConfigureAwait(false);
                }
            }
        }
    }



    private static async Task VerifyGZipAsync
    (
        string path,
        long? expectedSize,
        Func<Stream, CancellationToken, Task>? consume,
        CancellationToken cancellationToken
    )
    {
        // Decompress the whole stream, computing the CRC-32 and length of the
        // output ourselves, then compare against the trailer the gzip format
        // stores in the file's last 8 bytes (CRC-32, then ISIZE = length mod
        // 2^32). GZipStream does not perform this check when the stream is
        // truncated — it just stops. For tar.gz, `consume` (the tar walk)
        // reads the decompressed data first through the same hashing stream,
        // so the archive is read only once.
        var fileStream = File.OpenRead(path);
        await using (fileStream.ConfigureAwait(false))
        {
            var crc = new Crc32();
            long count;

            var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress, leaveOpen: true);
            await using (gzipStream.ConfigureAwait(false))
            {
                var hashing = new HashingReadStream(gzipStream, crc);
                await using (hashing.ConfigureAwait(false))
                {
                    if (consume != null)
                    {
                        await consume(hashing, cancellationToken).ConfigureAwait(false);
                    }

                    // Whatever the consumer left unread (all of it for plain gz;
                    // the tar end-of-archive padding for tar.gz) still counts.
                    await DrainAsync(hashing, hasher: null, cancellationToken).ConfigureAwait(false);
                    count = hashing.BytesRead;
                }
            }

            if (fileStream.Length < 18)
            {
                // Smaller than the minimal gzip member (10-byte header + 8-byte
                // trailer): cannot be complete.
                throw new InvalidDataException($"Gzip archive is too small to be complete: {fileStream.Length} bytes.");
            }

            var trailer = new byte[8];
            fileStream.Seek(-8, SeekOrigin.End);
            await fileStream.ReadExactlyAsync(trailer, cancellationToken).ConfigureAwait(false);

            var storedCrc = BinaryPrimitives.ReadUInt32LittleEndian(trailer);
            var storedSize = BinaryPrimitives.ReadUInt32LittleEndian(trailer.AsSpan(4));
            var actualCrc = BinaryPrimitives.ReadUInt32LittleEndian(crc.GetCurrentHash());

            if (storedCrc != actualCrc || storedSize != (uint)count)
            {
                throw new InvalidDataException("Gzip trailer (CRC-32/length) does not match the decompressed data — the archive is truncated or corrupt.");
            }

            EnsureExpectedSize(count, expectedSize);
        }
    }



    private static async Task VerifySizedDecompressionAsync(string path, Func<Stream, Stream> decompressorFactory, long? expectedSize, CancellationToken cancellationToken)
    {
        // For formats without a mandatory checksum/length trailer (brotli, plus
        // zstd and lz4 which make theirs optional), decompressing without error
        // proves very little on a truncated stream. The expected-size
        // comparison (available whenever the caller compressed a known
        // source) is the completeness check.
        var fileStream = File.OpenRead(path);
        await using (fileStream.ConfigureAwait(false))
        {
            long count;
            var decompressionStream = decompressorFactory(fileStream);
            await using (decompressionStream.ConfigureAwait(false))
            {
                count = await DrainAsync(decompressionStream, hasher: null, cancellationToken).ConfigureAwait(false);
            }

            EnsureExpectedSize(count, expectedSize);
        }
    }



    private static void EnsureExpectedSize(long actual, long? expected)
    {
        if (expected.HasValue && actual != expected.Value)
        {
            throw new InvalidDataException($"Decompressed length ({actual}) does not match the source length ({expected.Value}) — the archive is truncated or corrupt.");
        }
    }



    private static async Task<long> DrainAsync(Stream stream, Crc32? hasher, CancellationToken cancellationToken)
    {
        var buffer = ArrayPool<byte>.Shared.Rent(81920);

        try
        {
            long total = 0;
            int read;

            while ((read = await stream.ReadAsync(buffer, cancellationToken).ConfigureAwait(false)) > 0)
            {
                hasher?.Append(buffer.AsSpan(0, read));
                total += read;
            }

            return total;
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }



    private static async Task VerifyReadableAsync(string path, CancellationToken cancellationToken)
    {
        var stream = File.OpenRead(path);
        await using (stream.ConfigureAwait(false))
        {
            await stream.CopyToAsync(Stream.Null, cancellationToken).ConfigureAwait(false);
        }
    }
}
