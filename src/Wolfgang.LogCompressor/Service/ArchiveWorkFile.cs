using Microsoft.Extensions.Logging;
using Wolfgang.LogCompressor.Abstraction;

namespace Wolfgang.LogCompressor.Service;

/// <summary>
/// Names for the in-progress and failed copies of an archive. An archive is written under
/// <see cref="PartialSuffix"/> and moved to its final name only after it has been written and,
/// when requested, verified, so a final-named archive is always a finished one. An archive that
/// fails verification is kept under <see cref="FailedSuffix"/> for inspection.
/// </summary>
internal static class ArchiveWorkFile
{
    /// <summary>
    /// Suffix of an archive that is still being written. A leftover after a crash is deleted by the
    /// next attempt at the same output path.
    /// </summary>
    internal const string PartialSuffix = ".partial";

    /// <summary>
    /// Suffix of an archive that failed verification, kept for inspection. It never blocks a later run.
    /// </summary>
    internal const string FailedSuffix = ".failed";



    /// <summary>
    /// Returns the path an archive is written to before it is moved to <paramref name="outputPath"/>.
    /// </summary>
    /// <param name="outputPath">The final archive path.</param>
    /// <returns><paramref name="outputPath"/> with <see cref="PartialSuffix"/> appended.</returns>
    internal static string GetPartialPath(string outputPath) => outputPath + PartialSuffix;



    /// <summary>
    /// Returns the path an archive that failed verification is kept under.
    /// </summary>
    /// <param name="outputPath">The final archive path.</param>
    /// <returns><paramref name="outputPath"/> with <see cref="FailedSuffix"/> appended.</returns>
    internal static string GetFailedPath(string outputPath) => outputPath + FailedSuffix;



    /// <summary>
    /// Determines whether <paramref name="path"/> is a partial or failed archive written by logc.
    /// Such files are never compressed, bundled or treated as finished archives.
    /// </summary>
    /// <param name="path">A file name or path.</param>
    /// <returns><see langword="true"/> for a partial or failed archive; otherwise <see langword="false"/>.</returns>
    internal static bool IsWorkFile(string path)
    {
        return path.EndsWith(PartialSuffix, StringComparison.OrdinalIgnoreCase)
            || path.EndsWith(FailedSuffix, StringComparison.OrdinalIgnoreCase);
    }



    /// <summary>
    /// Deletes a partial archive after a failed or canceled attempt. A failure to delete it is
    /// logged and swallowed so it never hides the error that ended the attempt; the next attempt at
    /// the same output path deletes the leftover.
    /// </summary>
    /// <param name="fileSystem">The file system abstraction.</param>
    /// <param name="logger">The logger for a failed delete.</param>
    /// <param name="partialPath">The partial archive path.</param>
    internal static void DeleteQuietly(IFileSystem fileSystem, ILogger logger, string partialPath)
    {
        try
        {
            if (fileSystem.FileExists(partialPath))
            {
                fileSystem.DeleteFile(partialPath);
            }
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            logger.LogWarning(ex, "Could not delete partial archive {Partial}: {Message}", partialPath, ex.Message);
        }
    }
}
