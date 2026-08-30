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
    public void Dispose_when_readOnlyFileInside_expected_swallowedAndCleanedUpLater()
    {
        // On Windows a read-only file makes Directory.Delete throw
        // UnauthorizedAccessException — the second best-effort catch path.
        var sut = new TempDirectory();
        var filePath = Path.Combine(sut.Path, "readonly.log");
        File.WriteAllText(filePath, "keep");
        File.SetAttributes(filePath, FileAttributes.ReadOnly);

        sut.Dispose();

        if (File.Exists(filePath))
        {
            File.SetAttributes(filePath, FileAttributes.Normal);
        }

        sut.Dispose();
        Assert.False(Directory.Exists(sut.Path));
    }
}
