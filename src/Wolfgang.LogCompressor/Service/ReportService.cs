using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Wolfgang.LogCompressor.Model;

namespace Wolfgang.LogCompressor.Service;

/// <summary>
/// Generates summary reports for compression operations.
/// </summary>
internal sealed class ReportService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };



    /// <summary>
    /// Writes a summary report to the specified path.
    /// </summary>
    /// <param name="results">The compression results.</param>
    /// <param name="format">The report format ("json" or "csv").</param>
    /// <param name="outputPath">The output path for the report file.</param>
    /// <param name="duration">The total operation duration.</param>
    public Task WriteReportAsync
    (
        IReadOnlyList<CompressionResult> results,
        string format,
        string outputPath,
        TimeSpan duration
    )
    {
        ArgumentNullException.ThrowIfNull(results);
        ArgumentException.ThrowIfNullOrWhiteSpace(format);
        ArgumentException.ThrowIfNullOrWhiteSpace(outputPath);

        var content = format.ToLowerInvariant() switch
        {
            "json" => GenerateJson(results, duration),
            "csv" => GenerateCsv(results),
            _ => throw new ArgumentException($"Unsupported report format: {format}", nameof(format))
        };

        return File.WriteAllTextAsync(outputPath, content);
    }



    private static string GenerateJson(IReadOnlyList<CompressionResult> results, TimeSpan duration)
    {
        var report = new
        {
            Timestamp = DateTimeOffset.Now,
            Duration = duration.ToString(@"hh\:mm\:ss"),
            TotalFiles = results.Count,
            SucceededFiles = results.Count(r => r.Success),
            FailedFiles = results.Count(r => !r.Success),
            OriginalSizeBytes = results.Sum(r => r.OriginalSize),
            CompressedSizeBytes = results.Where(r => r.Success).Sum(r => r.CompressedSize),
            Files = results.Select(r => new
            {
                r.SourcePath,
                r.OutputPath,
                r.OriginalSize,
                r.CompressedSize,
                r.Success,
                r.ErrorMessage
            }),
            Errors = results.Where(r => !r.Success).Select(r => new { r.SourcePath, r.ErrorMessage })
        };

        return JsonSerializer.Serialize(report, JsonOptions);
    }



    private static string GenerateCsv(IReadOnlyList<CompressionResult> results)
    {
        var sb = new StringBuilder();
        sb.AppendLine("SourcePath,OutputPath,OriginalSize,CompressedSize,Success,ErrorMessage");

        foreach (var r in results)
        {
            sb.AppendLine
            (
                $"\"{EscapeCsv(r.SourcePath)}\",\"{EscapeCsv(r.OutputPath)}\",{r.OriginalSize},{r.CompressedSize},{r.Success},\"{EscapeCsv(r.ErrorMessage ?? string.Empty)}\""
            );
        }

        return sb.ToString();
    }



    private static string EscapeCsv(string value)
    {
        return value.Replace("\"", "\"\"", StringComparison.Ordinal);
    }
}
