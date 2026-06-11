using Microsoft.Extensions.Logging.Abstractions;
using Wolfgang.LogCompressor.Service;

namespace Wolfgang.LogCompressor.Tests.Integration;

/// <summary>
/// Integration tests for the real <see cref="ProcessLock"/> file-based
/// single-instance lock. These cover the "another instance is already running"
/// behaviour that the command unit tests cannot reach (the lock is constructed
/// internally and backed by a real lock file on disk).
/// </summary>
public sealed class ProcessLockTests
{
    [Fact]
    public void TryAcquire_when_secondLockOnSameDirectory_expected_false()
    {
        using var temp = new TempDirectory();

        using var first = new ProcessLock(temp.Path, NullLogger.Instance);
        Assert.True(first.TryAcquire());

        using var second = new ProcessLock(temp.Path, NullLogger.Instance);
        Assert.False(second.TryAcquire());
    }



    [Fact]
    public void TryAcquire_after_firstLockDisposed_expected_true()
    {
        using var temp = new TempDirectory();

        var first = new ProcessLock(temp.Path, NullLogger.Instance);
        Assert.True(first.TryAcquire());
        first.Dispose();

        using var second = new ProcessLock(temp.Path, NullLogger.Instance);
        Assert.True(second.TryAcquire());
    }



    [Fact]
    public void TryAcquire_when_locksOnDifferentDirectories_expected_bothSucceed()
    {
        using var tempA = new TempDirectory();
        using var tempB = new TempDirectory();

        using var lockA = new ProcessLock(tempA.Path, NullLogger.Instance);
        using var lockB = new ProcessLock(tempB.Path, NullLogger.Instance);

        Assert.True(lockA.TryAcquire());
        Assert.True(lockB.TryAcquire());
    }
}
