namespace Wolfgang.LogCompressor.Service;

/// <summary>
/// The shape of the JSON summary report. Property order is the order the keys appear in the file.
/// </summary>
/// <remarks>
/// Named types (not anonymous ones) so the serializer can be source-generated: reflection-based
/// <c>System.Text.Json</c> is switched off in trimmed builds.
/// </remarks>
internal sealed record ReportDocument
(
    DateTimeOffset Timestamp,
    string Duration,
    int TotalFiles,
    int SucceededFiles,
    int FailedFiles,
    long OriginalSizeBytes,
    long CompressedSizeBytes,
    IReadOnlyList<ReportFileEntry> Files,
    IReadOnlyList<ReportErrorEntry> Errors
);
