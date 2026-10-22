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
    private readonly TempDirectory _temp = new();



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

    }



    public void Dispose()
    {
        _temp.Dispose();
    }



    [Fact]
    public async Task OnExecuteAsync_when_canceled_expected_canceledExitCode()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        // Capture the token, not the source: the lambda below outlives this scope as far
        // as the analyzer can tell, and a CancellationToken is a struct that stays valid
        // after its source is disposed (ResharperAccessToDisposedClosure).
        var canceledToken = cts.Token;
        _bundleService.ExecuteAsync(Arg.Any<CompressionOptions>(), Arg.Any<CancellationToken>())
            .Returns<CompressionResult>(_ => throw new OperationCanceledException(canceledToken));

        var command = new Bundle { Path = "/tmp/logs", NoLock = true };

        var result = await command.OnExecuteAsync(_console, _logger, _bundleService, _reportService, _retentionService, cts.Token);

        // Deliberate cancellation is not an application error.
        Assert.Equal(ExitCode.Canceled, result);
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

        var reportPath = Path.Combine(_temp.Path, "bundle-report.csv");
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
                    SourcePath = _temp.Path,
                    OutputPath = Path.Combine(_temp.Path, "bundle.zip"),
                    Success = true,
                    OriginalSize = 1000,
                    CompressedSize = 200
                }
            );

        // NoLock omitted -> the real ProcessLock acquires a lock in the source
        // directory (a clean temp dir, so acquisition succeeds and is released).
        var command = new Bundle { Path = Path.Combine(_temp.Path, "logs") };

        var result = await command.OnExecuteAsync(_console, _logger, _bundleService, _reportService, _retentionService);

        Assert.Equal(ExitCode.Success, result);
    }



    [Fact]
    public async Task OnExecuteAsync_when_lockAlreadyHeld_expected_alreadyRunning()
    {
        // Hold the lock file open with FileShare.None — an open handle IS the
        // lock under the OpenOrCreate protocol (a mere leftover file no longer
        // blocks; see #172) — so the command's ProcessLock.TryAcquire fails.
        var lockFile = Path.Combine(_temp.Path, ".logc.lock");
        await using var heldLock = new FileStream
        (
            lockFile,
            FileMode.OpenOrCreate,
            FileAccess.Write,
            FileShare.None
        );

        var command = new Bundle { Path = Path.Combine(_temp.Path, "logs") };

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
        // name (relative to the working directory) is used. A substitute file
        // system records the path without writing into the test host's working
        // directory, where parallel tests would race on it.
        var fileSystem = Substitute.For<IFileSystem>();
        fileSystem.CreateWrite(Arg.Any<string>()).Returns(_ => new MemoryStream());
        var reportService = new ReportService(fileSystem, TimeProvider.System);
        var command = new Bundle { Path = "/tmp/logs", NoLock = true, Report = "json" };

        var result = await command.OnExecuteAsync(_console, _logger, _bundleService, reportService, _retentionService);

        Assert.Equal(ExitCode.Success, result);
        fileSystem.Received(1).CreateWrite("bundle-report.json");
    }



    [Fact]
    public async Task OnExecuteAsync_when_bundleSucceeds_withoutReport_expected_success()
    {
        _bundleService.ExecuteAsync(Arg.Any<CompressionOptions>(), Arg.Any<CancellationToken>())
            .Returns(new CompressionResult { SourcePath = "logs", OutputPath = "logs.zip", Success = true });

        var command = new Bundle { Path = _temp.Path, NoLock = true };

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

        var command = new Bundle { Path = _temp.Path, NoLock = true };

        var result = await command.OnExecuteAsync(_console, _logger, _bundleService, _reportService, _retentionService);

        Assert.Equal(ExitCode.ApplicationError, result);
        Assert.Contains("boom", stderr.ToString(), StringComparison.Ordinal);
    }



    [Fact]
    public async Task OnExecuteAsync_when_retention_withoutOutputPath_expected_sourceParentScanned()
    {
        _bundleService.ExecuteAsync(Arg.Any<CompressionOptions>(), Arg.Any<CancellationToken>())
            .Returns(new CompressionResult { SourcePath = "logs", OutputPath = "logs.zip", Success = true });

        var command = new Bundle { Path = _temp.Path, NoLock = true, DeleteArchivesOlderThan = 30 };

        await command.OnExecuteAsync(_console, _logger, _bundleService, _reportService, _retentionService);

        // Bundle writes next to the source directory, so retention scans the
        // source's PARENT (GetDirectoryName of the source path).
        _retentionFileSystem.Received(1).DirectoryExists(Path.GetDirectoryName(Path.GetFullPath(_temp.Path))!);
    }



    [Fact]
    public async Task OnExecuteAsync_when_retention_withOutputPath_expected_outputDirectoryScanned()
    {
        _bundleService.ExecuteAsync(Arg.Any<CompressionOptions>(), Arg.Any<CancellationToken>())
            .Returns(new CompressionResult { SourcePath = "logs", OutputPath = "logs.zip", Success = true });
        var outputDir = Path.Combine(_temp.Path, "archives");
        Directory.CreateDirectory(outputDir);

        var command = new Bundle { Path = _temp.Path, Output = outputDir, NoLock = true, DeleteArchivesOlderThan = 30 };

        await command.OnExecuteAsync(_console, _logger, _bundleService, _reportService, _retentionService);

        _retentionFileSystem.Received(1).DirectoryExists(Path.GetFullPath(outputDir));
    }



    [Fact]
    public async Task OnExecuteAsync_when_directorySourceWithRetention_expected_parentScannedAndOnlyThisBundlesArchivesDeleted()
    {
        // A directory is bundled into its PARENT, which usually holds other
        // applications' files: retention must delete only archives named like
        // this bundle's (#352).
        var logDir = Path.Combine(_temp.Path, "MyApp");
        Directory.CreateDirectory(logDir);
        var own = Path.Combine(_temp.Path, "MyApp-2026-01-01_00-00-00 to 2026-01-02_00-00-00.zip");
        var otherApp = Path.Combine(_temp.Path, "OtherApp-2026-01-01_00-00-00 to 2026-01-02_00-00-00.zip");
        var thirdParty = Path.Combine(_temp.Path, "backup.zip");
        foreach (var path in new[] { own, otherApp, thirdParty })
        {
            await File.WriteAllTextAsync(path, "x");
            File.SetLastWriteTime(path, DateTime.Today.AddDays(-90));
            _retentionFileSystem.GetFileInfo(path).Returns(new FileInfo(path));
        }

        _retentionFileSystem.DirectoryExists(Path.GetFullPath(_temp.Path)).Returns(returnThis: true);
        _retentionFileSystem.EnumerateFiles(Path.GetFullPath(_temp.Path), "*", SearchOption.TopDirectoryOnly).Returns([own, otherApp, thirdParty]);
        _bundleService.ExecuteAsync(Arg.Any<CompressionOptions>(), Arg.Any<CancellationToken>())
            .Returns(new CompressionResult { SourcePath = logDir, OutputPath = own, Success = true });

        var command = new Bundle { Path = logDir, NoLock = true, DeleteArchivesOlderThan = 30 };

        var result = await command.OnExecuteAsync(_console, _logger, _bundleService, _reportService, _retentionService);

        Assert.Equal(ExitCode.Success, result);
        _retentionFileSystem.Received(1).DeleteFile(own);
        _retentionFileSystem.DidNotReceive().DeleteFile(otherApp);
        _retentionFileSystem.DidNotReceive().DeleteFile(thirdParty);
    }



    [Fact]
    public async Task OnExecuteAsync_when_noFilesMatched_expected_successAndEmptyReport()
    {
        _bundleService.ExecuteAsync(Arg.Any<CompressionOptions>(), Arg.Any<CancellationToken>())
            .Returns(new CompressionResult { SourcePath = "/tmp/logs", OutputPath = string.Empty, Success = true });
        var reportPath = Path.Combine(_temp.Path, "empty-report.csv");

        var command = new Bundle { Path = "/tmp/logs", NoLock = true, Report = "csv", ReportPath = reportPath };

        var result = await command.OnExecuteAsync(_console, _logger, _bundleService, _reportService, _retentionService);

        Assert.Equal(ExitCode.Success, result);
        var lines = await File.ReadAllLinesAsync(reportPath);
        Assert.Single(lines);   // header only: no archive, no row
    }



    [Fact]
    public async Task OnExecuteAsync_when_bundleWritten_expected_reportHasItsRow()
    {
        // "Nothing matched" needs BOTH success and an empty output path; a
        // written bundle must still get its report row (#303).
        _bundleService.ExecuteAsync(Arg.Any<CompressionOptions>(), Arg.Any<CancellationToken>())
            .Returns(new CompressionResult { SourcePath = "/tmp/logs", OutputPath = "/tmp/bundle.zip", Success = true });
        var reportPath = Path.Combine(_temp.Path, "written-report.csv");

        var command = new Bundle { Path = "/tmp/logs", NoLock = true, Report = "csv", ReportPath = reportPath };

        var result = await command.OnExecuteAsync(_console, _logger, _bundleService, _reportService, _retentionService);

        Assert.Equal(ExitCode.Success, result);
        Assert.Equal(2, (await File.ReadAllLinesAsync(reportPath)).Length);
    }



    [Fact]
    public async Task OnExecuteAsync_when_namePrefixWithRetention_expected_archivesOfThatPrefixDeleted()
    {
        // With --name the bundle's archives carry the prefix, not the folder
        // name, so retention must match on the prefix (#303).
        var logDir = Path.Combine(_temp.Path, "MyApp");
        Directory.CreateDirectory(logDir);
        var own = Path.Combine(_temp.Path, "nightly-2026-01-01_00-00-00 to 2026-01-02_00-00-00.zip");
        await File.WriteAllTextAsync(own, "x");
        File.SetLastWriteTime(own, DateTime.Today.AddDays(-90));
        _retentionFileSystem.GetFileInfo(own).Returns(new FileInfo(own));
        _retentionFileSystem.DirectoryExists(Path.GetFullPath(_temp.Path)).Returns(returnThis: true);
        _retentionFileSystem.EnumerateFiles(Path.GetFullPath(_temp.Path), "*", SearchOption.TopDirectoryOnly).Returns([own]);
        _bundleService.ExecuteAsync(Arg.Any<CompressionOptions>(), Arg.Any<CancellationToken>())
            .Returns(new CompressionResult { SourcePath = logDir, OutputPath = own, Success = true });

        var command = new Bundle { Path = logDir, Name = "nightly", NoLock = true, DeleteArchivesOlderThan = 30 };

        var result = await command.OnExecuteAsync(_console, _logger, _bundleService, _reportService, _retentionService);

        Assert.Equal(ExitCode.Success, result);
        _retentionFileSystem.Received(1).DeleteFile(own);
    }
}
