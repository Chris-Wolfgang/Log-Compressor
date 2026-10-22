using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Wolfgang.LogCompressor.Abstraction;

namespace Wolfgang.LogCompressor.Service;

/// <summary>
/// Deletes old compressed archives based on a retention policy.
/// </summary>
internal sealed class RetentionService
{
    // A plain array: IsArchiveFile suffix-matches each entry, so a HashSet's
    // O(1) lookup gives no benefit here. Bundle extensions (.tar.gz etc.) are
    // covered by their final-segment suffixes (.gz etc.) — no separate entries.
    private static readonly string[] ArchiveExtensions =
    [
        ".zip", ".gz", ".br", ".zst", ".lz4"
    ];

    // The tail of every name FileNamingService produces: "-<timestamp>" (one
    // file, or a bundle under --timestamp compressed) or "-<min> to <max>"
    // (a bundle), an optional "-N" collision counter, then a logc archive
    // extension. Retention deletes only names that end this way, so unrelated
    // archives that merely share an extension are never touched (#352).
    // A plain Regex, not [GeneratedRegex]: the build uses no source
    // generators (cross-OS reproducible build).
    private static readonly Regex LogcArchiveName = new
    (
        @"-\d{4}-\d{2}-\d{2}_\d{2}-\d{2}-\d{2}( to \d{4}-\d{2}-\d{2}_\d{2}-\d{2}-\d{2})?(-\d+)?\.(zip|gz|br|zst|lz4|tar\.gz|tar\.br|tar\.zst|tar\.lz4)$",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture,
        TimeSpan.FromSeconds(1)
    );

    private readonly IFileSystem _fileSystem;
    private readonly ILogger<RetentionService> _logger;
    private readonly TimeProvider _timeProvider;



    /// <summary>
    /// Initializes a new instance of the <see cref="RetentionService"/> class.
    /// </summary>
    /// <param name="fileSystem">The file system abstraction.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="timeProvider">The time source for the age cutoff.</param>
    public RetentionService(IFileSystem fileSystem, ILogger<RetentionService> logger, TimeProvider timeProvider)
    {
        ArgumentNullException.ThrowIfNull(fileSystem);
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(timeProvider);

        _fileSystem = fileSystem;
        _logger = logger;
        _timeProvider = timeProvider;
    }



    /// <summary>
    /// Deletes archives that logc produced and that are older than the specified number of days.
    /// Only files whose names match logc's own naming pattern
    /// (<c>&lt;name&gt;-yyyy-MM-dd_HH-mm-ss[ to yyyy-MM-dd_HH-mm-ss][-N].&lt;archive extension&gt;</c>)
    /// are candidates; any other file is left alone, whatever its extension.
    /// </summary>
    /// <param name="directory">The directory to scan.</param>
    /// <param name="olderThanDays">Delete archives last modified more than this many days ago.</param>
    /// <param name="searchOption">
    /// <see cref="SearchOption.AllDirectories"/> to include subdirectories (where a recursive compress
    /// without <c>--output</c> writes its archives); otherwise only the top level.
    /// </param>
    /// <param name="baseName">
    /// When set, only archives whose name starts with <c>&lt;baseName&gt;-</c> are candidates (a bundle
    /// knows the base name of the archives it writes); otherwise every logc-named archive is.
    /// </param>
    /// <returns>The number of archives deleted.</returns>
    public int DeleteOldArchives(string directory, int olderThanDays, SearchOption searchOption = SearchOption.TopDirectoryOnly, string? baseName = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directory);
        ArgumentOutOfRangeException.ThrowIfLessThan(olderThanDays, 1);

        if (!_fileSystem.DirectoryExists(directory))
        {
            _logger.LogWarning("Retention directory does not exist: {Directory}", directory);
            return 0;
        }

        var today = _timeProvider.GetLocalNow().Date;
        var threshold = today.AddDays(-olderThanDays);
        var deleted = 0;

        foreach (var filePath in _fileSystem.EnumerateFiles(directory, "*", searchOption))
        {
            var fileInfo = _fileSystem.GetFileInfo(filePath);

            if (!IsRetentionCandidate(fileInfo.Name, baseName))
            {
                continue;
            }

            if (fileInfo.LastWriteTime >= threshold)
            {
                continue;
            }

            _logger.LogInformation
            (
                "Deleting old archive: {Path} (last modified: {Modified}, age: {Age} days)",
                fileInfo.FullName,
                fileInfo.LastWriteTime,
                (today - fileInfo.LastWriteTime.Date).Days
            );

            _fileSystem.DeleteFile(fileInfo.FullName);
            deleted++;
        }

        _logger.LogInformation("Retention cleanup: deleted {Count} old archive(s) from {Directory}", deleted, directory);
        return deleted;
    }



    internal static bool IsRetentionCandidate(string fileName, string? baseName)
    {
        return LogcArchiveName.IsMatch(fileName)
            && (baseName is null || fileName.StartsWith(baseName + "-", StringComparison.OrdinalIgnoreCase));
    }



    internal static bool IsArchiveFile(string fileName)
    {
        return ArchiveExtensions.Any(ext => fileName.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
    }
}
