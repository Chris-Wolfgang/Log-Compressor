using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Wolfgang.LogCompressor.Abstraction;
using Wolfgang.LogCompressor.Command;
using Wolfgang.LogCompressor.Model;
using Wolfgang.LogCompressor.Service;

namespace Wolfgang.LogCompressor.Tests.Unit.Command;

public sealed class BundleCommandTests : IDisposable
{
    private readonly IConsole _console = Substitute.For<IConsole>();
    private readonly ILogger<Bundle> _logger = Substitute.For<ILogger<Bundle>>();
    private readonly BundleService _bundleService;
    private readonly ReportService _reportService = new();
    private readonly IFileSystem _retentionFileSystem = Substitute.For<IFileSystem>();
    private readonly RetentionService _retentionService;
    private readonly string _tempDir;



    public BundleCommandTests()
    {
        _console.Out.Returns(new StringWriter());
        _console.Error.Returns(new StringWriter());

        _bundleService = Substitute.For<BundleService>
        (
            Substitute.For<IFileSystem>(),
            Substitute.For<IFileFilter>(),
            Substitute.For<IFileNamer>(),
            Substitute.For<IArchiveVerifier>(),
            Substitute.For<Wolfgang.LogCompressor.Service.Compression.CompressionStrategyFactory>(),
            Substitute.For<ILogger<BundleService>>()
        );

        _retentionService = new RetentionService
        (
            _retentionFileSystem,
            Substitute.For<ILogger<RetentionService>>()
        );

        _tempDir = Path.Combine(Path.GetTempPath(), "BundleCommandTests_" + Guid.NewGuid());
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
    public async Task OnExecuteAsync_when_validArgs_expected_success()
    {
        _bundleService.ExecuteAsync(Arg.Any<CompressionOptions>(), Arg.Any<CancellationToken>())
            .Returns
            (
                new CompressionResult
                {
                    SourcePath = "/tmp/logs",
                    OutputPath = "/tmp/logs/bundle.zip",
                    Success = true,
                    OriginalSize = 1000,
                    CompressedSize = 200
                }
            );

        var command = new Bundle { Path = "/tmp/logs", NoLock = true };

        var result = await command.OnExecuteAsync(_console, _logger, _bundleService, _reportService, _retentionService);

        Assert.Equal(ExitCode.Success, result);
    }



    [Fact]
    public async Task OnExecuteAsync_when_bundleFails_expected_applicationError()
    {
        _bundleService.ExecuteAsync(Arg.Any<CompressionOptions>(), Arg.Any<CancellationToken>())
            .Returns
            (
                new CompressionResult
                {
                    SourcePath = "/tmp/logs",
                    OutputPath = string.Empty,
                    Success = false,
                    ErrorMessage = "No files matched"
                }
            );

        var command = new Bundle { Path = "/tmp/logs", NoLock = true };

        var result = await command.OnExecuteAsync(_console, _logger, _bundleService, _reportService, _retentionService);

        Assert.Equal(ExitCode.ApplicationError, result);
    }



    [Fact]
    public async Task OnExecuteAsync_when_invalidOptions_expected_invalidArguments()
    {
        var command = new Bundle
        {
            Path = "/tmp",
            OlderThan = 7,
            MaxDateTime = "2026-12-31"
        };

        var result = await command.OnExecuteAsync(_console, _logger, _bundleService, _reportService, _retentionService);

        Assert.Equal(ExitCode.InvalidArguments, result);
    }



    [Fact]
    public async Task OnExecuteAsync_when_serviceThrows_expected_applicationError()
    {
        _bundleService.ExecuteAsync(Arg.Any<CompressionOptions>(), Arg.Any<CancellationToken>())
            .Returns<CompressionResult>(_ => throw new IOException("disk full"));

        var command = new Bundle { Path = "/tmp", NoLock = true };

        var result = await command.OnExecuteAsync(_console, _logger, _bundleService, _reportService, _retentionService);

        Assert.Equal(ExitCode.ApplicationError, result);
    }



    [Fact]
    public async Task OnExecuteAsync_when_reportOptionSet_expected_reportWritten()
    {
        _bundleService.ExecuteAsync(Arg.Any<CompressionOptions>(), Arg.Any<CancellationToken>())
            .Returns
            (
                new CompressionResult
                {
                    SourcePath = "/tmp/logs",
                    OutputPath = "/tmp/logs/bundle.zip",
                    Success = true,
                    OriginalSize = 1000,
                    CompressedSize = 200
                }
            );

        var reportPath = Path.Combine(_tempDir, "bundle-report.csv");
        var command = new Bundle
        {
            Path = "/tmp/logs",
            NoLock = true,
            Report = "csv",
            ReportPath = reportPath
        };

        var result = await command.OnExecuteAsync(_console, _logger, _bundleService, _reportService, _retentionService);

        Assert.Equal(ExitCode.Success, result);
        Assert.True(File.Exists(reportPath));
    }



    [Fact]
    public async Task OnExecuteAsync_when_deleteArchivesOlderThanSet_expected_retentionRuns()
    {
        _bundleService.ExecuteAsync(Arg.Any<CompressionOptions>(), Arg.Any<CancellationToken>())
            .Returns
            (
                new CompressionResult
                {
                    SourcePath = "/tmp/logs",
                    OutputPath = "/tmp/logs/bundle.zip",
                    Success = true,
                    OriginalSize = 1000,
                    CompressedSize = 200
                }
            );

        var command = new Bundle { Path = "/tmp/logs", NoLock = true, DeleteArchivesOlderThan = 60 };

        var result = await command.OnExecuteAsync(_console, _logger, _bundleService, _reportService, _retentionService);

        Assert.Equal(ExitCode.Success, result);
        _retentionFileSystem.Received(1).DirectoryExists(Arg.Any<string>());
    }
}
