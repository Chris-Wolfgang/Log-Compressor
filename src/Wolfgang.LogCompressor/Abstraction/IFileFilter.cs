namespace Wolfgang.LogCompressor.Abstraction;

/// <summary>
/// Filters files based on date and pattern criteria.
/// </summary>
internal interface IFileFilter
{
    /// <summary>
    /// Applies date-based and pattern-based filters to a collection of files.
    /// </summary>
    /// <param name="files">The files to filter.</param>
    /// <param name="olderThanDays">If specified, only files last modified this many days ago or more.</param>
    /// <param name="minDateTime">If specified, only files modified on or after this date.</param>
    /// <param name="maxDateTime">If specified, only files modified on or before this date.</param>
    /// <param name="includePatterns">If non-empty, only files matching at least one pattern are included.</param>
    /// <param name="excludePatterns">Files matching any pattern are excluded.</param>
    /// <returns>The filtered list of files.</returns>
    IReadOnlyList<FileInfo> Apply
    (
        IEnumerable<FileInfo> files,
        int? olderThanDays,
        DateTime? minDateTime,
        DateTime? maxDateTime,
        IReadOnlyList<string>? includePatterns = null,
        IReadOnlyList<string>? excludePatterns = null
    );
}
