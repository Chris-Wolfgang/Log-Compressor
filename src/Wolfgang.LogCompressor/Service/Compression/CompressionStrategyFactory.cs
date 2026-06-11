using System.IO.Compression;
using Wolfgang.LogCompressor.Abstraction;
using Wolfgang.LogCompressor.Model;

namespace Wolfgang.LogCompressor.Service.Compression;

/// <summary>
/// Creates the appropriate <see cref="ICompressionStrategy"/> for a given <see cref="CompressionFormat"/>.
/// </summary>
internal class CompressionStrategyFactory
{
    /// <summary>
    /// Creates a compression strategy for the specified format and compression level.
    /// </summary>
    /// <param name="format">The compression format.</param>
    /// <param name="level">The compression level. Defaults to <see cref="CompressionLevel.SmallestSize"/>.</param>
    /// <returns>An <see cref="ICompressionStrategy"/> instance.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="format"/> is not a known value.</exception>
    public virtual ICompressionStrategy Create
    (
        CompressionFormat format,
        CompressionLevel level = CompressionLevel.SmallestSize
    )
    {
        return format switch
        {
            CompressionFormat.Zip => new ZipCompressionStrategy(level),
            CompressionFormat.Gz => new GZipCompressionStrategy(level),
            CompressionFormat.Brotli => new BrotliCompressionStrategy(level),
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, "Unsupported compression format.")
        };
    }
}
