using Microsoft.Extensions.Logging.Abstractions;
using Wolfgang.LogCompressor.Service;

namespace Wolfgang.LogCompressor.Tests.Unit.Service;

/// <summary>
/// Unit-level checks of <see cref="ProcessLock"/>. The concurrency behaviour is covered by the
/// integration tests; these pin the guards and the release order, which the mutation run (unit
/// project only) otherwise cannot see (#303).
/// </summary>
public sealed class ProcessLockTests : IDisposable
{
    private readonly TempDirectory _temp = new();



    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void LockDirectoryFor_when_sourcePathMissing_expected_throws(string? sourcePath)
    {
        Assert.ThrowsAny<ArgumentException>(() => ProcessLock.LockDirectoryFor(sourcePath!));
    }



    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Ctor_when_directoryMissing_expected_throws(string? directory)
    {
        Assert.ThrowsAny<ArgumentException>(() => new ProcessLock(directory!, NullLogger.Instance));
    }



    [Fact]
    public void Ctor_when_loggerNull_expected_throwsWithParamName()
    {
        Assert.Equal("logger", Assert.Throws<ArgumentNullException>(() => new ProcessLock(_temp.Path, null!)).ParamName);
    }



    [Fact]
    public void Dispose_when_defaultReleaseOrder_expected_lockFileRemoved()
    {
        // The platform default must remove the lock file on every OS; on
        // Windows the unlink-while-held order cannot (the file is open).
        var sut = new ProcessLock(_temp.Path, NullLogger.Instance);
        Assert.True(sut.TryAcquire());

        sut.Dispose();

        Assert.False(File.Exists(Path.Combine(_temp.Path, ProcessLock.LockFileName)));
    }



    [Fact]
    public void Dispose_when_closeThenDelete_expected_lockFileRemoved()
    {
        var sut = new ProcessLock(_temp.Path, NullLogger.Instance, unlinkWhileHeld: false);
        Assert.True(sut.TryAcquire());

        sut.Dispose();

        Assert.False(File.Exists(Path.Combine(_temp.Path, ProcessLock.LockFileName)));
    }



    public void Dispose()
    {
        _temp.Dispose();
    }
}
