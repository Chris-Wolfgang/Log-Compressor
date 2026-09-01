namespace Wolfgang.LogCompressor.Model;

/// <summary>
/// The result of a compression operation for a single source file or bundle.
/// </summary>
internal record CompressionResult
{
    /// <summary>
    /// Gets the source path: the file that was compressed, or — for bundles — the
    /// bundle's source path, which may be a directory or a single file.
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



    /// <summary>
    /// Gets the number of input items that were skipped after failing (the
    /// <c>--on-error</c> skip / exhausted-retry outcome). Non-zero with
    /// <see cref="Success"/> <see langword="false"/> means the operation
    /// completed degraded rather than failed outright — commands map this to
    /// exit code 3 (completed with skips) instead of 11.
    /// </summary>
    public int SkippedCount { get; init; }
}
