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
    private readonly TempDirectory _temp = new();



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
        _compressService.ExecuteAsync(Arg.Any<CompressionOptions>(), Arg.Any<CancellationToken>())
            .Returns<IReadOnlyList<CompressionResult>>(_ => throw new OperationCanceledException(canceledToken));

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

        var reportPath = Path.Combine(_temp.Path, "compress-report.json");
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

        var command = new Compress { Path = _temp.Path, NoLock = true, DeleteArchivesOlderThan = 30 };

        var result = await command.OnExecuteAsync(_console, _logger, _compressService, _reportService, _retentionService);

        Assert.Equal(ExitCode.Success, result);

        // Retention must scan the source directory itself (where compress writes the
        // archives), not its parent.
        _retentionFileSystem.Received(1).DirectoryExists(Path.GetFullPath(_temp.Path));
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
        var command = new Compress { Path = Path.Combine(_temp.Path, "test.log") };

        var result = await command.OnExecuteAsync(_console, _logger, _compressService, _reportService, _retentionService);

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

        var command = new Compress { Path = Path.Combine(_temp.Path, "test.log") };

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
        // name (relative to the working directory) is used. A substitute file
        // system records the path without writing into the test host's working
        // directory, where parallel tests would race on it.
        var fileSystem = Substitute.For<IFileSystem>();
        fileSystem.CreateWrite(Arg.Any<string>()).Returns(_ => new MemoryStream());
        var reportService = new ReportService(fileSystem, TimeProvider.System);
        var command = new Compress { Path = "/tmp/test.log", NoLock = true, Report = "json" };

        var result = await command.OnExecuteAsync(_console, _logger, _compressService, reportService, _retentionService);

        Assert.Equal(ExitCode.Success, result);
        fileSystem.Received(1).CreateWrite("compress-report.json");
    }



    [Fact]
    public async Task OnExecuteAsync_when_allSucceed_withoutReport_expected_success()
    {
        _compressService.ExecuteAsync(Arg.Any<CompressionOptions>(), Arg.Any<CancellationToken>())
            .Returns([new CompressionResult { SourcePath = "a.log", OutputPath = "a.zip", Success = true }]);

        var command = new Compress { Path = Path.Combine(_temp.Path, "test.log"), NoLock = true };

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

        var command = new Compress { Path = Path.Combine(_temp.Path, "test.log"), NoLock = true, OnError = "fail" };

        var result = await command.OnExecuteAsync(_console, _logger, _compressService, _reportService, _retentionService);

        Assert.Equal(ExitCode.ApplicationError, result);
    }



    [Fact]
    public async Task OnExecuteAsync_when_retention_withFileSource_expected_parentDirectoryScanned()
    {
        _compressService.ExecuteAsync(Arg.Any<CompressionOptions>(), Arg.Any<CancellationToken>())
            .Returns([new CompressionResult { SourcePath = "a.log", OutputPath = "a.zip", Success = true }]);
        var sourceFile = Path.Combine(_temp.Path, "test.log");
        await File.WriteAllTextAsync(sourceFile, "x");

        var command = new Compress { Path = sourceFile, NoLock = true, DeleteArchivesOlderThan = 30 };

        await command.OnExecuteAsync(_console, _logger, _compressService, _reportService, _retentionService);

        // SourcePath is a FILE, so retention must scan its parent directory.
        _retentionFileSystem.Received(1).DirectoryExists(Path.GetFullPath(_temp.Path));
    }



    [Fact]
    public async Task OnExecuteAsync_when_retention_withDirectorySource_expected_sourceDirectoryScanned()
    {
        _compressService.ExecuteAsync(Arg.Any<CompressionOptions>(), Arg.Any<CancellationToken>())
            .Returns([new CompressionResult { SourcePath = "a.log", OutputPath = "a.zip", Success = true }]);

        var command = new Compress { Path = _temp.Path, NoLock = true, DeleteArchivesOlderThan = 30 };

        await command.OnExecuteAsync(_console, _logger, _compressService, _reportService, _retentionService);

        _retentionFileSystem.Received(1).DirectoryExists(Path.GetFullPath(_temp.Path));
    }



    [Fact]
    public async Task OnExecuteAsync_when_retention_withOutputPath_expected_outputDirectoryScanned()
    {
        _compressService.ExecuteAsync(Arg.Any<CompressionOptions>(), Arg.Any<CancellationToken>())
            .Returns([new CompressionResult { SourcePath = "a.log", OutputPath = "a.zip", Success = true }]);
        var outputDir = Path.Combine(_temp.Path, "archives");
        Directory.CreateDirectory(outputDir);

        var command = new Compress { Path = _temp.Path, Output = outputDir, NoLock = true, DeleteArchivesOlderThan = 30 };

        await command.OnExecuteAsync(_console, _logger, _compressService, _reportService, _retentionService);

        _retentionFileSystem.Received(1).DirectoryExists(Path.GetFullPath(outputDir));
    }



    [Fact]
    public async Task OnExecuteAsync_when_someFilesFail_expected_failureCountOnStderr()
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

        var command = new Compress { Path = Path.Combine(_temp.Path, "test.log"), NoLock = true };

        await command.OnExecuteAsync(_console, _logger, _compressService, _reportService, _retentionService);

        Assert.Contains("1 file(s) failed to compress.", _console.Error.ToString(), StringComparison.Ordinal);
    }



    [Fact]
    public async Task OnExecuteAsync_when_allSucceed_expected_noFailureLineOnStderr()
    {
        _compressService.ExecuteAsync(Arg.Any<CompressionOptions>(), Arg.Any<CancellationToken>())
            .Returns([new CompressionResult { SourcePath = "a.log", OutputPath = "a.zip", Success = true }]);

        var command = new Compress { Path = Path.Combine(_temp.Path, "test.log"), NoLock = true };

        await command.OnExecuteAsync(_console, _logger, _compressService, _reportService, _retentionService);

        Assert.DoesNotContain("failed to compress", _console.Error.ToString(), StringComparison.Ordinal);
    }



    [Theory]
    [InlineData(false, SearchOption.AllDirectories)]
    [InlineData(true, SearchOption.TopDirectoryOnly)]
    public async Task OnExecuteAsync_when_recurseWithRetention_expected_retentionWalksWhereArchivesAreWritten(bool withOutput, SearchOption expected)
    {
        // compress --recurse without --output writes each archive next to its
        // source, in subdirectories too; with --output they all land in one
        // directory (#352).
        _compressService.ExecuteAsync(Arg.Any<CompressionOptions>(), Arg.Any<CancellationToken>())
            .Returns(new List<CompressionResult>());
        var output = Path.Combine(_temp.Path, "out");
        _retentionFileSystem.DirectoryExists(Arg.Any<string>()).Returns(returnThis: true);
        _retentionFileSystem.EnumerateFiles(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<SearchOption>()).Returns([]);

        var command = new Compress
        {
            Path = _temp.Path,
            Recurse = true,
            Output = withOutput ? output : null,
            NoLock = true,
            DeleteArchivesOlderThan = 30
        };

        var result = await command.OnExecuteAsync(_console, _logger, _compressService, _reportService, _retentionService);

        Assert.Equal(ExitCode.Success, result);
        _retentionFileSystem.Received(1).EnumerateFiles(Arg.Any<string>(), "*", expected);
    }
}
