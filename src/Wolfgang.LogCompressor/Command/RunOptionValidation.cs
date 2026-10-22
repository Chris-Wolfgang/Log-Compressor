using Wolfgang.LogCompressor.Model;

namespace Wolfgang.LogCompressor.Command;

/// <summary>
/// Validation of the options every run command shares (<c>compress</c>, <c>bundle</c> and
/// <c>decompress</c>): <c>--on-error</c>, <c>--report</c> and <c>--report-path</c>. One
/// implementation, so the three commands accept the same values (case-insensitively) and print
/// the same errors.
/// </summary>
internal static class RunOptionValidation
{
    /// <summary>
    /// Validates the error policy and report options.
    /// </summary>
    /// <param name="onError">The <c>--on-error</c> value.</param>
    /// <param name="report">The <c>--report</c> value, if any.</param>
    /// <param name="reportPath">The <c>--report-path</c> value, if any.</param>
    /// <returns>The error message for the first invalid option, or <see langword="null"/> when all are valid.</returns>
    internal static string? Validate(string onError, string? report, string? reportPath)
    {
        if (!ErrorPolicy.TryParse(onError, out _))
        {
            return $"Error: Unsupported error policy: '{onError}'. Supported: skip, fail, retry:N (1-100)";
        }

        if (report != null && !IsValidReportFormat(report))
        {
            return $"Error: Unsupported report format: '{report}'. Supported: json, csv";
        }

        if (reportPath != null && report == null)
        {
            return "Error: --report-path requires --report to be specified.";
        }

        return null;
    }



    private static bool IsValidReportFormat(string value)
    {
        return value.ToLowerInvariant() is "json" or "csv";
    }
}
