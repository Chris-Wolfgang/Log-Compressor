using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Wolfgang.LogCompressor.Abstraction;
using Wolfgang.LogCompressor.Command;
using Wolfgang.LogCompressor.Model;
using Wolfgang.LogCompressor.Service;

namespace Wolfgang.LogCompressor.Tests.Unit.Command;

public sealed class CompressCommandTests : IDisposable
{
    private readonly IConsole _console = Substitute.For<IConsole>();
    private readonly ILogger<Compress> _logger = Substitute.For<ILogger<Compress>>();
    private readonly CompressService _compressService;
    private readonly ReportService _reportService = new(new FileSystemWrapper(), TimeProvider.System);
    private readonly IFileSystem _retentionFileSystem = Substitute.For<IFileSystem>();
    private readonly RetentionService _retentionService;
    private readonly string _tempDir;



    public CompressCommandTests()
    {
        _console.Out.Returns(new StringWriter());
        _console.Error.Returns(new StringWriter());

        _compressService = Substitute.For<CompressService>
        (
            Substitute.For<IFileSystem>(),
            Substitute.For<IFileFilter>(),
            Substitute.For<IFileNamer>(),
            Substitute.For<IArchiveVerifier>(),
            Substitute.For<Wolfgang.LogCompressor.Service.Compression.CompressionStrategyFactory>(),
            Substitute.For<ILogger<CompressService>>()
        );

        _retentionService = new RetentionService
        (
            _retentionFileSystem,
            Substitute.For<ILogger<RetentionService>>(),
            TimeProvider.System
        );

        _tempDir = Path.Combine(Path.GetTempPath(), "CompressCommandTests_" + Guid.NewGuid());
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
    public async Task OnExecuteAsync_when_canceled_expected_canceledExitCode()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        // The closure runs inside the awaited OnExecuteAsync call below,
        // before cts is disposed at the end of this method.
        // ReSharper disable once AccessToDisposedClosure
        _compressService.ExecuteAsync(Arg.Any<CompressionOptions>(), Arg.Any<CancellationToken>())
            .Returns<IReadOnlyList<CompressionResult>>(_ => throw new OperationCanceledException(cts.Token));

        var command = new Compress { Path = "/tmp/test.log", NoLock = true };

        var result = await command.OnExecuteAsync(_console, _logger, _compressService, _reportService, _retentionService, cts.Token);

        // Deliberate cancellation is not an application error.
        Assert.Equal(ExitCode.Canceled, result);
    }



    [Fact]
    public async Task OnExecuteAsync_when_validArgs_expected_success()
    {
        _compressService.ExecuteAsync(Arg.Any<CompressionOptions>(), Arg.Any<CancellationToken>())
            .Returns
            (
                new List<CompressionResult>
                {
                    new() { SourcePath = "a.log", OutputPath = "a.zip", Success = true, OriginalSize = 100, CompressedSize = 50 }
                }
            );

        var command = new Compress { Path = "/tmp/test.log", NoLock = true };

        var result = await command.OnExecuteAsync(_console, _logger, _compressService, _reportService, _retentionService);

        Assert.Equal(ExitCode.Success, result);
    }



    [Fact]
    public async Task OnExecuteAsync_when_someFilesFail_expected_completedWithSkips()
    {
        _compressService.ExecuteAsync(Arg.Any<CompressionOptions>(), Arg.Any<CancellationToken>())
            .Returns
            (
                new List<CompressionResult>
                {
                    new() { SourcePath = "a.log", OutputPath = "a.zip", Success = true },
                    new() { SourcePath = "b.log", OutputPath = "b.zip", Success = false, ErrorMessage = "error" }
                }
            );

        var command = new Compress { Path = "/tmp", NoLock = true };

        var result = await command.OnExecuteAsync(_console, _logger, _compressService, _reportService, _retentionService);

        // Default skip mode: partial failure is "completed with skips" (3), not
        // ApplicationError (11) — schedulers must distinguish degraded from broken.
        Assert.Equal(ExitCode.CompletedWithSkips, result);
    }



    [Fact]
    public async Task OnExecuteAsync_when_invalidOptions_expected_invalidArguments()
    {
        var command = new Compress
        {
            Path = "/tmp",
            OlderThan = 7,
            MinDateTime = "2026-01-01"
        };

        var result = await command.OnExecuteAsync(_console, _logger, _compressService, _reportService, _retentionService);

        Assert.Equal(ExitCode.InvalidArguments, result);
    }



