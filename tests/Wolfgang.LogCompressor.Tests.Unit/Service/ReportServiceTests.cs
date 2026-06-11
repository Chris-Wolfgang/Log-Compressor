using System.Text.Json;
using Wolfgang.LogCompressor.Model;
using Wolfgang.LogCompressor.Service;

namespace Wolfgang.LogCompressor.Tests.Unit.Service;

public sealed class ReportServiceTests : IDisposable
{
    private readonly ReportService _sut = new();
    private readonly string _tempDir;



    public ReportServiceTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "ReportServiceTests_" + Guid.NewGuid());
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
    public async Task WriteReportAsync_when_jsonFormat_expected_validJsonFile()
    {
        var results = CreateSampleResults();
        var outputPath = Path.Combine(_tempDir, "report.json");

        await _sut.WriteReportAsync(results, "json", outputPath, TimeSpan.FromSeconds(42));

        Assert.True(File.Exists(outputPath));

        var json = await File.ReadAllTextAsync(outputPath);
        var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        Assert.Equal(2, root.GetProperty("totalFiles").GetInt32());
        Assert.Equal(1, root.GetProperty("succeededFiles").GetInt32());
        Assert.Equal(1, root.GetProperty("failedFiles").GetInt32());
        Assert.Equal("00:00:42", root.GetProperty("duration").GetString());
    }



    [Fact]
    public async Task WriteReportAsync_when_csvFormat_expected_validCsvFile()
    {
        var results = CreateSampleResults();
        var outputPath = Path.Combine(_tempDir, "report.csv");

        await _sut.WriteReportAsync(results, "csv", outputPath, TimeSpan.FromSeconds(10));

        Assert.True(File.Exists(outputPath));

        var lines = await File.ReadAllLinesAsync(outputPath);

        Assert.True(lines.Length >= 3);
        Assert.Equal
        (
            "SourcePath,OutputPath,OriginalSize,CompressedSize,Success,ErrorMessage",
            lines[0]
        );
        Assert.Contains("a.log", lines[1]);
        Assert.Contains("b.log", lines[2]);
    }



    [Fact]
    public async Task WriteReportAsync_when_invalidFormat_expected_throwsArgumentException()
    {
        var results = CreateSampleResults();
        var outputPath = Path.Combine(_tempDir, "report.txt");

        await Assert.ThrowsAsync<ArgumentException>
        (
            () => _sut.WriteReportAsync(results, "xml", outputPath, TimeSpan.Zero)
        );
    }



    [Fact]
    public async Task WriteReportAsync_when_nullResults_expected_throwsArgumentNullException()
    {
        var outputPath = Path.Combine(_tempDir, "report.json");

        await Assert.ThrowsAsync<ArgumentNullException>
        (
            () => _sut.WriteReportAsync(null!, "json", outputPath, TimeSpan.Zero)
        );
    }



    [Fact]
    public async Task WriteReportAsync_when_nullFormat_expected_throwsArgumentNullException()
    {
        var results = CreateSampleResults();
        var outputPath = Path.Combine(_tempDir, "report.json");

        await Assert.ThrowsAsync<ArgumentNullException>
        (
            () => _sut.WriteReportAsync(results, null!, outputPath, TimeSpan.Zero)
        );
    }



    [Fact]
    public async Task WriteReportAsync_when_nullOutputPath_expected_throwsArgumentNullException()
    {
        var results = CreateSampleResults();

        await Assert.ThrowsAsync<ArgumentNullException>
        (
            () => _sut.WriteReportAsync(results, "json", null!, TimeSpan.Zero)
        );
    }



    [Fact]
    public async Task WriteReportAsync_when_directoryNotExists_expected_createsDirectory()
    {
        var results = CreateSampleResults();
        var nestedDir = Path.Combine(_tempDir, "sub", "dir");
        var outputPath = Path.Combine(nestedDir, "report.json");

        await _sut.WriteReportAsync(results, "json", outputPath, TimeSpan.FromSeconds(1));

        Assert.True(Directory.Exists(nestedDir));
        Assert.True(File.Exists(outputPath));
    }



    [Fact]
    public async Task WriteReportAsync_when_emptyResults_expected_validJsonWithZeroCounts()
    {
        var results = new List<CompressionResult>();
        var outputPath = Path.Combine(_tempDir, "empty.json");

        await _sut.WriteReportAsync(results, "json", outputPath, TimeSpan.Zero);

        var json = await File.ReadAllTextAsync(outputPath);
        var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        Assert.Equal(0, root.GetProperty("totalFiles").GetInt32());
        Assert.Equal(0, root.GetProperty("succeededFiles").GetInt32());
    }



    [Fact]
    public async Task WriteReportAsync_when_csvWithQuotesInPath_expected_quotesEscaped()
    {
        var results = new List<CompressionResult>
        {
            new()
            {
                SourcePath = "path\"with\"quotes.log",
                OutputPath = "out.zip",
                OriginalSize = 100,
                CompressedSize = 50,
                Success = true
            }
        };
        var outputPath = Path.Combine(_tempDir, "escaped.csv");

        await _sut.WriteReportAsync(results, "csv", outputPath, TimeSpan.Zero);

        var content = await File.ReadAllTextAsync(outputPath);

        Assert.Contains("\"\"", content);
    }



    private static List<CompressionResult> CreateSampleResults()
    {
        return
        [
            new CompressionResult
            {
                SourcePath = "a.log",
                OutputPath = "a.zip",
                OriginalSize = 1000,
                CompressedSize = 200,
                Success = true
            },
            new CompressionResult
            {
                SourcePath = "b.log",
                OutputPath = "b.zip",
                OriginalSize = 500,
                CompressedSize = 0,
                Success = false,
                ErrorMessage = "disk full"
            }
        ];
    }
}
