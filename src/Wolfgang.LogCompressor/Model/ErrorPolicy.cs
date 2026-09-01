using System.Globalization;

namespace Wolfgang.LogCompressor.Model;

/// <summary>
/// Parsed <c>--on-error</c> policy: the mode plus an optional per-item retry
/// count applied before the mode takes effect.
/// </summary>
internal sealed record ErrorPolicy
{
    /// <summary>
    /// Gets the default policy: skip and continue, no retries — the behaviour
    /// every command had before <c>--on-error</c> existed.
    /// </summary>
    public static ErrorPolicy Default { get; } = new();



    /// <summary>
    /// Gets what happens after retries (if any) are exhausted.
    /// </summary>
    public OnErrorMode Mode { get; init; } = OnErrorMode.Skip;



    /// <summary>
    /// Gets how many times a failing item is retried before <see cref="Mode"/> applies.
    /// </summary>
    public int RetryCount { get; init; }



    /// <summary>
    /// Parses <c>skip</c>, <c>fail</c> or <c>retry:N</c> (1–100).
    /// </summary>
    /// <param name="value">The raw flag value.</param>
    /// <param name="policy">The parsed policy on success.</param>
    /// <returns><see langword="true"/> when the value is a recognized policy.</returns>
    public static bool TryParse(string value, out ErrorPolicy policy)
    {
        policy = Default;

        switch (value.ToLowerInvariant())
        {
            case "skip":
                return true;
            case "fail":
                policy = new ErrorPolicy { Mode = OnErrorMode.Fail };
                return true;
        }

        if (value.StartsWith("retry:", StringComparison.OrdinalIgnoreCase)
            && int.TryParse(value["retry:".Length..], NumberStyles.None, CultureInfo.InvariantCulture, out var count)
            && count is >= 1 and <= 100)
        {
            // Retries exhausted -> skip-and-continue; combine with fail via
            // a follow-up if anyone ever needs it.
            policy = new ErrorPolicy { RetryCount = count };
            return true;
        }

        return false;
    }
}