    [Fact]
    public async Task OnExecuteAsync_when_serviceThrows_expected_applicationError()
    {
        _compressService.ExecuteAsync(Arg.Any<CompressionOptions>(), Arg.Any<CancellationToken>())
            .Returns<IReadOnlyList<CompressionResult>>(_ => throw new IOException("disk full"));

        var command = new Compress { Path = "/tmp", NoLock = true };

        var result = await command.OnExecuteAsync(_console, _logger, _compressService, _reportService, _retentionService);

        Assert.Equal(ExitCode.ApplicationError, result);
    }



    [Fact]
    public async Task OnExecuteAsync_when_reportOptionSet_expected_reportWritten()
    {
        _compressService.ExecuteAsync(Arg.Any<CompressionOptions>(), Arg.Any<CancellationToken>())
            .Returns
            (
                new List<CompressionResult>
                {
                    new() { SourcePath = "a.log", OutputPath = "a.zip", Success = true, OriginalSize = 100, CompressedSize = 50 }
                }
            );

        var reportPath = Path.Combine(_tempDir, "compress-report.json");
        var command = new Compress
        {
            Path = "/tmp/test.log",
            NoLock = true,
            Report = "json",
            ReportPath = reportPath
        };

        var result = await command.OnExecuteAsync(_console, _logger, _compressService, _reportService, _retentionService);

        Assert.Equal(ExitCode.Success, result);
        Assert.True(File.Exists(reportPath));
    }



    [Fact]
    public async Task OnExecuteAsync_when_deleteArchivesOlderThanSet_expected_retentionRuns()
    {
        _compressService.ExecuteAsync(Arg.Any<CompressionOptions>(), Arg.Any<CancellationToken>())
            .Returns
            (
                new List<CompressionResult>
                {
                    new() { SourcePath = "a.log", OutputPath = "a.zip", Success = true, OriginalSize = 100, CompressedSize = 50 }
                }
            );

        var command = new Compress { Path = "/tmp/test.log", NoLock = true, DeleteArchivesOlderThan = 30 };

        var result = await command.OnExecuteAsync(_console, _logger, _compressService, _reportService, _retentionService);

        Assert.Equal(ExitCode.Success, result);
        _retentionFileSystem.Received(1).DirectoryExists(Arg.Any<string>());
    }



    [Fact]
    public async Task OnExecuteAsync_when_sourceIsDirectory_expected_retentionScansSourceDirectory()
    {
        _compressService.ExecuteAsync(Arg.Any<CompressionOptions>(), Arg.Any<CancellationToken>())
            .Returns(new List<CompressionResult>());

        var command = new Compress { Path = _tempDir, NoLock = true, DeleteArchivesOlderThan = 30 };

        var result = await command.OnExecuteAsync(_console, _logger, _compressService, _reportService, _retentionService);

        Assert.Equal(ExitCode.Success, result);

        // Retention must scan the source directory itself (where compress writes the
        // archives), not its parent.
        _retentionFileSystem.Received(1).DirectoryExists(Path.GetFullPath(_tempDir));
    }



    [Fact]
    public async Task OnExecuteAsync_when_lockNotHeld_expected_acquiresLockAndSucceeds()
    {
        _compressService.ExecuteAsync(Arg.Any<CompressionOptions>(), Arg.Any<CancellationToken>())
            .Returns
            (
                new List<CompressionResult>
                {
                    new() { SourcePath = "a.log", OutputPath = "a.zip", Success = true }
                }
            );

        // NoLock omitted -> the real ProcessLock acquires a lock in the source
        // directory (a clean temp dir, so acquisition succeeds and is released).
        var command = new Compress { Path = Path.Combine(_tempDir, "test.log") };

        var result = await command.OnExecuteAsync(_console, _logger, _compressService, _reportService, _retentionService);

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

        var command = new Compress { Path = Path.Combine(_tempDir, "test.log") };

        var result = await command.OnExecuteAsync(_console, _logger, _compressService, _reportService, _retentionService);

        Assert.Equal(ExitCode.AlreadyRunning, result);
    }



