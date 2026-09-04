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
    private readonly ReportService _reportService = new(new FileSystemWrapper(), TimeProvider.System);
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
            Substitute.For<ILogger<RetentionService>>(),
            TimeProvider.System
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
    public async Task OnExecuteAsync_when_bundleCompletesWithSkippedInputs_expected_completedWithSkips()
    {
        _bundleService.ExecuteAsync(Arg.Any<CompressionOptions>(), Arg.Any<CancellationToken>())
            .Returns
            (
                new CompressionResult
                {
                    SourcePath = "/tmp/logs",
                    OutputPath = "/tmp/logs.zip",
                    Success = false,
                    SkippedCount = 2,
                    ErrorMessage = "2 file(s) could not be read and were skipped"
                }
            );

        var command = new Bundle { Path = "/tmp/logs", NoLock = true };

        var result = await command.OnExecuteAsync(_console, _logger, _bundleService, _reportService, _retentionService);

        // The archive was written; only some inputs were skipped — degraded (3),
        // not broken (11).
        Assert.Equal(ExitCode.CompletedWithSkips, result);
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



    [Fact]
    public async Task OnExecuteAsync_when_lockNotHeld_expected_acquiresLockAndSucceeds()
    {
        _bundleService.ExecuteAsync(Arg.Any<CompressionOptions>(), Arg.Any<CancellationToken>())
            .Returns
            (
                new CompressionResult
                {
                    SourcePath = _tempDir,
                    OutputPath = Path.Combine(_tempDir, "bundle.zip"),
                    Success = true,
                    OriginalSize = 1000,
                    CompressedSize = 200
                }
            );

        // NoLock omitted -> the real ProcessLock acquires a lock in the source
        // directory (a clean temp dir, so acquisition succeeds and is released).
        var command = new Bundle { Path = Path.Combine(_tempDir, "logs") };

        var result = await command.OnExecuteAsync(_console, _logger, _bundleService, _reportService, _retentionService);

        Assert.Equal(ExitCode.Success, result);
    }



    [Fact]
    public async Task OnExecuteAsync_when_lockAlreadyHeld_expected_alreadyRunning()
    {
        // Hold the lock file open with FileShare.None — an open handle IS the
        // lock under the OpenOrCreate protocol (a mere leftover file no longer
        // blocks; see #172) — so the command's ProcessLock.TryAcquire fails.
        var lockFile = Path.Combine(_tempDir, ".logc.lock");
        await using var heldLock = new FileStream
        (
            lockFile,
            FileMode.OpenOrCreate,
            FileAccess.Write,
            FileShare.None
        );

        var command = new Bundle { Path = Path.Combine(_tempDir, "logs") };

        var result = await command.OnExecuteAsync(_console, _logger, _bundleService, _reportService, _retentionService);

        Assert.Equal(ExitCode.AlreadyRunning, result);
    }



    [Fact]
    public async Task OnExecuteAsync_when_reportPathNotSet_expected_defaultReportNameUsed()
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

        // Report set but ReportPath omitted -> the default "bundle-report.<fmt>"
        // name (relative to the working directory) is used.
        const string defaultReport = "bundle-report.json";
        try
        {
            var command = new Bundle { Path = "/tmp/logs", NoLock = true, Report = "json" };

            var result = await command.OnExecuteAsync(_console, _logger, _bundleService, _reportService, _retentionService);

            Assert.Equal(ExitCode.Success, result);
            Assert.True(File.Exists(defaultReport));
        }
        finally
        {
            if (File.Exists(defaultReport))
            {
                File.Delete(defaultReport);
            }
        }
    }

    [Fact]
    public async Task OnExecuteAsync_when_bundleSucceeds_withoutReport_expected_success()
    {
        _bundleService.ExecuteAsync(Arg.Any<CompressionOptions>(), Arg.Any<CancellationToken>())
            .Returns(new CompressionResult { SourcePath = "logs", OutputPath = "logs.zip", Success = true });

        var command = new Bundle { Path = _tempDir, NoLock = true };

        var result = await command.OnExecuteAsync(_console, _logger, _bundleService, _reportService, _retentionService);

        Assert.Equal(ExitCode.Success, result);
    }



    [Fact]
    public async Task OnExecuteAsync_when_bundleFails_expected_applicationErrorAndMessageOnStderr()
    {
        _bundleService.ExecuteAsync(Arg.Any<CompressionOptions>(), Arg.Any<CancellationToken>())
            .Returns(new CompressionResult { SourcePath = "logs", OutputPath = "logs.zip", Success = false, ErrorMessage = "boom" });
        var stderr = new StringWriter();
        _console.Error.Returns(stderr);

        var command = new Bundle { Path = _tempDir, NoLock = true };

        var result = await command.OnExecuteAsync(_console, _logger, _bundleService, _reportService, _retentionService);

        Assert.Equal(ExitCode.ApplicationError, result);
        Assert.Contains("boom", stderr.ToString(), StringComparison.Ordinal);
    }



    [Fact]
    public async Task OnExecuteAsync_when_retention_withoutOutputPath_expected_sourceParentScanned()
    {
        _bundleService.ExecuteAsync(Arg.Any<CompressionOptions>(), Arg.Any<CancellationToken>())
            .Returns(new CompressionResult { SourcePath = "logs", OutputPath = "logs.zip", Success = true });

        var command = new Bundle { Path = _tempDir, NoLock = true, DeleteArchivesOlderThan = 30 };

        await command.OnExecuteAsync(_console, _logger, _bundleService, _reportService, _retentionService);

        // Bundle writes next to the source directory, so retention scans the
        // source's PARENT (GetDirectoryName of the source path).
        _retentionFileSystem.Received(1).DirectoryExists(Path.GetDirectoryName(Path.GetFullPath(_tempDir))!);
    }



    [Fact]
    public async Task OnExecuteAsync_when_retention_withOutputPath_expected_outputDirectoryScanned()
    {
        _bundleService.ExecuteAsync(Arg.Any<CompressionOptions>(), Arg.Any<CancellationToken>())
            .Returns(new CompressionResult { SourcePath = "logs", OutputPath = "logs.zip", Success = true });
        var outputDir = Path.Combine(_tempDir, "archives");
        Directory.CreateDirectory(outputDir);

        var command = new Bundle { Path = _tempDir, Output = outputDir, NoLock = true, DeleteArchivesOlderThan = 30 };

        await command.OnExecuteAsync(_console, _logger, _bundleService, _reportService, _retentionService);

        _retentionFileSystem.Received(1).DirectoryExists(Path.GetFullPath(outputDir));
    }

}
