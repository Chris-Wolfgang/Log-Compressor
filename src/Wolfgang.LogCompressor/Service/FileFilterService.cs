using Microsoft.Extensions.FileSystemGlobbing;
using Wolfgang.LogCompressor.Abstraction;

namespace Wolfgang.LogCompressor.Service;

/// <summary>
/// Filters files based on last-modified date and glob pattern criteria.
/// </summary>
internal sealed class FileFilterService : IFileFilter
{
    /// <inheritdoc />
    public IReadOnlyList<FileInfo> Apply
    (
        IEnumerable<FileInfo> files,
        int? olderThanDays,
        DateTime? minDateTime,
        DateTime? maxDateTime,
        IReadOnlyList<string>? includePatterns = null,
        IReadOnlyList<string>? excludePatterns = null
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

        if (includePatterns is { Count: > 0 })
        {
            var matcher = new Matcher();
            foreach (var pattern in includePatterns)
            {
                matcher.AddInclude(pattern);
            }

            query = query.Where(f => matcher.Match(f.Name).HasMatches);
        }

        if (excludePatterns is { Count: > 0 })
        {
            var matcher = new Matcher();
            foreach (var pattern in excludePatterns)
            {
                matcher.AddInclude(pattern);
            }

            query = query.Where(f => !matcher.Match(f.Name).HasMatches);
        }

        return query.ToList();
    }
}
