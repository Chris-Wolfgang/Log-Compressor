using Xunit;

namespace Wolfgang.LogCompressor.Tests.Fuzz;

/// <summary>
/// Covers the fuzz project's own <see cref="TempDirectory"/> helper, including
/// its best-effort dispose catches, so the fixture is held to the same
/// coverage bar as everything else.
/// </summary>
public sealed class TempDirectoryTests
{
    [Fact]
    public void Dispose_when_called_expected_directoryDeleted()
    {
        var sut = new TempDirectory();
        File.WriteAllText(Path.Combine(sut.Path, "sample.log"), "hello");

        sut.Dispose();

        Assert.False(Directory.Exists(sut.Path));
    }



    [Fact]
    public void Dispose_when_fileInUse_expected_swallowedAndCleanedUpLater()
    {
        // Windows mandatory locking makes Directory.Delete throw IOException
        // while the handle is open; Linux/macOS delete open files, so there
        // this asserts the happy path instead.
        var sut = new TempDirectory();
        var filePath = Path.Combine(sut.Path, "locked.log");
        File.WriteAllText(filePath, "in use");

        using (File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
        {
            sut.Dispose();
        }

        sut.Dispose();
        Assert.False(Directory.Exists(sut.Path));
    }



    [Fact]
    public void Dispose_when_deleteIsBlocked_expected_swallowedAndCleanedUpLater()
    {
        var sut = new TempDirectory();
        File.WriteAllText(Path.Combine(sut.Path, "keep.log"), "keep");

        // Marking the DIRECTORY read-only blocks the delete on every platform, so
        // this exercises the catch without an `if (OperatingSystem...)` - which
        // would leave the untaken branch uncovered on the other platform, the very
        // problem this test used to have. Windows raises IOException for a
        // read-only directory; Unix clears the directory's write bit, so its
        // entries cannot be unlinked (EACCES -> UnauthorizedAccessException).
        File.SetAttributes(sut.Path, FileAttributes.ReadOnly);

        sut.Dispose();

        // The failure was swallowed and the tree is still there.
        Assert.True(Directory.Exists(sut.Path));

        // Clear only the read-only bit, leaving the rest of the attributes alone.
        File.SetAttributes(sut.Path, File.GetAttributes(sut.Path) & ~FileAttributes.ReadOnly);

        sut.Dispose();
        Assert.False(Directory.Exists(sut.Path));
    }
}
