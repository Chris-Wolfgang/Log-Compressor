using System.Formats.Tar;
using System.IO.Compression;
using Wolfgang.LogCompressor.Abstraction;

namespace Wolfgang.LogCompressor.Service.Compression;

/// <summary>
/// Compression strategy using the Brotli format. Bundles use tar+brotli.
/// </summary>
internal sealed class BrotliCompressionStrategy : ICompressionStrategy
{
    private readonly CompressionLevel _level;



    /// <summary>
    /// Initializes a new instance of the <see cref="BrotliCompressionStrategy"/> class.
    /// </summary>
    /// <param name="level">The compression level to use.</param>
    public BrotliCompressionStrategy(CompressionLevel level = CompressionLevel.SmallestSize)
    {
        _level = level;
    }



    /// <inheritdoc />
    public string FileExtension => "br";



    /// <inheritdoc />
    public string BundleFileExtension => "tar.br";



    /// <inheritdoc />
    public async Task CompressFileAsync
    (
        Stream inputStream,
        Stream outputStream,
        string entryName,
        CancellationToken cancellationToken = default
    )
    {
        _ = entryName; // Not used by single-stream Brotli format
        var brotliStream = new BrotliStream(outputStream, _level, leaveOpen: true);
        await using var brotliStreamDisposal = brotliStream.ConfigureAwait(false);
        await inputStream.CopyToAsync(brotliStream, cancellationToken).ConfigureAwait(false);
    }



    /// <inheritdoc />
    public async Task CompressFilesAsync
    (
        IAsyncEnumerable<(Stream Stream, string EntryName)> inputs,
        Stream outputStream,
        CancellationToken cancellationToken = default
    )
    {
        var brotliStream = new BrotliStream(outputStream, _level, leaveOpen: true);
        await using var brotliStreamDisposal = brotliStream.ConfigureAwait(false);
        var tarWriter = new TarWriter(brotliStream, leaveOpen: true);
        await using var tarWriterDisposal = tarWriter.ConfigureAwait(false);

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
