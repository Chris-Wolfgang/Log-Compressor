using Wolfgang.LogCompressor.Model;
using Wolfgang.LogCompressor.Service;

namespace Wolfgang.LogCompressor.Tests.Unit.Service;

/// <summary>
/// Pins the exact text of the JSON report for the inputs where a JSON writer is easiest to get
/// wrong: empty collections, null and empty messages, and characters the encoder escapes. The
/// expected text is what the serializer-based writer this one replaced produced, byte for byte.
/// </summary>
public sealed class ReportServiceJsonFormatTests : IDisposable
{
    private static readonly DateTimeOffset Now = new DateTimeOffset(2026, 10, 17, 21, 59, 6, 724, TimeSpan.Zero).AddTicks(9381);

    private readonly string _tempDir = Path.Combine(Path.GetTempPath(), "ReportServiceJsonFormatTests_" + Guid.NewGuid().ToString("N"));



    public ReportServiceJsonFormatTests()
    {
        Directory.CreateDirectory(_tempDir);
    }



    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
        {
            Directory.Delete(_tempDir, recursive: true);
        }
    }



    [Fact]
    public async Task WriteReportAsync_when_noResults_expected_emptyArraysAndZeroCounts()
    {
        var text = await WriteAsync([]);

        Assert.Equal
        (
            """
            {
              "timestamp": "2026-10-17T21:59:06.7249381+00:00",
              "duration": "00:00:42",
              "totalFiles": 0,
              "succeededFiles": 0,
              "failedFiles": 0,
              "originalSizeBytes": 0,
              "compressedSizeBytes": 0,
              "files": [],
              "errors": []
            }
            """,
            text
        );
    }



    [Fact]
    public async Task WriteReportAsync_when_stringsNeedEscaping_expected_encoderEscapesAndEmptyMessageKept()
    {
        var text = await WriteAsync
        (
            [
                new CompressionResult
                {
                    SourcePath = "C:\\logs\\\"quoted\" <tag> & ampersand 'single'.log",
                    OutputPath = "/caf\u00e9/\u65e5\u672c\u8a9e/\U0001F600.zip",
                    OriginalSize = 1,
                    CompressedSize = 1,
                    Success = false,
                    ErrorMessage = "line1\nline2\ttab\r\nback\\slash \u0001 control \u2028 sep"
                },
                new CompressionResult
                {
                    SourcePath = "",
                    OutputPath = "x",
                    OriginalSize = 0,
                    CompressedSize = 0,
                    Success = false,
                    ErrorMessage = ""
                }
            ]
        );

        Assert.Equal
        (
            """
            {
              "timestamp": "2026-10-17T21:59:06.7249381+00:00",
              "duration": "00:00:42",
              "totalFiles": 2,
              "succeededFiles": 0,
              "failedFiles": 2,
              "originalSizeBytes": 1,
              "compressedSizeBytes": 0,
              "files": [
                {
                  "sourcePath": "C:\\logs\\\u0022quoted\u0022 \u003Ctag\u003E \u0026 ampersand \u0027single\u0027.log",
                  "outputPath": "/caf\u00E9/\u65E5\u672C\u8A9E/\uD83D\uDE00.zip",
                  "originalSize": 1,
                  "compressedSize": 1,
                  "success": false,
                  "errorMessage": "line1\nline2\ttab\r\nback\\slash \u0001 control \u2028 sep"
                },
                {
                  "sourcePath": "",
                  "outputPath": "x",
                  "originalSize": 0,
                  "compressedSize": 0,
                  "success": false,
                  "errorMessage": ""
                }
              ],
              "errors": [
                {
                  "sourcePath": "C:\\logs\\\u0022quoted\u0022 \u003Ctag\u003E \u0026 ampersand \u0027single\u0027.log",
                  "errorMessage": "line1\nline2\ttab\r\nback\\slash \u0001 control \u2028 sep"
                },
                {
                  "sourcePath": "",
                  "errorMessage": ""
                }
              ]
            }
            """,
            text
        );
    }



    [Fact]
    public async Task WriteReportAsync_when_failureHasNoMessage_expected_errorMessageKeyOmitted()
    {
        var text = await WriteAsync
        (
            [
                new CompressionResult { SourcePath = "/a.log", OutputPath = "", OriginalSize = 10, CompressedSize = 0, Success = false, ErrorMessage = null }
            ]
        );

        Assert.DoesNotContain("errorMessage", text, StringComparison.Ordinal);
        Assert.Contains("\"success\": false", text, StringComparison.Ordinal);
    }



    private async Task<string> WriteAsync(IReadOnlyList<CompressionResult> results)
    {
        var path = Path.Combine(_tempDir, "report.json");
        var sut = new ReportService(new FileSystemWrapper(), new FixedTimeProvider(Now));

        await sut.WriteReportAsync(results, "json", path, TimeSpan.FromSeconds(42));

        // The report uses the platform newline, as it always has; compare on LF.
        return (await File.ReadAllTextAsync(path)).Replace("\r\n", "\n", StringComparison.Ordinal);
    }



    private sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => now;



        public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;
    }
}
