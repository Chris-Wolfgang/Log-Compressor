namespace Wolfgang.LogCompressor;

internal static class ExitCode
{
    internal const int Success = 0;



    internal const int InvalidArguments = 1;



    internal const int AlreadyRunning = 2;



    /// <summary>
    /// The run finished, but one or more items were skipped after failing
    /// (the --on-error skip / exhausted-retry outcome). Distinct from
    /// <see cref="ApplicationError"/> so schedulers can tell a partially
    /// degraded run from a broken one.
    /// </summary>
    internal const int CompletedWithSkips = 3;



    internal const int UnhandledException = 10;



    internal const int ApplicationError = 11;
}
