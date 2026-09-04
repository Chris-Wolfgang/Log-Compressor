using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Wolfgang.LogCompressor.Abstraction;
using Wolfgang.LogCompressor.Model;

namespace Wolfgang.LogCompressor.Service;

/// <summary>
/// Generates summary reports for compression operations.
/// </summary>
internal sealed class ReportService
{
    private readonly IFileSystem _fileSystem;
    private readonly TimeProvider _timeProvider;



    /// <summary>
    /// Initializes a new instance of the <see cref="ReportService"/> class.
    /// </summary>
    /// <param name="fileSystem">The file system abstraction.</param>
    /// <param name="timeProvider">The time source for the report timestamp.</param>
    public ReportService(IFileSystem fileSystem, TimeProvider timeProvider)
    {
        ArgumentNullException.ThrowIfNull(fileSystem);
        ArgumentNullException.ThrowIfNull(timeProvider);

        _fileSystem = fileSystem;
        _timeProvider = timeProvider;
    }



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
    /// <exception cref="ArgumentException">Thrown when <paramref name="format"/> is not supported.</exception>
    public async Task WriteReportAsync
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

        var directory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(directory) && !_fileSystem.DirectoryExists(directory))
        {
            _fileSystem.CreateDirectory(directory);
        }

        var stream = _fileSystem.CreateWrite(outputPath);
        await using (stream.ConfigureAwait(false))
        {
            var writer = new StreamWriter(stream);
            await using (writer.ConfigureAwait(false))
            {
                await writer.WriteAsync(content).ConfigureAwait(false);
            }
        }
    }



    private string GenerateJson(IReadOnlyList<CompressionResult> results, TimeSpan duration)
    {
        var report = new
        {
            Timestamp = _timeProvider.GetLocalNow(),
            // Total hours, not a 24h-wrapping clock face — a >24h run must
            // not silently drop its days.
            Duration = string.Create(CultureInfo.InvariantCulture, $"{(int)duration.TotalHours:D2}:{duration.Minutes:D2}:{duration.Seconds:D2}"),
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
        var escaped = value.Replace("\"", "\"\"", StringComparison.Ordinal);

        // Spreadsheet formula-injection guard: a leading =, +, - or @ is
        // executed as a formula when the CSV opens in Excel/Sheets.
        return escaped.Length > 0 && escaped[0] is '=' or '+' or '-' or '@'
            ? "'" + escaped
            : escaped;
    }
}
