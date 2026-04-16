using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Wolfgang.LogCompressor.Command;
using Wolfgang.LogCompressor.Model;
using Wolfgang.LogCompressor.Service;

namespace Wolfgang.LogCompressor.Tests.Unit.Command;

public sealed class CompressCommandTests
{
    private readonly IConsole _console = Substitute.For<IConsole>();
    private readonly ILogger<Compress> _logger = Substitute.For<ILogger<Compress>>();
    private readonly CompressService _compressService;
    private readonly ReportService _reportService = new();
    private readonly RetentionService _retentionService;



    public CompressCommandTests()
    {
        _console.Out.Returns(new StringWriter());
        _console.Error.Returns(new StringWriter());

        _compressService = Substitute.For<CompressService>
        (
            Substitute.For<Wolfgang.LogCompressor.Abstraction.IFileSystem>(),
            Substitute.For<Wolfgang.LogCompressor.Abstraction.IFileFilter>(),
            Substitute.For<Wolfgang.LogCompressor.Abstraction.IFileNamer>(),
            Substitute.For<Wolfgang.LogCompressor.Abstraction.IArchiveVerifier>(),
            Substitute.For<Wolfgang.LogCompressor.Service.Compression.CompressionStrategyFactory>(),
            Substitute.For<ILogger<CompressService>>()
        );

        _retentionService = new RetentionService
        (
            Substitute.For<Wolfgang.LogCompressor.Abstraction.IFileSystem>(),
            Substitute.For<ILogger<RetentionService>>()
        );
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
    public async Task OnExecuteAsync_when_someFilesFail_expected_applicationError()
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

        Assert.Equal(ExitCode.ApplicationError, result);
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
}
