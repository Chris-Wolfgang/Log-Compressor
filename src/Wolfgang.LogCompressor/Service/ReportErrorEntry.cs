namespace Wolfgang.LogCompressor.Service;

/// <summary>
/// One failure in the JSON summary report.
/// </summary>
internal sealed record ReportErrorEntry
(
    string SourcePath,
    string? ErrorMessage
);
