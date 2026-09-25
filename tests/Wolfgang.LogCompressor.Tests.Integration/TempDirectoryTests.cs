namespace Wolfgang.LogCompressor.Tests.Integration;

/// <summary>
/// Tests for the integration-test <see cref="TempDirectory"/> helper itself,
/// so the fixture every integration test relies on is held to the same
/// coverage bar as the rest of the test code.
/// </summary>
public sealed class TempDirectoryTests
{
    [Fact]
    public void Constructor_when_created_expected_directoryExistsUnderTempPath()
    {
        using var sut = new TempDirectory();

        Assert.True(Directory.Exists(sut.Path));
        Assert.StartsWith(Path.GetTempPath(), sut.Path);
    }



    [Fact]
    public void WriteFile_when_called_expected_fileWithContent()
    {
        using var sut = new TempDirectory();

        var path = sut.WriteFile("sample.log", "hello");

        Assert.Equal("hello", File.ReadAllText(path));
    }



    [Fact]
    public void Dispose_when_called_expected_directoryDeleted()
    {
        var sut = new TempDirectory();
        sut.WriteFile("sample.log", "hello");

        sut.Dispose();

        Assert.False(Directory.Exists(sut.Path));
    }



    [Fact]
    public void Dispose_when_directoryAlreadyDeleted_expected_noThrow()
    {
        var sut = new TempDirectory();
        Directory.Delete(sut.Path, recursive: true);

        sut.Dispose();

        Assert.False(Directory.Exists(sut.Path));
    }



    [Fact]
    public void Dispose_when_fileInUse_expected_swallowedAndCleanedUpLater()
    {
        // Windows enforces mandatory file locking, so an open handle makes
        // Directory.Delete throw IOException — the best-effort catch path.
        // Linux/macOS allow deleting open files, so the path is unreachable
        // there and this test only asserts the happy path instead.
        var sut = new TempDirectory();
        var filePath = sut.WriteFile("locked.log", "in use");

        using (File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
        {
            sut.Dispose();
        }

        // Whether or not the delete succeeded above, disposing again after the
        // handle is released must remove the directory.
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
