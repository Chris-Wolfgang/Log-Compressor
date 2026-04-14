using System.Formats.Tar;
using System.IO.Compression;
using Wolfgang.LogCompressor.Abstraction;

namespace Wolfgang.LogCompressor.Service.Compression;

/// <summary>
/// Compression strategy using the GZip format. Bundles use tar+gzip.
/// </summary>
internal sealed class GZipCompressionStrategy : ICompressionStrategy
{
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
        await using var gzipStream = new GZipStream(outputStream, _level, leaveOpen: true);
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
        await using var gzipStream = new GZipStream(outputStream, _level, leaveOpen: true);
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
