using System.Formats.Tar;
using System.IO.Compression;
using Wolfgang.LogCompressor.Abstraction;

namespace Wolfgang.LogCompressor.Service.Compression;

/// <summary>
/// Compression strategy using the Brotli format. Bundles use tar+brotli.
/// </summary>
internal sealed class BrotliCompressionStrategy : ICompressionStrategy
{
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
        await using var brotliStream = new BrotliStream(outputStream, CompressionLevel.SmallestSize, leaveOpen: true);
        await inputStream.CopyToAsync(brotliStream, cancellationToken).ConfigureAwait(false);
    }



    /// <inheritdoc />
    public async Task CompressFilesAsync
    (
        IReadOnlyList<(Stream Stream, string EntryName)> inputs,
        Stream outputStream,
        CancellationToken cancellationToken = default
    )
    {
        await using var brotliStream = new BrotliStream(outputStream, CompressionLevel.SmallestSize, leaveOpen: true);
        await using var tarWriter = new TarWriter(brotliStream, leaveOpen: true);

        foreach (var (stream, entryName) in inputs)
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
