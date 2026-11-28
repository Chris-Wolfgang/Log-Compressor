using McMaster.Extensions.CommandLineUtils;
using Wolfgang.LogCompressor.Service;

namespace Wolfgang.LogCompressor.Command;

/// <summary>
/// Reports a failed <see cref="ProcessLock.TryAcquire"/> on the console with the exit code that
/// matches its cause, so only a lock held by another instance exits with
/// <see cref="ExitCode.AlreadyRunning"/> (#447).
/// </summary>
internal static class LockFailureConsole
{
    /// <summary>
    /// Writes the error for <paramref name="processLock"/>'s failure and returns the exit code.
    /// </summary>
    /// <param name="console">The console to write the error to.</param>
    /// <param name="processLock">The lock whose <see cref="ProcessLock.TryAcquire"/> returned <see langword="false"/>.</param>
    /// <param name="sourcePath">The source path of the run, named in the not-found message.</param>
    /// <returns>The exit code for the failure.</returns>
    internal static async Task<int> ReportAsync(IConsole console, ProcessLock processLock, string sourcePath)
    {
        switch (processLock.Failure)
        {
            case ProcessLockFailure.DirectoryNotFound:
                await console.Error.WriteLineAsync($"Error: Source path not found: {sourcePath}").ConfigureAwait(false);
                return ExitCode.ApplicationError;

            case ProcessLockFailure.CannotCreate:
                await console.Error.WriteLineAsync($"Error: Cannot create the lock file {processLock.LockFilePath}: {processLock.FailureMessage} Use --no-lock to skip locking.").ConfigureAwait(false);
                return ExitCode.ApplicationError;

            default:
                await console.Error.WriteLineAsync("Another instance is already processing this directory.").ConfigureAwait(false);
                return ExitCode.AlreadyRunning;
        }
    }
}
