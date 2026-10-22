using Microsoft.Extensions.Logging;
using NSubstitute;
using Wolfgang.LogCompressor.Abstraction;
using Wolfgang.LogCompressor.Service;

namespace Wolfgang.LogCompressor.Tests.Unit.Service;

public sealed class RetentionServiceTests : IDisposable
{
    private readonly TempDirectory _tempDir = new();
    private readonly IFileSystem _fileSystem = Substitute.For<IFileSystem>();
    private readonly RetentionService _sut;



    public RetentionServiceTests()
    {
        _sut = new RetentionService(_fileSystem, Substitute.For<ILogger<RetentionService>>(), TimeProvider.System);
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
    public void Ctor_when_timeProviderNull_expected_throwsWithParamName()
    {
        Assert.Equal("timeProvider", Assert.Throws<ArgumentNullException>(() => new RetentionService(_fileSystem, Substitute.For<ILogger<RetentionService>>(), null!)).ParamName);
    }



    [Fact]
    public void DeleteOldArchives_when_directoryDoesNotExist_expected_returnsZero()
    {
        _fileSystem.DirectoryExists("/nonexistent").Returns(returnThis: false);

        var result = _sut.DeleteOldArchives("/nonexistent", 30);

        Assert.Equal(0, result);
        _fileSystem.DidNotReceiveWithAnyArgs().EnumerateFiles(default!, default!, default);
    }



    [Fact]
    public void DeleteOldArchives_when_oldArchivesExist_expected_deletesOldOnes()
    {
        var dir = "/tmp/archives";
        var oldArchive = "/tmp/archives/old-2026-01-01_00-00-00.zip";
        var newArchive = "/tmp/archives/new-2026-01-01_00-00-00.zip";

        _fileSystem.DirectoryExists(dir).Returns(returnThis: true);
        _fileSystem.EnumerateFiles(dir, "*", SearchOption.TopDirectoryOnly)
            .Returns([oldArchive, newArchive]);

        var tempDir = _tempDir.CreateSubdirectory();

        var oldTempPath = Path.Combine(tempDir, "old-2026-01-01_00-00-00.zip");
        File.WriteAllText(oldTempPath, "old");
        File.SetLastWriteTime(oldTempPath, DateTime.Today.AddDays(-60));
        var oldFi = new FileInfo(oldTempPath);

        var newTempPath = Path.Combine(tempDir, "new-2026-01-01_00-00-00.zip");
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

        var tempDir = _tempDir.CreateSubdirectory();
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



    public void Dispose()
    {
        _tempDir.Dispose();
    }

    [Fact]
    public void DeleteOldArchives_when_fileExactlyAtThreshold_expected_kept()
    {
        // Boundary contract: the cutoff is exclusive — a file whose write time
        // equals Today-N is NOT older than N days and must be kept (#176:
        // the >= vs > equality mutant survived without this case).
        var dir = "/tmp/archives";
        var archive = "/tmp/archives/edge-2026-01-01_00-00-00.zip";

        _fileSystem.DirectoryExists(dir).Returns(returnThis: true);
        _fileSystem.EnumerateFiles(dir, "*", SearchOption.TopDirectoryOnly).Returns([archive]);

        var tempDir = _tempDir.CreateSubdirectory();
        var tempPath = Path.Combine(tempDir, "edge-2026-01-01_00-00-00.zip");
        File.WriteAllText(tempPath, "edge");
        File.SetLastWriteTime(tempPath, DateTime.Today.AddDays(-30));
        var fi = new FileInfo(tempPath);

        _fileSystem.GetFileInfo(archive).Returns(fi);

        var result = _sut.DeleteOldArchives(dir, 30);

        Assert.Equal(0, result);
        _fileSystem.DidNotReceive().DeleteFile(Arg.Any<string>());
    }



    [Fact]
    public void Ctor_when_fileSystemOrLoggerNull_expected_throwsWithParamName()
    {
        var logger = Substitute.For<ILogger<RetentionService>>();

        Assert.Equal("fileSystem", Assert.Throws<ArgumentNullException>(() => new RetentionService(null!, logger, TimeProvider.System)).ParamName);
        Assert.Equal("logger", Assert.Throws<ArgumentNullException>(() => new RetentionService(_fileSystem, null!, TimeProvider.System)).ParamName);
    }



    [Fact]
    public void DeleteOldArchives_when_olderThanDaysBelowOne_expected_throwsWithParamName()
    {
        _fileSystem.DirectoryExists("/logs").Returns(returnThis: false);

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => _sut.DeleteOldArchives("/logs", 0));

        Assert.Equal("olderThanDays", ex.ParamName);
    }



    [Theory]
    [InlineData("app-2026-01-01_00-00-00.zip", true)]
    [InlineData("app-2026-01-01_00-00-00.GZ", true)]
    [InlineData("app-2026-01-01_00-00-00-2.zst", true)]
    [InlineData("my.app-2026-01-01_00-00-00.lz4", true)]
    [InlineData("MyApp-2026-01-01_00-00-00 to 2026-01-02_00-00-00.tar.gz", true)]
    [InlineData("MyApp-2026-01-01_00-00-00 to 2026-01-02_00-00-00.zip", true)]
    [InlineData("MyApp-2026-01-01_00-00-00.tar.br", true)]
    [InlineData("backup.zip", false)]
    [InlineData("photos-2026.tar.gz", false)]
    [InlineData("app-2026-01-01.zip", false)]
    [InlineData("app-2026-01-01_00-00-00.zip.partial", false)]
    [InlineData("app-2026-01-01_00-00-00.log", false)]
    [InlineData("app-2026-01-01_00-00-00.7z", false)]
    public void IsRetentionCandidate_when_noBaseName_expected_onlyLogcNamedArchives(string fileName, bool expected)
    {
        Assert.Equal(expected, RetentionService.IsRetentionCandidate(fileName, baseName: null));
    }



    [Theory]
    [InlineData("MyApp-2026-01-01_00-00-00 to 2026-01-02_00-00-00.zip", true)]
    [InlineData("myapp-2026-01-01_00-00-00.zip", true)]
    [InlineData("OtherApp-2026-01-01_00-00-00 to 2026-01-02_00-00-00.zip", false)]
    [InlineData("MyAppX-2026-01-01_00-00-00.zip", false)]
    public void IsRetentionCandidate_when_baseNameGiven_expected_onlyThatBaseName(string fileName, bool expected)
    {
        Assert.Equal(expected, RetentionService.IsRetentionCandidate(fileName, "MyApp"));
    }



    [Fact]
    public void DeleteOldArchives_when_oldUnrelatedArchive_expected_keptAndOnlyLogcArchiveDeleted()
    {
        // The blast-radius fix (#352): an old third-party archive that merely
        // shares an extension is never deleted.
        var dir = _tempDir.CreateSubdirectory();
        var unrelated = Path.Combine(dir, "backup.zip");
        var own = Path.Combine(dir, "app-2026-01-01_00-00-00.zip");
        foreach (var path in new[] { unrelated, own })
        {
            File.WriteAllText(path, "x");
            File.SetLastWriteTime(path, DateTime.Today.AddDays(-90));
            _fileSystem.GetFileInfo(path).Returns(new FileInfo(path));
        }

        _fileSystem.DirectoryExists(dir).Returns(returnThis: true);
        _fileSystem.EnumerateFiles(dir, "*", SearchOption.TopDirectoryOnly).Returns([unrelated, own]);

        var result = _sut.DeleteOldArchives(dir, 30);

        Assert.Equal(1, result);
        _fileSystem.Received(1).DeleteFile(own);
        _fileSystem.DidNotReceive().DeleteFile(unrelated);
    }



    [Fact]
    public void DeleteOldArchives_when_allDirectories_expected_subdirectoriesScanned()
    {
        var dir = _tempDir.CreateSubdirectory();
        var nested = Path.Combine(dir, "sub");
        Directory.CreateDirectory(nested);
        var archive = Path.Combine(nested, "app-2026-01-01_00-00-00.gz");
        File.WriteAllText(archive, "x");
        File.SetLastWriteTime(archive, DateTime.Today.AddDays(-90));
        _fileSystem.GetFileInfo(archive).Returns(new FileInfo(archive));
        _fileSystem.DirectoryExists(dir).Returns(returnThis: true);
        _fileSystem.EnumerateFiles(dir, "*", SearchOption.AllDirectories).Returns([archive]);

        var result = _sut.DeleteOldArchives(dir, 30, SearchOption.AllDirectories);

        Assert.Equal(1, result);
        _fileSystem.Received(1).DeleteFile(archive);
    }
}
