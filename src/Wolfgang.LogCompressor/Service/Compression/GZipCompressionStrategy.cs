using System.Formats.Tar;
using System.IO.Compression;
using Wolfgang.LogCompressor.Abstraction;

namespace Wolfgang.LogCompressor.Service.Compression;

/// <summary>
/// Compression strategy using the GZip format. Bundles use tar+gzip.
/// </summary>
internal sealed class GZipCompressionStrategy : ICompressionStrategy
{
    // .NET's GZipStream emits NOTHING at all (no header, no trailer) when no
    // byte is ever written through it, so an empty source would produce a
    // 0-byte, malformed .gz that fails verification (found by the fuzz sweep:
    // seed 6ynpRrX3UoE1). Neither Flush nor an empty write completes the
    // framing, so empty sources get the canonical empty gzip member instead:
    // 10-byte header, empty final deflate block (03 00), zero CRC-32/ISIZE.
    private static readonly byte[] EmptyGzipMember =
    [
        0x1F, 0x8B, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x03, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
    ];



    private readonly CompressionLevel _level;



    /// <summary>
    /// Initializes a new instance of the <see cref="GZipCompressionStrategy"/> class.
    /// </summary>
    /// <param name="level">The compression level to use.</param>
    public GZipCompressionStrategy(CompressionLevel level = CompressionLevel.SmallestSize)
    {
        _level = level;
    }



    /// <inheritdoc />
    public string FileExtension => "gz";



    /// <inheritdoc />
    public string BundleFileExtension => "tar.gz";



    /// <inheritdoc />
    public async Task CompressFileAsync
    (
        Stream inputStream,
        Stream outputStream,
        string entryName,
        CancellationToken cancellationToken = default
    )
    {
        _ = entryName; // Not used by single-stream GZip format

        var buffer = new byte[81920];
        var firstRead = await inputStream.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);
        if (firstRead == 0)
        {
            await outputStream.WriteAsync(EmptyGzipMember, cancellationToken).ConfigureAwait(false);
            return;
        }

        var gzipStream = new GZipStream(outputStream, _level, leaveOpen: true);
        await using (gzipStream.ConfigureAwait(false))
        {
            await gzipStream.WriteAsync(buffer.AsMemory(0, firstRead), cancellationToken).ConfigureAwait(false);
            await inputStream.CopyToAsync(gzipStream, cancellationToken).ConfigureAwait(false);
        }
    }



    /// <inheritdoc />
    public async Task CompressFilesAsync
    (
        IAsyncEnumerable<(Stream Stream, string EntryName)> inputs,
        Stream outputStream,
        CancellationToken cancellationToken = default
    )
    {
        await using var gzipStream = new GZipStream(outputStream, _level, leaveOpen: true);
        await using var tarWriter = new TarWriter(gzipStream, leaveOpen: true);

        await foreach (var (stream, entryName) in inputs.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            // Take ownership of the source stream: dispose it as soon as its entry
            // is written so only one source handle is open at a time.
            await using (stream.ConfigureAwait(false))
            {
                cancellationToken.ThrowIfCancellationRequested();

                var entry = new PaxTarEntry(TarEntryType.RegularFile, entryName)
                {
                    DataStream = stream
                };

                await tarWriter.WriteEntryAsync(entry, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
