using Wolfgang.LogCompressor.Abstraction;

namespace Wolfgang.LogCompressor.Service;

/// <summary>
/// Filters files based on last-modified date criteria.
/// </summary>
internal sealed class FileFilterService : IFileFilter
{
    /// <inheritdoc />
    public IReadOnlyList<FileInfo> Apply
    (
        IEnumerable<FileInfo> files,
        int? olderThanDays,
        DateTime? minDateTime,
        DateTime? maxDateTime
    )
    {
        ArgumentNullException.ThrowIfNull(files);

        var query = files.AsEnumerable();

        if (olderThanDays.HasValue)
        {
            var threshold = DateTime.Today.AddDays(-olderThanDays.Value);
            query = query.Where(f => f.LastWriteTime < threshold);
        }

        if (minDateTime.HasValue)
        {
            query = query.Where(f => f.LastWriteTime >= minDateTime.Value);
        }

        if (maxDateTime.HasValue)
        {
            query = query.Where(f => f.LastWriteTime <= maxDateTime.Value);
        }

        return query.ToList();
    }
}
