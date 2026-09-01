namespace Wolfgang.LogCompressor.Model;

/// <summary>
/// Which timestamp is embedded in generated archive names.
/// </summary>
internal enum TimestampSource
{
    /// <summary>
    /// The source file's last-modified time (the default) — archive names are
    /// stable across re-runs over unchanged sources.
    /// </summary>
    Modified,

    /// <summary>
    /// The time of compression — archive names record when the job ran.
    /// </summary>
    Compressed
}
