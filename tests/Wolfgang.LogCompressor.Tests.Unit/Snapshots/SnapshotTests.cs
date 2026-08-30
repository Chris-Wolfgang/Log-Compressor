using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Wolfgang.LogCompressor.Command;
using Wolfgang.LogCompressor.Model;
using Wolfgang.LogCompressor.Service;

namespace Wolfgang.LogCompressor.Tests.Unit.Snapshots;

/// <summary>
/// Approval tests (Verify framework) for every text-shaped output the tool
/// produces: report files and generated .rsp configuration templates. The
/// committed .verified.txt snapshots catch accidental format drift that
/// targeted assertions miss; .received.txt files from local runs are
/// gitignored.
/// </summary>
public sealed class SnapshotTests : IDisposable
{
    private readonly TempDirectory _tempDir = new();



    private static VerifySettings Settings
    {
        get
        {
            var settings = new VerifySettings();
            settings.UseDirectory(".");
            // The JSON report embeds the wall-clock run time.
            settings.ScrubLinesContaining("\"timestamp\"");
            return settings;
        }
    }



    private static IReadOnlyList<CompressionResult> SampleResults() =>
    [
        new()
        {
            SourcePath = "/logs/app.log",
            OutputPath = "/logs/app-2026-01-05_09-30-00.zip",
            OriginalSize = 1234567,
            CompressedSize = 98765,
            Success = true
        },
        new()
        {
            SourcePath = "/logs/locked, \"quoted\".log",
            OutputPath = "/logs/locked-2026-01-06_10-00-00.zip",
            OriginalSize = 4096,
            CompressedSize = 0,
            Success = false,
            ErrorMessage = "Archive verification failed."
        }
    ];



    [Fact]
    public async Task WriteReportAsync_when_json_expected_stableShape()
    {
        var sut = new ReportService();
        var reportPath = Path.Combine(_tempDir.Path, "report.json");

        await sut.WriteReportAsync(SampleResults(), "json", reportPath, TimeSpan.FromMinutes(5));

        var content = await File.ReadAllTextAsync(reportPath);
        await Verify(content, Settings);
    }



    [Fact]
    public async Task WriteReportAsync_when_csv_expected_stableShape()
    {
        var sut = new ReportService();
        var reportPath = Path.Combine(_tempDir.Path, "report.csv");

        await sut.WriteReportAsync(SampleResults(), "csv", reportPath, TimeSpan.FromMinutes(5));

        var content = await File.ReadAllTextAsync(reportPath);
        await Verify(content, Settings);
    }



    [Fact]
    public async Task InitCompress_when_executed_expected_stableRspTemplate()
    {
        var outputPath = Path.Combine(_tempDir.Path, "compress.rsp");
        var command = new InitCompress { Output = outputPath };
        var console = Substitute.For<IConsole>();
        console.Out.Returns(TextWriter.Null);

        var exitCode = await command.OnExecuteAsync
        (
            console,
            Substitute.For<ILogger<InitCompress>>()
        );

        Assert.Equal(0, exitCode);
        var content = await File.ReadAllTextAsync(outputPath);
        await Verify(content, Settings);
    }



    [Fact]
    public async Task InitBundle_when_executed_expected_stableRspTemplate()
    {
        var outputPath = Path.Combine(_tempDir.Path, "bundle.rsp");
        var command = new InitBundle { Output = outputPath };
        var console = Substitute.For<IConsole>();
        console.Out.Returns(TextWriter.Null);

        var exitCode = await command.OnExecuteAsync
        (
            console,
            Substitute.For<ILogger<InitBundle>>()
        );

        Assert.Equal(0, exitCode);
        var content = await File.ReadAllTextAsync(outputPath);
        await Verify(content, Settings);
    }



    public void Dispose()
    {
        _tempDir.Dispose();
    }
}
