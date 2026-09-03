using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace Wolfgang.LogCompressor.Service;

/// <summary>
/// Provides single-instance locking for a directory using a lock file.
/// </summary>
/// <remarks>
/// Mutual exclusion comes entirely from holding the lock file open with
/// <see cref="FileShare.None"/> (mandatory sharing on Windows, advisory flock
/// on Unix — both enforced between .NET processes, which is the only
/// contention that matters for logc-vs-logc). The file is opened with
/// <see cref="FileMode.OpenOrCreate"/>, so a leftover file from a crashed run
/// never blocks: the crashed holder's handle is gone and the open simply
/// succeeds. This is deliberately one atomic step — the previous
/// check-stale / delete / create sequence had a window where a contender
/// could unlink a freshly created LIVE lock on Unix and acquire alongside its
/// owner (issue #172). The file's PID/Started content is diagnostic only and
/// plays no part in the locking protocol.
/// </remarks>
[ExcludeFromCodeCoverage]
internal sealed class ProcessLock : IDisposable
{
    /// <summary>
    /// Name of the lock file created in the locked directory. Compression and
    /// bundling exclude this file from enumeration so a run never tries to
    /// compress (and delete) its own live lock.
    /// </summary>
    internal const string LockFileName = ".logc.lock";

    private readonly string _lockFilePath;
    private readonly ILogger _logger;
    private FileStream? _lockStream;



    /// <summary>
    /// Initializes a new instance of the <see cref="ProcessLock"/> class.
    /// </summary>
    /// <param name="directory">The directory to lock.</param>
    /// <param name="logger">The logger.</param>
    public ProcessLock(string directory, ILogger logger)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directory);
        ArgumentNullException.ThrowIfNull(logger);

        _lockFilePath = Path.Combine(directory, LockFileName);
        _logger = logger;
    }



    /// <summary>
    /// Determines whether the given path is a logc lock file.
    /// </summary>
    /// <param name="path">The file path to test.</param>
    /// <returns><see langword="true"/> if the file name is the lock file name; otherwise, <see langword="false"/>.</returns>
    internal static bool IsLockFile(string path)
    {
        return string.Equals(Path.GetFileName(path), LockFileName, StringComparison.OrdinalIgnoreCase);
    }



    /// <summary>
    /// Attempts to acquire the lock.
    /// </summary>
    /// <returns><see langword="true"/> if the lock was acquired; otherwise, <see langword="false"/>.</returns>
    public bool TryAcquire()
    {
        try
        {
            // OpenOrCreate + FileShare.None IS the lock: it atomically claims
            // the file whether or not it already exists. A live holder makes
            // this throw IOException; a crashed holder's leftover file opens
            // fine. No existence pre-check, no stale-PID takeover, no delete
            // — nothing to race (#172).
            _lockStream = new FileStream
            (
                _lockFilePath,
                FileMode.OpenOrCreate,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 4096,
                FileOptions.None
            );

            // Refresh the diagnostic content for humans inspecting the
            // directory. SetLength(0) discards whatever a previous (crashed)
            // holder wrote.
            _lockStream.SetLength(0);

            using (var writer = new StreamWriter(_lockStream, leaveOpen: true))
            {
                writer.WriteLine($"PID={Environment.ProcessId}");
                writer.WriteLine($"Started={DateTimeOffset.Now:O}");
                writer.Flush();
            }

            // StreamWriter.Flush only pushes the writer buffer into the
            // FileStream buffer; push the bytes to disk so anything reading
            // the file for diagnostics sees the PID rather than an empty file.
            _lockStream.Flush(flushToDisk: true);

            _logger.LogDebug("Lock acquired: {Path}", _lockFilePath);
            return true;
        }
        catch (IOException ex)
        {
            _logger.LogWarning
            (
                ex,
                "Another instance is already processing this directory. Lock file: {Path}",
                _lockFilePath
            );
            return false;
        }
    }



    /// <inheritdoc />
    public void Dispose()
    {
        if (_lockStream == null)
        {
            return;
        }

        // Release order is platform-specific to keep the one-live-holder
        // invariant airtight (#172):
        //
        // - Unix: unlink FIRST, while still holding the lock. A contender
        //   opening after the unlink creates a fresh file and legitimately
        //   acquires; deleting after close would instead race — unlink
        //   succeeds on open files there, so a delayed delete could remove
        //   a successor's live lock.
        // - Windows: deleting an open file is a sharing violation (even
        //   against our own handle), so delete AFTER close. Safe there: if a
        //   successor acquires between our close and delete, the delete
        //   fails with a sharing violation and is swallowed as best-effort.
        if (OperatingSystem.IsWindows())
        {
            _lockStream.Dispose();
            TryDeleteLockFile();
        }
        else
        {
            TryDeleteLockFile();
            _lockStream.Dispose();
        }

        _lockStream = null;
        _logger.LogDebug("Lock released: {Path}", _lockFilePath);
    }



    private void TryDeleteLockFile()
    {
        // Best-effort: a lingering file (e.g. held by a successor on
        // Windows, or after an unlikely IO error) never blocks a future
        // acquire under the OpenOrCreate protocol.
        try
        {
            File.Delete(_lockFilePath);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            _logger.LogDebug(ex, "Could not delete lock file {Path}", _lockFilePath);
        }
    }
}
