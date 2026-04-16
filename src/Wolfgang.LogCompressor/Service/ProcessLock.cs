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

            _lockStream = new FileStream
            (
                _lockFilePath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 4096,
                FileOptions.DeleteOnClose
            );

            using var writer = new StreamWriter(_lockStream, leaveOpen: true);
            writer.WriteLine($"PID={Environment.ProcessId}");
            writer.WriteLine($"Started={DateTimeOffset.Now:O}");
            writer.Flush();

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
        catch
        {
            return true;
        }
    }
}
