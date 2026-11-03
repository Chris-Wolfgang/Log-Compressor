using System.Globalization;
using System.Text;
using System.Text.Json;
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
        // Written with Utf8JsonWriter rather than a serializer: no reflection (trim-safe) and no
        // source generator either. The .NET generators emit their code with the build machine's
        // newline, which made logc.dll differ between Windows and Linux builds.
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true }))
        {
            writer.WriteStartObject();
            writer.WriteString("timestamp", _timeProvider.GetLocalNow());
            // Total hours, not a 24h-wrapping clock face — a >24h run must
            // not silently drop its days.
            writer.WriteString("duration", string.Create(CultureInfo.InvariantCulture, $"{(int)duration.TotalHours:D2}:{duration.Minutes:D2}:{duration.Seconds:D2}"));
            writer.WriteNumber("totalFiles", results.Count);
            writer.WriteNumber("succeededFiles", results.Count(r => r.Success));
            writer.WriteNumber("failedFiles", results.Count(r => !r.Success));
            writer.WriteNumber("originalSizeBytes", results.Sum(r => r.OriginalSize));
            writer.WriteNumber("compressedSizeBytes", results.Where(r => r.Success).Sum(r => r.CompressedSize));

            writer.WriteStartArray("files");
            foreach (var result in results)
            {
                writer.WriteStartObject();
                writer.WriteString("sourcePath", result.SourcePath);
                writer.WriteString("outputPath", result.OutputPath);
                writer.WriteNumber("originalSize", result.OriginalSize);
                writer.WriteNumber("compressedSize", result.CompressedSize);
                writer.WriteBoolean("success", result.Success);
                WriteErrorMessage(writer, result);
                writer.WriteEndObject();
            }

            writer.WriteEndArray();

            writer.WriteStartArray("errors");
            foreach (var result in results.Where(r => !r.Success))
            {
                writer.WriteStartObject();
                writer.WriteString("sourcePath", result.SourcePath);
                WriteErrorMessage(writer, result);
                writer.WriteEndObject();
            }

            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        return Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Length);
    }



    // A missing message is left out of the report rather than written as null.
    private static void WriteErrorMessage(Utf8JsonWriter writer, CompressionResult result)
    {
        if (result.ErrorMessage is not null)
        {
            writer.WriteString("errorMessage", result.ErrorMessage);
        }
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

        // Spreadsheet formula-injection guard: a leading =, +, - or @ (even
        // behind whitespace) is executed as a formula when the CSV opens in
        // Excel/Sheets.
        var significant = escaped.AsSpan().TrimStart();
        return significant.Length > 0 && significant[0] is '=' or '+' or '-' or '@'
            ? "'" + escaped
            : escaped;
    }
}
