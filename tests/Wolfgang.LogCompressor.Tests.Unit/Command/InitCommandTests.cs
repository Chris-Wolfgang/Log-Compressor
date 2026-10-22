using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Wolfgang.LogCompressor.Command;

namespace Wolfgang.LogCompressor.Tests.Unit.Command;

/// <summary>
/// <c>logc init</c> never overwrites an existing, possibly hand-edited, configuration file unless
/// <c>--force</c> is given (#353).
/// </summary>
public sealed class InitCommandTests : IDisposable
{
    private readonly TempDirectory _tempDir = new();
    private readonly IConsole _console = Substitute.For<IConsole>();
    private readonly StringWriter _error = new();



    public InitCommandTests()
    {
        _console.Out.Returns(TextWriter.Null);
        _console.Error.Returns(_error);
    }



    [Fact]
    public async Task InitCompress_when_fileExistsWithoutForce_expected_refusedAndFileUnchanged()
    {
        var path = _tempDir.WriteFile("compress.rsp", "hand-edited");

        var exitCode = await new InitCompress { Output = path }.OnExecuteAsync(_console, Substitute.For<ILogger<InitCompress>>());

        Assert.Equal(ExitCode.InvalidArguments, exitCode);
        Assert.Equal("hand-edited", await File.ReadAllTextAsync(path));
        Assert.Contains("--force", _error.ToString(), StringComparison.Ordinal);
    }



    [Fact]
    public async Task InitCompress_when_fileExistsWithForce_expected_overwritten()
    {
        var path = _tempDir.WriteFile("compress.rsp", "hand-edited");

        var exitCode = await new InitCompress { Output = path, Force = true }.OnExecuteAsync(_console, Substitute.For<ILogger<InitCompress>>());

        Assert.Equal(ExitCode.Success, exitCode);
        Assert.StartsWith("# logc compress configuration", await File.ReadAllTextAsync(path), StringComparison.Ordinal);
    }



    [Fact]
    public async Task InitBundle_when_fileExistsWithoutForce_expected_refusedAndFileUnchanged()
    {
        var path = _tempDir.WriteFile("bundle.rsp", "hand-edited");

        var exitCode = await new InitBundle { Output = path }.OnExecuteAsync(_console, Substitute.For<ILogger<InitBundle>>());

        Assert.Equal(ExitCode.InvalidArguments, exitCode);
        Assert.Equal("hand-edited", await File.ReadAllTextAsync(path));
    }



    [Fact]
    public async Task InitBundle_when_fileExistsWithForce_expected_overwritten()
    {
        var path = _tempDir.WriteFile("bundle.rsp", "hand-edited");

        var exitCode = await new InitBundle { Output = path, Force = true }.OnExecuteAsync(_console, Substitute.For<ILogger<InitBundle>>());

        Assert.Equal(ExitCode.Success, exitCode);
        Assert.StartsWith("# logc bundle configuration", await File.ReadAllTextAsync(path), StringComparison.Ordinal);
    }



    public void Dispose()
    {
        _error.Dispose();
        _tempDir.Dispose();
    }
}
