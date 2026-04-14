using System.Formats.Tar;
using System.IO.Compression;
using Wolfgang.LogCompressor.Abstraction;

namespace Wolfgang.LogCompressor.Service.Compression;

/// <summary>
/// Compression strategy using the GZip format. Bundles use tar+gzip.
/// </summary>
internal sealed class GZipCompressionStrategy : ICompressionStrategy
{
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
        await using var gzipStream = new GZipStream(outputStream, CompressionLevel.SmallestSize, leaveOpen: true);
        await inputStream.CopyToAsync(gzipStream, cancellationToken).ConfigureAwait(false);
    }



    /// <inheritdoc />
    public async Task CompressFilesAsync
    (
        IReadOnlyList<(Stream Stream, string EntryName)> inputs,
        Stream outputStream,
        CancellationToken cancellationToken = default
    )
    {
        await using var gzipStream = new GZipStream(outputStream, CompressionLevel.SmallestSize, leaveOpen: true);
        await using var tarWriter = new TarWriter(gzipStream, leaveOpen: true);

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
