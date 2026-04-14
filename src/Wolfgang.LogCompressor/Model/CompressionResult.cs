namespace Wolfgang.LogCompressor.Model;

/// <summary>
/// The result of a compression operation for a single source file or bundle.
/// </summary>
internal record CompressionResult
{
    /// <summary>
    /// Gets the source file path (or directory path for bundles).
    /// </summary>
    public required string SourcePath { get; init; }



    /// <summary>
    /// Gets the output archive path.
    /// </summary>
    public required string OutputPath { get; init; }



    /// <summary>
    /// Gets the total original size in bytes.
    /// </summary>
    public long OriginalSize { get; init; }



    /// <summary>
    /// Gets the compressed size in bytes.
    /// </summary>
    public long CompressedSize { get; init; }



    /// <summary>
    /// Gets a value indicating whether the operation succeeded.
    /// </summary>
    public bool Success { get; init; }



    /// <summary>
    /// Gets the error message if the operation failed.
    /// </summary>
    public string? ErrorMessage { get; init; }
}
