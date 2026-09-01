namespace Wolfgang.LogCompressor.Model;

/// <summary>
/// How a run reacts when one item in the batch fails.
/// </summary>
internal enum OnErrorMode
{
    /// <summary>Record the failure and continue with the remaining items (the default).</summary>
    Skip,

    /// <summary>Stop the run at the first failure.</summary>
    Fail
}
