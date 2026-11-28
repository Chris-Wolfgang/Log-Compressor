using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Wolfgang.LogCompressor.Command;
using Wolfgang.LogCompressor.Service;

namespace Wolfgang.LogCompressor.Tests.Unit.Command;

public sealed class LockFailureConsoleTests : IDisposable
{
    private readonly TempDirectory _temp = new();
    private readonly IConsole _console = Substitute.For<IConsole>();
    private readonly StringWriter _error = new();



    public LockFailureConsoleTests()
    {
        _console.Error.Returns(_error);
    }



    [Fact]
    public async Task ReportAsync_when_directoryNotFound_expected_sourceNotFoundAndApplicationError()
    {
        // #447: a typo in the path is not "another instance" (exit 2).
        var source = Path.Combine(_temp.Path, "nope", "missing.log");
        using var processLock = new ProcessLock(ProcessLock.LockDirectoryFor(source), NullLogger.Instance);
        Assert.False(processLock.TryAcquire());

        var exitCode = await LockFailureConsole.ReportAsync(_console, processLock, source);

        Assert.Equal(ExitCode.ApplicationError, exitCode);
        Assert.Contains($"Source path not found: {source}", _error.ToString(), StringComparison.Ordinal);
    }



    [Fact]
    public async Task ReportAsync_when_cannotCreate_expected_noLockHintAndApplicationError()
    {
        Directory.CreateDirectory(Path.Combine(_temp.Path, ProcessLock.LockFileName));
        using var processLock = new ProcessLock(_temp.Path, NullLogger.Instance);
        Assert.False(processLock.TryAcquire());

        var exitCode = await LockFailureConsole.ReportAsync(_console, processLock, _temp.Path);

        Assert.Equal(ExitCode.ApplicationError, exitCode);
        Assert.Contains("--no-lock", _error.ToString(), StringComparison.Ordinal);
    }



    [Fact]
    public async Task ReportAsync_when_alreadyHeld_expected_anotherInstanceAndAlreadyRunning()
    {
        await using var held = new FileStream(Path.Combine(_temp.Path, ProcessLock.LockFileName), FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
        using var processLock = new ProcessLock(_temp.Path, NullLogger.Instance);
        Assert.False(processLock.TryAcquire());

        var exitCode = await LockFailureConsole.ReportAsync(_console, processLock, _temp.Path);

        Assert.Equal(ExitCode.AlreadyRunning, exitCode);
        Assert.Contains("Another instance", _error.ToString(), StringComparison.Ordinal);
    }



    public void Dispose()
    {
        _error.Dispose();
        _temp.Dispose();
    }
}
