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
    /// Gets the minimum file age in calendar days. Only files last modified this many days ago or more are included.
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
}
