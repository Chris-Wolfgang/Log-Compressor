namespace Wolfgang.LogCompressor.Tests.Unit;

/// <summary>
/// Tests for the unit-test <see cref="TempDirectory"/> helper itself, so the
/// fixture every file-system test relies on is held to the same coverage bar
/// as the rest of the test code.
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
    public void CreateSubdirectory_when_calledTwice_expected_twoDistinctDirectories()
    {
        using var sut = new TempDirectory();

        var first = sut.CreateSubdirectory();
        var second = sut.CreateSubdirectory();

        Assert.True(Directory.Exists(first));
        Assert.True(Directory.Exists(second));
        Assert.NotEqual(first, second);
        Assert.StartsWith(sut.Path, first);
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
    public void Dispose_when_readOnlyFileInside_expected_swallowedAndCleanedUpLater()
    {
        // On Windows a read-only file makes Directory.Delete throw
        // UnauthorizedAccessException — the second best-effort catch path.
        var sut = new TempDirectory();
        var filePath = sut.WriteFile("readonly.log", "keep");
        File.SetAttributes(filePath, FileAttributes.ReadOnly);

        sut.Dispose();

        // On Linux/macOS the first dispose already deleted the tree.
        if (File.Exists(filePath))
        {
            File.SetAttributes(filePath, FileAttributes.Normal);
        }

        sut.Dispose();
        Assert.False(Directory.Exists(sut.Path));
    }
}
