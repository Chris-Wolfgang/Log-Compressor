namespace Wolfgang.LogCompressor;

internal static class ExitCode
{
    /// <summary>
    /// Every item was processed successfully (or there was nothing to do).
    /// </summary>
    internal const int Success = 0;



    /// <summary>
    /// The command line or response file was invalid; nothing was processed.
    /// </summary>
    internal const int InvalidArguments = 1;



    /// <summary>
    /// Another logc instance holds the lock on the source directory; nothing was processed.
    /// </summary>
    internal const int AlreadyRunning = 2;



    /// <summary>
    /// The run finished, but one or more items were skipped after failing
    /// (the --on-error skip / exhausted-retry outcome). Distinct from
    /// <see cref="ApplicationError"/> so schedulers can tell a partially
    /// degraded run from a broken one.
    /// </summary>
    internal const int CompletedWithSkips = 3;



    /// <summary>
    /// The run was canceled (Ctrl+C / host shutdown) before completing.
    /// Deliberate cancellation is not an application error.
    /// </summary>
    internal const int Canceled = 4;



    /// <summary>
    /// An exception escaped the command (for example an unrecognized option rejected by the
    /// command-line parser).
    /// </summary>
    internal const int UnhandledException = 10;



    /// <summary>
    /// The run failed: <c>--on-error fail</c> stopped at the first failure, or a fatal error ended it.
    /// </summary>
    internal const int ApplicationError = 11;
}
