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

    private readonly IFileSystem _fileSystem;
    private readonly ILogger<RetentionService> _logger;



    /// <summary>
    /// Initializes a new instance of the <see cref="RetentionService"/> class.
    /// </summary>
    /// <param name="fileSystem">The file system abstraction.</param>
    /// <param name="logger">The logger.</param>
    public RetentionService(IFileSystem fileSystem, ILogger<RetentionService> logger)
    {
        ArgumentNullException.ThrowIfNull(fileSystem);
        ArgumentNullException.ThrowIfNull(logger);

        _fileSystem = fileSystem;
        _logger = logger;
    }



    /// <summary>
    /// Deletes compressed archives older than the specified number of days.
    /// </summary>
    /// <param name="directory">The directory to scan.</param>
    /// <param name="olderThanDays">Delete archives last modified more than this many days ago.</param>
    /// <returns>The number of archives deleted.</returns>
    public int DeleteOldArchives(string directory, int olderThanDays)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directory);
        ArgumentOutOfRangeException.ThrowIfLessThan(olderThanDays, 1);

        if (!_fileSystem.DirectoryExists(directory))
        {
            _logger.LogWarning("Retention directory does not exist: {Directory}", directory);
            return 0;
        }

        var threshold = DateTime.Today.AddDays(-olderThanDays);
        var deleted = 0;

        foreach (var filePath in _fileSystem.EnumerateFiles(directory, "*", SearchOption.TopDirectoryOnly))
        {
            var fileInfo = _fileSystem.GetFileInfo(filePath);

            if (!IsArchiveFile(fileInfo.Name))
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
                (DateTime.Today - fileInfo.LastWriteTime.Date).Days
            );

            _fileSystem.DeleteFile(fileInfo.FullName);
            deleted++;
        }

        _logger.LogInformation("Retention cleanup: deleted {Count} old archive(s) from {Directory}", deleted, directory);
        return deleted;
    }



    internal static bool IsArchiveFile(string fileName)
    {
        return ArchiveExtensions.Any(ext => fileName.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
    }
}
