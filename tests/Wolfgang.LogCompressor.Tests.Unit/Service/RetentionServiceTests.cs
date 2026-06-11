using Microsoft.Extensions.Logging;
using NSubstitute;
using Wolfgang.LogCompressor.Abstraction;
using Wolfgang.LogCompressor.Service;

namespace Wolfgang.LogCompressor.Tests.Unit.Service;

public sealed class RetentionServiceTests
{
    private readonly IFileSystem _fileSystem = Substitute.For<IFileSystem>();
    private readonly RetentionService _sut;



    public RetentionServiceTests()
    {
        _sut = new RetentionService(_fileSystem, Substitute.For<ILogger<RetentionService>>());
    }



    [Theory]
    [InlineData("archive.zip", true)]
    [InlineData("archive.gz", true)]
    [InlineData("archive.br", true)]
    [InlineData("archive.zst", true)]
    [InlineData("archive.lz4", true)]
    [InlineData("archive.tar.gz", true)]
    [InlineData("archive.tar.br", true)]
    [InlineData("archive.tar.zst", true)]
    [InlineData("archive.tar.lz4", true)]
    [InlineData("archive.ZIP", true)]
    [InlineData("archive.GZ", true)]
    [InlineData("app.log", false)]
    [InlineData("data.csv", false)]
    [InlineData("readme.txt", false)]
    [InlineData("archive.exe", false)]
    public void IsArchiveFile_when_variousExtensions_expected_correctResult(string fileName, bool expected)
    {
        Assert.Equal(expected, RetentionService.IsArchiveFile(fileName));
    }



    [Fact]
    public void DeleteOldArchives_when_directoryDoesNotExist_expected_returnsZero()
    {
        _fileSystem.DirectoryExists("/nonexistent").Returns(returnThis: false);

        var result = _sut.DeleteOldArchives("/nonexistent", 30);

        Assert.Equal(0, result);
    }



    [Fact]
    public void DeleteOldArchives_when_oldArchivesExist_expected_deletesOldOnes()
    {
        var dir = "/tmp/archives";
        var oldArchive = "/tmp/archives/old.zip";
        var newArchive = "/tmp/archives/new.zip";

        _fileSystem.DirectoryExists(dir).Returns(returnThis: true);
        _fileSystem.EnumerateFiles(dir, "*", SearchOption.TopDirectoryOnly)
            .Returns([oldArchive, newArchive]);

        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        var oldTempPath = Path.Combine(tempDir, "old.zip");
        File.WriteAllText(oldTempPath, "old");
        File.SetLastWriteTime(oldTempPath, DateTime.Today.AddDays(-60));
        var oldFi = new FileInfo(oldTempPath);

        var newTempPath = Path.Combine(tempDir, "new.zip");
        File.WriteAllText(newTempPath, "new");
        File.SetLastWriteTime(newTempPath, DateTime.Today.AddDays(-1));
        var newFi = new FileInfo(newTempPath);

        _fileSystem.GetFileInfo(oldArchive).Returns(oldFi);
        _fileSystem.GetFileInfo(newArchive).Returns(newFi);

        var result = _sut.DeleteOldArchives(dir, 30);

        Assert.Equal(1, result);
        _fileSystem.Received(1).DeleteFile(oldFi.FullName);
        _fileSystem.DidNotReceive().DeleteFile(newFi.FullName);
    }



    [Fact]
    public void DeleteOldArchives_when_nonArchiveFiles_expected_skipped()
    {
        var dir = "/tmp/archives";
        var logFile = "/tmp/archives/old.log";

        _fileSystem.DirectoryExists(dir).Returns(returnThis: true);
        _fileSystem.EnumerateFiles(dir, "*", SearchOption.TopDirectoryOnly).Returns([logFile]);

        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        var tempPath = Path.Combine(tempDir, "old.log");
        File.WriteAllText(tempPath, "log content");
        File.SetLastWriteTime(tempPath, DateTime.Today.AddDays(-60));
        var fi = new FileInfo(tempPath);

        _fileSystem.GetFileInfo(logFile).Returns(fi);

        var result = _sut.DeleteOldArchives(dir, 30);

        Assert.Equal(0, result);
        _fileSystem.DidNotReceive().DeleteFile(Arg.Any<string>());
    }



    [Fact]
    public void DeleteOldArchives_when_nullDirectory_expected_throwsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _sut.DeleteOldArchives(null!, 30));
    }



    [Fact]
    public void DeleteOldArchives_when_emptyDirectory_expected_throwsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => _sut.DeleteOldArchives("", 30));
    }



    [Fact]
    public void DeleteOldArchives_when_noFiles_expected_returnsZero()
    {
        var dir = "/tmp/archives";

        _fileSystem.DirectoryExists(dir).Returns(returnThis: true);
        _fileSystem.EnumerateFiles(dir, "*", SearchOption.TopDirectoryOnly).Returns([]);

        var result = _sut.DeleteOldArchives(dir, 30);

        Assert.Equal(0, result);
    }
}
