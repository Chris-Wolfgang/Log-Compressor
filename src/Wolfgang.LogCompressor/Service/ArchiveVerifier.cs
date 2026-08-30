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
    public async Task<bool> VerifyAsync(string archivePath, string format, long? expectedUncompressedSize = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(archivePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(format);

        if (expectedUncompressedSize is < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(expectedUncompressedSize));
        }

        try
        {
            await VerifyByFormatAsync(archivePath, format, expectedUncompressedSize).ConfigureAwait(false);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Archive verification failed for {Path}: {Message}", archivePath, ex.Message);
            return false;
        }
    }



    private static async Task VerifyByFormatAsync(string archivePath, string format, long? expectedUncompressedSize)
    {
        switch (format.ToLowerInvariant())
        {
            case "zip":
                // ZipArchive validates each entry's recorded CRC-32 as the
                // entry stream is drained, and a truncated central
                // directory fails construction — no extra checks needed.
                await VerifyZipAsync(archivePath).ConfigureAwait(false);
                break;
            case "tar.gz":
                await VerifyTarStreamAsync(archivePath, static s => new GZipStream(s, CompressionMode.Decompress, leaveOpen: true)).ConfigureAwait(false);
                // The tar walk proves structure; this pass proves the gzip
                // stream is complete and uncorrupted via its own trailer.
                await VerifyGZipAsync(archivePath, expectedSize: null).ConfigureAwait(false);
                break;
            case "tar.br":
                // Brotli carries no checksum; the 512-byte tar structure is
                // the completeness signal (a truncation mid-entry breaks
                // the walk).
                await VerifyTarStreamAsync(archivePath, static s => new BrotliStream(s, CompressionMode.Decompress, leaveOpen: true)).ConfigureAwait(false);
                break;
            case "gz":
                await VerifyGZipAsync(archivePath, expectedUncompressedSize).ConfigureAwait(false);
                break;
            case "br":
                await VerifySizedDecompressionAsync(archivePath, static s => new BrotliStream(s, CompressionMode.Decompress, leaveOpen: true), expectedUncompressedSize).ConfigureAwait(false);
                break;
            case "tar.zst":
                // ZstdSharp validates each frame's checksum when present;
                // the tar structure is the completeness signal.
                await VerifyTarStreamAsync(archivePath, static s => new ZstdSharp.DecompressionStream(s, leaveOpen: true)).ConfigureAwait(false);
                break;
            case "tar.lz4":
                await VerifyTarStreamAsync(archivePath, static s => K4os.Compression.LZ4.Streams.LZ4Stream.Decode(s, leaveOpen: true)).ConfigureAwait(false);
                break;
            case "zst":
                // Like brotli: decompress fully and compare against the
                // expected size — zstd's optional content checksum is
                // validated by ZstdSharp when the frame carries one, but
                // presence isn't guaranteed, so the size check is the
                // completeness gate.
                await VerifySizedDecompressionAsync(archivePath, static s => new ZstdSharp.DecompressionStream(s, leaveOpen: true), expectedUncompressedSize).ConfigureAwait(false);
                break;
            case "lz4":
                await VerifySizedDecompressionAsync(archivePath, static s => K4os.Compression.LZ4.Streams.LZ4Stream.Decode(s, leaveOpen: true), expectedUncompressedSize).ConfigureAwait(false);
                break;
            default:
                await VerifyReadableAsync(archivePath).ConfigureAwait(false);
                break;
        }
    }



    private static async Task VerifyZipAsync(string path)
    {
        await using var stream = File.OpenRead(path);
        using var archive = new ZipArchive(stream, ZipArchiveMode.Read);

        foreach (var entry in archive.Entries)
        {
            var entryStream = await entry.OpenAsync().ConfigureAwait(false);
            await using (entryStream.ConfigureAwait(false))
            {
                await entryStream.CopyToAsync(Stream.Null).ConfigureAwait(false);
            }
        }
    }



    private static async Task VerifyTarStreamAsync(string path, Func<Stream, Stream> decompressorFactory)
    {
        await using var fileStream = File.OpenRead(path);
        var decompressionStream = decompressorFactory(fileStream);

        await using (decompressionStream.ConfigureAwait(false))
        {
            await using var tarReader = new TarReader(decompressionStream);

            while (await tarReader.GetNextEntryAsync().ConfigureAwait(false) is { } entry)
            {
                if (entry.DataStream != null)
                {
                    await entry.DataStream.CopyToAsync(Stream.Null).ConfigureAwait(false);
                }
            }
        }
    }



    private static async Task VerifyGZipAsync(string path, long? expectedSize)
    {
        // Decompress the whole stream, computing the CRC-32 and length of the
        // output ourselves, then compare against the trailer the gzip format
        // stores in the file's last 8 bytes (CRC-32, then ISIZE = length mod
        // 2^32). GZipStream does not perform this check when the stream is
        // truncated — it just stops.
        var crc = new Crc32();
        long count;

        await using var fileStream = File.OpenRead(path);

        var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress, leaveOpen: true);
        await using (gzipStream.ConfigureAwait(false))
        {
            count = await DrainAsync(gzipStream, crc).ConfigureAwait(false);
        }

        if (fileStream.Length < 18)
        {
            // Smaller than the minimal gzip member (10-byte header + 8-byte
            // trailer): cannot be complete.
            throw new InvalidDataException($"Gzip archive is too small to be complete: {fileStream.Length} bytes.");
        }

        var trailer = new byte[8];
        fileStream.Seek(-8, SeekOrigin.End);
        await fileStream.ReadExactlyAsync(trailer).ConfigureAwait(false);

        var storedCrc = BinaryPrimitives.ReadUInt32LittleEndian(trailer);
        var storedSize = BinaryPrimitives.ReadUInt32LittleEndian(trailer.AsSpan(4));
        var actualCrc = BinaryPrimitives.ReadUInt32LittleEndian(crc.GetCurrentHash());

        if (storedCrc != actualCrc || storedSize != (uint)count)
        {
            throw new InvalidDataException("Gzip trailer (CRC-32/length) does not match the decompressed data — the archive is truncated or corrupt.");
        }

        EnsureExpectedSize(count, expectedSize);
    }



    private static async Task VerifySizedDecompressionAsync(string path, Func<Stream, Stream> decompressorFactory, long? expectedSize)
    {
        // For formats without a mandatory checksum/length trailer (brotli;
        // zstd and lz4 make theirs optional), decompressing without error
        // proves very little on a truncated stream. The expected-size
        // comparison (available whenever the caller compressed a known
        // source) is the completeness check.
        await using var fileStream = File.OpenRead(path);

        long count;
        var decompressionStream = decompressorFactory(fileStream);
        await using (decompressionStream.ConfigureAwait(false))
        {
            count = await DrainAsync(decompressionStream, hasher: null).ConfigureAwait(false);
        }

        EnsureExpectedSize(count, expectedSize);
    }



    private static void EnsureExpectedSize(long actual, long? expected)
    {
        if (expected.HasValue && actual != expected.Value)
        {
            throw new InvalidDataException($"Decompressed length ({actual}) does not match the source length ({expected.Value}) — the archive is truncated or corrupt.");
        }
    }



    private static async Task<long> DrainAsync(Stream stream, Crc32? hasher)
    {
        var buffer = ArrayPool<byte>.Shared.Rent(81920);

        try
        {
            long total = 0;
            int read;

            while ((read = await stream.ReadAsync(buffer).ConfigureAwait(false)) > 0)
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



    private static async Task VerifyReadableAsync(string path)
    {
        await using var stream = File.OpenRead(path);
        await stream.CopyToAsync(Stream.Null).ConfigureAwait(false);
    }
}
