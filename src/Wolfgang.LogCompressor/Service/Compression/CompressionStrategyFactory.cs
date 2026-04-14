using Wolfgang.LogCompressor.Abstraction;
using Wolfgang.LogCompressor.Model;

namespace Wolfgang.LogCompressor.Service.Compression;

/// <summary>
/// Creates the appropriate <see cref="ICompressionStrategy"/> for a given <see cref="CompressionFormat"/>.
/// </summary>
internal class CompressionStrategyFactory
{
    /// <summary>
    /// Creates a compression strategy for the specified format.
    /// </summary>
    /// <param name="format">The compression format.</param>
    /// <returns>An <see cref="ICompressionStrategy"/> instance.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="format"/> is not a known value.</exception>
    public virtual ICompressionStrategy Create(CompressionFormat format)
    {
        return format switch
        {
            CompressionFormat.Zip => new ZipCompressionStrategy(),
            CompressionFormat.Gz => new GZipCompressionStrategy(),
            CompressionFormat.Brotli => new BrotliCompressionStrategy(),
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, "Unsupported compression format.")
        };
    }
}
