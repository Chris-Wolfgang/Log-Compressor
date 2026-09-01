using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Wolfgang.LogCompressor.Abstraction;
using Wolfgang.LogCompressor.Command;
using Wolfgang.LogCompressor.Model;
using Wolfgang.LogCompressor.Service;

namespace Wolfgang.LogCompressor.Tests.Unit.Command;

public sealed class DecompressCommandTests : IDisposable
{
    private readonly IConsole _console = Substitute.For<IConsole>();
    private readonly ILogger<Decompress> _logger = Substitute.For<ILogger<Decompress>>();
    private readonly DecompressService _decompressService;
    private readonly ReportService _reportService = new();
    private readonly string _tempDir;



    public DecompressCommandTests()
    {
        _console.Out.Returns(new StringWriter());
        _console.Error.Returns(new StringWriter());

        _decompressService = Substitute.For<DecompressService>
        (
            Substitute.For<IFileSystem>(),
            Substitute.For<IFileFilter>(),
            Substitute.For<ILogger<DecompressService>>()
        );

        _tempDir = Path.Combine(Path.GetTempPath(), "DecompressCommandTests_" + Guid.NewGuid());
        Directory.CreateDirectory(_tempDir);
    }



    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
        {
            Directory.Delete(_tempDir, recursive: true);
        }
    }



    private static CompressionResult Ok() => new()
    {
        SourcePath = "a.zip",
        OutputPath = "/out",
        Success = true
    };



    [Fact]
    public async Task OnExecuteAsync_when_allSucceed_expected_success()
    {
        _decompressService.ExecuteAsync(Arg.Any<DecompressionOptions>(), Arg.Any<CancellationToken>())
            .Returns([Ok()]);

        var command = new Decompress { Path = _tempDir, NoLock = true };

        var result = await command.OnExecuteAsync(_console, _logger, _decompressService, _reportService);

        Assert.Equal(ExitCode.Success, result);
    }



    [Fact]
    public async Task OnExecuteAsync_when_anyArchiveFails_expected_applicationError()
    {
        _decompressService.ExecuteAsync(Arg.Any<DecompressionOptions>(), Arg.Any<CancellationToken>())
            .Returns(
            [
                Ok(),
                new CompressionResult { SourcePath = "b.zip", OutputPath = "/out", Success = false, ErrorMessage = "boom" }
            ]);

        var command = new Decompress { Path = _tempDir, NoLock = true };

        var result = await command.OnExecuteAsync(_console, _logger, _decompressService, _reportService);

        Assert.Equal(ExitCode.ApplicationError, result);
    }



    [Fact]
    public async Task OnExecuteAsync_when_invalidReportFormat_expected_invalidArguments()
    {
        var command = new Decompress { Path = _tempDir, NoLock = true, Report = "xml" };

        var result = await command.OnExecuteAsync(_console, _logger, _decompressService, _reportService);

        Assert.Equal(ExitCode.InvalidArguments, result);
        await _decompressService.DidNotReceive().ExecuteAsync(Arg.Any<DecompressionOptions>(), Arg.Any<CancellationToken>());
    }



    [Fact]
    public async Task OnExecuteAsync_when_reportPathWithoutReport_expected_invalidArguments()
    {
        var command = new Decompress { Path = _tempDir, NoLock = true, ReportPath = "somewhere.json" };

        var result = await command.OnExecuteAsync(_console, _logger, _decompressService, _reportService);

        Assert.Equal(ExitCode.InvalidArguments, result);
        await _decompressService.DidNotReceive().ExecuteAsync(Arg.Any<DecompressionOptions>(), Arg.Any<CancellationToken>());
    }



    [Fact]
    public async Task OnExecuteAsync_when_reportRequested_expected_reportWritten()
    {
        _decompressService.ExecuteAsync(Arg.Any<DecompressionOptions>(), Arg.Any<CancellationToken>())
            .Returns([Ok()]);
        var reportPath = Path.Combine(_tempDir, "report.json");

        var command = new Decompress { Path = _tempDir, NoLock = true, Report = "json", ReportPath = reportPath };

        var result = await command.OnExecuteAsync(_console, _logger, _decompressService, _reportService);

        Assert.Equal(ExitCode.Success, result);
        Assert.True(File.Exists(reportPath));
    }



    [Fact]
    public async Task OnExecuteAsync_when_lockAlreadyHeld_expected_alreadyRunning()
    {
        var lockFile = Path.Combine(Path.GetDirectoryName(_tempDir)!, ".logc.lock");
        await using var heldLock = new FileStream
        (
            lockFile,
            FileMode.OpenOrCreate,
            FileAccess.Write,
            FileShare.None
        );

        var command = new Decompress { Path = _tempDir };

        var result = await command.OnExecuteAsync(_console, _logger, _decompressService, _reportService);

        Assert.Equal(ExitCode.AlreadyRunning, result);
    }



    [Fact]
    public async Task OnExecuteAsync_when_serviceThrows_expected_applicationErrorAndMessageOnStderr()
    {
        _decompressService.ExecuteAsync(Arg.Any<DecompressionOptions>(), Arg.Any<CancellationToken>())
            .Returns<IReadOnlyList<CompressionResult>>(_ => throw new FileNotFoundException("no such source"));
        var stderr = new StringWriter();
        _console.Error.Returns(stderr);

        var command = new Decompress { Path = _tempDir, NoLock = true };

        var result = await command.OnExecuteAsync(_console, _logger, _decompressService, _reportService);

        Assert.Equal(ExitCode.ApplicationError, result);
        Assert.Contains("no such source", stderr.ToString(), StringComparison.Ordinal);
    }



    [Fact]
    public async Task OnExecuteAsync_when_optionsSet_expected_mappedIntoServiceOptions()
    {
        DecompressionOptions? captured = null;
        _decompressService.ExecuteAsync(Arg.Do<DecompressionOptions>(o => captured = o), Arg.Any<CancellationToken>())
            .Returns([Ok()]);
        var outputDir = Path.Combine(_tempDir, "out");
        Directory.CreateDirectory(outputDir);

        var command = new Decompress
        {
            Path = _tempDir,
            Output = outputDir,
            Recurse = true,
            Include = ["*.zip"],
            Exclude = ["*.bak"],
            Force = true,
            KeepArchives = true,
            NoLock = true
        };

        await command.OnExecuteAsync(_console, _logger, _decompressService, _reportService);

        Assert.NotNull(captured);
        Assert.Equal(Path.GetFullPath(_tempDir), captured.SourcePath);
        Assert.Equal(Path.GetFullPath(outputDir), captured.OutputPath);
        Assert.True(captured.Recurse);
        Assert.Equal(["*.zip"], captured.IncludePatterns);
        Assert.Equal(["*.bak"], captured.ExcludePatterns);
        Assert.True(captured.Force);
        Assert.True(captured.KeepArchives);
    }
}
