using System.IO.Compression;

namespace Wolfgang.LogCompressor.Model;

/// <summary>
/// Options for compression operations.
/// </summary>
internal record CompressionOptions
{
    /// <summary>
    /// Gets the path to the source file or directory.
    /// </summary>
    public required string SourcePath { get; init; }



    /// <summary>
    /// Gets the output directory path. When <see langword="null"/>, archives are written to the source directory.
    /// </summary>
    public string? OutputPath { get; init; }



    /// <summary>
    /// Gets a value indicating whether to recurse into subdirectories.
    /// </summary>
    public bool Recurse { get; init; }



    /// <summary>
    /// Gets which timestamp is embedded in generated archive names.
    /// </summary>
    public TimestampSource TimestampSource { get; init; } = TimestampSource.Modified;



    /// <summary>
    /// Gets the custom base name for generated archives, replacing the source
    /// file or folder name. <see langword="null"/> uses the default.
    /// </summary>
    public string? NamePrefix { get; init; }



    /// <summary>
    /// Gets the minimum file age in calendar days. Only files last modified more than this many days ago (before local midnight that many days back) are included.
    /// </summary>
    public int? OlderThanDays { get; init; }



    /// <summary>
    /// Gets the minimum last-modified date filter.
    /// </summary>
    public DateTime? MinDateTime { get; init; }



    /// <summary>
    /// Gets the maximum last-modified date filter.
    /// </summary>
    public DateTime? MaxDateTime { get; init; }



    /// <summary>
    /// Gets the compression format to use.
    /// </summary>
    public CompressionFormat Format { get; init; } = CompressionFormat.Zip;



    /// <summary>
    /// Gets the compression level to use.
    /// </summary>
    public CompressionLevel Level { get; init; } = CompressionLevel.SmallestSize;



    /// <summary>
    /// Gets the glob patterns to include. Only files matching at least one pattern are processed.
    /// </summary>
    public IReadOnlyList<string> IncludePatterns { get; init; } = [];



    /// <summary>
    /// Gets the glob patterns to exclude. Files matching any pattern are skipped.
    /// </summary>
    public IReadOnlyList<string> ExcludePatterns { get; init; } = [];



    /// <summary>
    /// Gets a value indicating whether to verify archive integrity before deleting originals.
    /// </summary>
    public bool Verify { get; init; } = true;



    /// <summary>
    /// Gets the report format. When <see langword="null"/>, no report file is written.
    /// </summary>
    public string? ReportFormat { get; init; }



    /// <summary>
    /// Gets the report output path.
    /// </summary>
    public string? ReportPath { get; init; }



    /// <summary>
    /// Gets the number of days after which compressed archives should be deleted.
    /// </summary>
    public int? DeleteArchivesOlderThanDays { get; init; }



    /// <summary>
    /// Gets a value indicating whether to disable single-instance directory locking.
    /// </summary>
    public bool NoLock { get; init; }
}