    [Fact]
    public async Task OnExecuteAsync_when_reportPathNotSet_expected_defaultReportNameUsed()
    {
        _compressService.ExecuteAsync(Arg.Any<CompressionOptions>(), Arg.Any<CancellationToken>())
            .Returns
            (
                new List<CompressionResult>
                {
                    new() { SourcePath = "a.log", OutputPath = "a.zip", Success = true }
                }
            );

        // Report set but ReportPath omitted -> the default "compress-report.<fmt>"
        // name (relative to the working directory) is used.
        const string defaultReport = "compress-report.json";
        try
        {
            var command = new Compress { Path = "/tmp/test.log", NoLock = true, Report = "json" };

            var result = await command.OnExecuteAsync(_console, _logger, _compressService, _reportService, _retentionService);

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
    public async Task OnExecuteAsync_when_allSucceed_withoutReport_expected_success()
    {
        _compressService.ExecuteAsync(Arg.Any<CompressionOptions>(), Arg.Any<CancellationToken>())
            .Returns([new CompressionResult { SourcePath = "a.log", OutputPath = "a.zip", Success = true }]);

        var command = new Compress { Path = Path.Combine(_tempDir, "test.log"), NoLock = true };

        var result = await command.OnExecuteAsync(_console, _logger, _compressService, _reportService, _retentionService);

        Assert.Equal(ExitCode.Success, result);
    }



    [Fact]
    public async Task OnExecuteAsync_when_onErrorFail_and_anyFileFails_expected_applicationError()
    {
        _compressService.ExecuteAsync(Arg.Any<CompressionOptions>(), Arg.Any<CancellationToken>())
            .Returns(
            [
                new CompressionResult { SourcePath = "a.log", OutputPath = "a.zip", Success = true },
                new CompressionResult { SourcePath = "b.log", OutputPath = "b.zip", Success = false, ErrorMessage = "boom" }
            ]);

        var command = new Compress { Path = Path.Combine(_tempDir, "test.log"), NoLock = true, OnError = "fail" };

        var result = await command.OnExecuteAsync(_console, _logger, _compressService, _reportService, _retentionService);

        Assert.Equal(ExitCode.ApplicationError, result);
    }



    [Fact]
    public async Task OnExecuteAsync_when_retention_withFileSource_expected_parentDirectoryScanned()
    {
        _compressService.ExecuteAsync(Arg.Any<CompressionOptions>(), Arg.Any<CancellationToken>())
            .Returns([new CompressionResult { SourcePath = "a.log", OutputPath = "a.zip", Success = true }]);
        var sourceFile = Path.Combine(_tempDir, "test.log");
        await File.WriteAllTextAsync(sourceFile, "x");

        var command = new Compress { Path = sourceFile, NoLock = true, DeleteArchivesOlderThan = 30 };

        await command.OnExecuteAsync(_console, _logger, _compressService, _reportService, _retentionService);

        // SourcePath is a FILE, so retention must scan its parent directory.
        _retentionFileSystem.Received(1).DirectoryExists(Path.GetFullPath(_tempDir));
    }



    [Fact]
    public async Task OnExecuteAsync_when_retention_withDirectorySource_expected_sourceDirectoryScanned()
    {
        _compressService.ExecuteAsync(Arg.Any<CompressionOptions>(), Arg.Any<CancellationToken>())
            .Returns([new CompressionResult { SourcePath = "a.log", OutputPath = "a.zip", Success = true }]);

        var command = new Compress { Path = _tempDir, NoLock = true, DeleteArchivesOlderThan = 30 };

        await command.OnExecuteAsync(_console, _logger, _compressService, _reportService, _retentionService);

        _retentionFileSystem.Received(1).DirectoryExists(Path.GetFullPath(_tempDir));
    }



    [Fact]
    public async Task OnExecuteAsync_when_retention_withOutputPath_expected_outputDirectoryScanned()
    {
        _compressService.ExecuteAsync(Arg.Any<CompressionOptions>(), Arg.Any<CancellationToken>())
            .Returns([new CompressionResult { SourcePath = "a.log", OutputPath = "a.zip", Success = true }]);
        var outputDir = Path.Combine(_tempDir, "archives");
        Directory.CreateDirectory(outputDir);

        var command = new Compress { Path = _tempDir, Output = outputDir, NoLock = true, DeleteArchivesOlderThan = 30 };

        await command.OnExecuteAsync(_console, _logger, _compressService, _reportService, _retentionService);

        _retentionFileSystem.Received(1).DirectoryExists(Path.GetFullPath(outputDir));
    }

}
