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



    [Fact]
    public void TryAcquire_when_leftoverFileFromCrashedRun_expected_true()
    {
        // A crashed holder leaves the file behind but no open handle. Under
        // the OpenOrCreate protocol (#172) the leftover never blocks — no
        // stale-PID parsing, no delete-based takeover.
        using var temp = new TempDirectory();
        temp.WriteFile(ProcessLock.LockFileName, $"PID={int.MaxValue}\nStarted=2020-01-01T00:00:00Z\n");

        using var sut = new ProcessLock(temp.Path, NullLogger.Instance);

        Assert.True(sut.TryAcquire());
    }



    [Fact]
    public void TryAcquire_when_fileHeldByAnotherHandle_expected_false()
    {
        // An OPEN handle is the lock, regardless of file content.
        using var temp = new TempDirectory();
        var lockPath = Path.Combine(temp.Path, ProcessLock.LockFileName);
        using var held = new FileStream(lockPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);

        using var sut = new ProcessLock(temp.Path, NullLogger.Instance);

        Assert.False(sut.TryAcquire());
    }



    [Fact]
    public void IsLockFile_when_lockFileNameAnyCase_expected_true()
    {
        // Forward slashes only: Path.GetFileName treats backslash as a
        // separator on Windows but as an ordinary character on Unix, so a
        // Windows-style literal here fails on Linux (caught by the v0.2.0
        // release PR's Stage 1 — the first Linux run of the cycle).
        Assert.True(ProcessLock.IsLockFile("/some/dir/.logc.lock"));
        Assert.True(ProcessLock.IsLockFile("/logs/.LOGC.LOCK"));
        Assert.False(ProcessLock.IsLockFile("/some/dir/app.log"));
    }
}
