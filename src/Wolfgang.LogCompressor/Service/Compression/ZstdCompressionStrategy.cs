using System.Formats.Tar;
using System.IO.Compression;
using Wolfgang.LogCompressor.Abstraction;
using ZstdSharp;

namespace Wolfgang.LogCompressor.Service.Compression;

/// <summary>
/// Compression strategy using the Zstandard (zstd) format. Bundles use tar+zstd.
/// </summary>
internal sealed class ZstdCompressionStrategy : ICompressionStrategy
{
    private readonly int _level;



    /// <summary>
    /// Initializes a new instance of the <see cref="ZstdCompressionStrategy"/> class.
    /// </summary>
    /// <param name="level">The compression level to use.</param>
    public ZstdCompressionStrategy(CompressionLevel level = CompressionLevel.SmallestSize)
    {
        _level = MapLevel(level);
    }



    /// <inheritdoc />
    public string FileExtension => "zst";



    /// <inheritdoc />
    public string BundleFileExtension => "tar.zst";



    /// <inheritdoc />
    public async Task CompressFileAsync
    (
        Stream inputStream,
        Stream outputStream,
        string entryName,
        CancellationToken cancellationToken = default
    )
    {
        _ = entryName; // Not used by single-stream zstd format
        await using var zstdStream = new CompressionStream(outputStream, _level, leaveOpen: true);
        await inputStream.CopyToAsync(zstdStream, cancellationToken).ConfigureAwait(false);
    }



    /// <inheritdoc />
    public async Task CompressFilesAsync
    (
        IEnumerable<(Stream Stream, string EntryName)> inputs,
        Stream outputStream,
        CancellationToken cancellationToken = default
    )
    {
        await using var zstdStream = new CompressionStream(outputStream, _level, leaveOpen: true);
        await using var tarWriter = new TarWriter(zstdStream, leaveOpen: true);

        foreach (var (stream, entryName) in inputs)
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



    // Map the framework CompressionLevel onto zstd's 1..22 scale. SmallestSize uses
    // 19 (the practical maximum before zstd's "ultra" levels); Optimal uses zstd's
    // own default of 3; Fastest uses 1.
    private static int MapLevel(CompressionLevel level)
    {
        return level switch
        {
            CompressionLevel.Fastest => 1,
            CompressionLevel.Optimal => 3,
            CompressionLevel.NoCompression => 1,
            _ => 19
        };
    }
}
