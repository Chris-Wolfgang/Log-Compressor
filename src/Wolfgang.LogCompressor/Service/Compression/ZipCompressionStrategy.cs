using System.IO.Compression;
using Wolfgang.LogCompressor.Abstraction;

namespace Wolfgang.LogCompressor.Service.Compression;

/// <summary>
/// Compression strategy using the ZIP archive format.
/// </summary>
internal sealed class ZipCompressionStrategy : ICompressionStrategy
{
    private readonly CompressionLevel _level;



    /// <summary>
    /// Initializes a new instance of the <see cref="ZipCompressionStrategy"/> class.
    /// </summary>
    /// <param name="level">The compression level to use.</param>
    public ZipCompressionStrategy(CompressionLevel level = CompressionLevel.SmallestSize)
    {
        _level = level;
    }



    /// <inheritdoc />
    public string FileExtension => "zip";



    /// <inheritdoc />
    public string BundleFileExtension => "zip";



    /// <inheritdoc />
    public async Task CompressFileAsync
    (
        Stream inputStream,
        Stream outputStream,
        string entryName,
        CancellationToken cancellationToken = default
    )
    {
        using var archive = new ZipArchive(outputStream, ZipArchiveMode.Create, leaveOpen: true);
        var entry = archive.CreateEntry(entryName, _level);
        var entryStream = await entry.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using (entryStream.ConfigureAwait(false))
        {
            await inputStream.CopyToAsync(entryStream, cancellationToken).ConfigureAwait(false);
        }
    }



    /// <inheritdoc />
    public async Task CompressFilesAsync
    (
        IReadOnlyList<(Stream Stream, string EntryName)> inputs,
        Stream outputStream,
        CancellationToken cancellationToken = default
    )
    {
        using var archive = new ZipArchive(outputStream, ZipArchiveMode.Create, leaveOpen: true);

        foreach (var (stream, entryName) in inputs)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var entry = archive.CreateEntry(entryName, _level);
            var entryStream = await entry.OpenAsync(cancellationToken).ConfigureAwait(false);
            await using (entryStream.ConfigureAwait(false))
            {
                await stream.CopyToAsync(entryStream, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
