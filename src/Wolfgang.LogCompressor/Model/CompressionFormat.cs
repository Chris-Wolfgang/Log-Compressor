namespace Wolfgang.LogCompressor.Model;

/// <summary>
/// Supported compression formats.
/// </summary>
internal enum CompressionFormat
{
    /// <summary>ZIP archive format.</summary>
    Zip,

    /// <summary>GZip compression format.</summary>
    Gz,

    /// <summary>Brotli compression format.</summary>
    Brotli,

    /// <summary>Zstandard (zstd) compression format.</summary>
    Zstd,

    /// <summary>LZ4 compression format.</summary>
    Lz4
}
