namespace Wolfgang.LogCompressor.Service;

/// <summary>
/// One processed file in the JSON summary report.
/// </summary>
internal sealed record ReportFileEntry
(
    string SourcePath,
    string OutputPath,
    long OriginalSize,
    long CompressedSize,
    bool Success,
    string? ErrorMessage
);
