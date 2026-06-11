using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace Wolfgang.LogCompressor.Service;

/// <summary>
/// Provides single-instance locking for a directory using a lock file.
/// </summary>
[ExcludeFromCodeCoverage]
internal sealed class ProcessLock : IDisposable
{
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

        _lockFilePath = Path.Combine(directory, ".logc.lock");
        _logger = logger;
    }



    /// <summary>
    /// Attempts to acquire the lock.
    /// </summary>
    /// <returns><see langword="true"/> if the lock was acquired; otherwise, <see langword="false"/>.</returns>
    public bool TryAcquire()
    {
        try
        {
            if (File.Exists(_lockFilePath))
            {
                if (IsLockStale())
                {
                    _logger.LogWarning("Stale lock file detected, taking over: {Path}", _lockFilePath);
                    File.Delete(_lockFilePath);
                }
            }

            // Do NOT use FileOptions.DeleteOnClose: on Unix .NET unlinks the file
            // immediately (open-then-unlink), so a second instance's File.Exists
            // check sees nothing and its CreateNew succeeds — defeating the lock.
            // The file is removed explicitly in Dispose instead.
            _lockStream = new FileStream
            (
                _lockFilePath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 4096,
                FileOptions.None
            );

            using (var writer = new StreamWriter(_lockStream, leaveOpen: true))
            {
                writer.WriteLine($"PID={Environment.ProcessId}");
                writer.WriteLine($"Started={DateTimeOffset.Now:O}");
                writer.Flush();
            }

            // StreamWriter.Flush only pushes the writer buffer into the
            // FileStream buffer; the bytes still sit in the FileStream's
            // 4096-byte buffer until something forces a disk write. Push
            // them out explicitly so a second instance's IsLockStale call
            // can read the PID rather than seeing an empty file and
            // mis-classifying the live lock as stale (the original
            // Linux-CI failure mode).
            _lockStream.Flush(flushToDisk: true);

            _logger.LogDebug("Lock acquired: {Path}", _lockFilePath);
            return true;
        }
        catch (IOException)
        {
            _logger.LogWarning
            (
                "Another instance is already processing this directory. Lock file: {Path}",
                _lockFilePath
            );
            return false;
        }
    }



    /// <inheritdoc />
    public void Dispose()
    {
        if (_lockStream != null)
        {
            _lockStream.Dispose();
            _lockStream = null;

            // Remove the lock file now that the stream is closed. Best-effort: a
            // lingering file (e.g. after a crash) is reclaimed by the next run's
            // stale-lock check.
            try
            {
                File.Delete(_lockFilePath);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                _logger.LogDebug(ex, "Could not delete lock file {Path}", _lockFilePath);
            }

            _logger.LogDebug("Lock released: {Path}", _lockFilePath);
        }
    }



    private bool IsLockStale()
    {
        try
        {
#pragma warning disable RS0030 // Sync read acceptable in lock-check context
            var content = File.ReadAllText(_lockFilePath);
#pragma warning restore RS0030
            var pidLine = content.Split('\n').FirstOrDefault(l => l.StartsWith("PID=", StringComparison.Ordinal));

            if (pidLine == null)
            {
                return true;
            }

            var pidStr = pidLine["PID=".Length..].Trim();
            if (!int.TryParse(pidStr, out var pid))
            {
                return true;
            }

            try
            {
                Process.GetProcessById(pid);
                return false;
            }
            catch (ArgumentException)
            {
                return true;
            }
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            // Can't read the lock file — most likely because another instance
            // is holding it open with FileShare.None. Treat as NOT stale (lock
            // is held, refuse to take over) rather than deleting a live lock
            // file out from under the existing owner.
            _logger.LogDebug(ex, "Could not read lock file {Path} to evaluate staleness; treating as held.", _lockFilePath);
            return false;
        }
    }
}
