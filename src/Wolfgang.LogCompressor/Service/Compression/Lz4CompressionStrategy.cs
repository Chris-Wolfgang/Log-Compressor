using System.Formats.Tar;
using System.IO.Compression;
using K4os.Compression.LZ4;
using K4os.Compression.LZ4.Streams;
using Wolfgang.LogCompressor.Abstraction;

namespace Wolfgang.LogCompressor.Service.Compression;

/// <summary>
/// Compression strategy using the LZ4 format. Bundles use tar+lz4.
/// </summary>
internal sealed class Lz4CompressionStrategy : ICompressionStrategy
{
    private readonly LZ4Level _level;



    /// <summary>
    /// Initializes a new instance of the <see cref="Lz4CompressionStrategy"/> class.
    /// </summary>
    /// <param name="level">The compression level to use.</param>
    public Lz4CompressionStrategy(CompressionLevel level = CompressionLevel.SmallestSize)
    {
        _level = MapLevel(level);
    }



    /// <inheritdoc />
    public string FileExtension => "lz4";



    /// <inheritdoc />
    public string BundleFileExtension => "tar.lz4";



    /// <inheritdoc />
    public async Task CompressFileAsync
    (
        Stream inputStream,
        Stream outputStream,
        string entryName,
        CancellationToken cancellationToken = default
    )
    {
        _ = entryName; // Not used by single-stream LZ4 format
        await using var lz4Stream = LZ4Stream.Encode(outputStream, _level, leaveOpen: true);
        await inputStream.CopyToAsync(lz4Stream, cancellationToken).ConfigureAwait(false);
    }



    /// <inheritdoc />
    public async Task CompressFilesAsync
    (
        IAsyncEnumerable<(Stream Stream, string EntryName)> inputs,
        Stream outputStream,
        CancellationToken cancellationToken = default
    )
    {
        await using var lz4Stream = LZ4Stream.Encode(outputStream, _level, leaveOpen: true);
        await using var tarWriter = new TarWriter(lz4Stream, leaveOpen: true);

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



    // Map the framework CompressionLevel onto LZ4's levels. SmallestSize uses the
    // high-compression maximum; Optimal uses a mid HC level; Fastest uses the
    // (very fast) default block level.
    private static LZ4Level MapLevel(CompressionLevel level)
    {
        return level switch
        {
            CompressionLevel.Fastest => LZ4Level.L00_FAST,
            CompressionLevel.Optimal => LZ4Level.L09_HC,
            CompressionLevel.NoCompression => LZ4Level.L00_FAST,
            _ => LZ4Level.L12_MAX
        };
    }
}
