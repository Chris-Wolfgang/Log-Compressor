namespace Wolfgang.LogCompressor.Model;

/// <summary>
/// Options for decompression operations.
/// </summary>
internal record DecompressionOptions
{
    /// <summary>
    /// Gets the path to the source archive file or a directory containing archives.
    /// </summary>
    public required string SourcePath { get; init; }



    /// <summary>
    /// Gets the output directory path. When <see langword="null"/>, files are extracted next to each archive.
    /// </summary>
    public string? OutputPath { get; init; }



    /// <summary>
    /// Gets a value indicating whether to recurse into subdirectories when the source is a directory.
    /// </summary>
    public bool Recurse { get; init; }



    /// <summary>
    /// Gets the glob patterns of archive names to include. Empty means all recognized archives.
    /// </summary>
    public IReadOnlyList<string> IncludePatterns { get; init; } = [];



    /// <summary>
    /// Gets the glob patterns of archive names to exclude.
    /// </summary>
    public IReadOnlyList<string> ExcludePatterns { get; init; } = [];



    /// <summary>
    /// Gets a value indicating whether existing files at an extraction target may be overwritten.
    /// When <see langword="false"/> (the default), a collision fails that archive and it is kept.
    /// </summary>
    public bool Force { get; init; }



    /// <summary>
    /// Gets a value indicating whether archives are kept after successful extraction.
    /// When <see langword="false"/> (the default), an archive is deleted once every entry
    /// extracted successfully — the mirror of compress's verify-then-delete contract.
    /// </summary>
    public bool KeepArchives { get; init; }



    /// <summary>
    /// Gets a value indicating whether the single-instance process lock is skipped.
    /// </summary>
    public bool NoLock { get; init; }



    /// <summary>
    /// Gets the batch error policy (skip / fail / retry-then-skip).
    /// </summary>
    public ErrorPolicy OnError { get; init; } = ErrorPolicy.Default;



    /// <summary>
    /// Gets the report format ("json" or "csv"), or <see langword="null"/> for no report.
    /// </summary>
    public string? ReportFormat { get; init; }



    /// <summary>
    /// Gets the report output path.
    /// </summary>
    public string? ReportPath { get; init; }
}
