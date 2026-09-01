using Wolfgang.LogCompressor.Model;
using Wolfgang.LogCompressor.Service;

namespace Wolfgang.LogCompressor.Tests.Unit.Service;

public sealed class FileNamingServiceTests : IDisposable
{
    private readonly TempDirectory _tempDir = new();
    private readonly FileNamingService _sut = new();



    [Fact]
    public void GetCompressedFileName_when_validFile_expected_formattedNameWithTimestamp()
    {
        var tempPath = CreateTempFileWithWriteTime(new DateTime(2026, 3, 15, 23, 0, 15));
        var file = new FileInfo(tempPath);

        var result = _sut.GetCompressedFileName(file, "zip");

        var expectedName = $"{Path.GetFileNameWithoutExtension(file.Name)}-2026-03-15_23-00-15.zip";
        Assert.Equal(expectedName, result);
    }



    [Fact]
    public void GetCompressedFileName_when_fileWithMultipleDots_expected_correctBaseName()
    {
        var tempDir = _tempDir.CreateSubdirectory();
        var tempPath = Path.Combine(tempDir, "my.app.log");
        File.WriteAllText(tempPath, "content");
        File.SetLastWriteTime(tempPath, new DateTime(2026, 1, 1, 12, 0, 0));
        var file = new FileInfo(tempPath);

        var result = _sut.GetCompressedFileName(file, "gz");

        Assert.Equal("my.app-2026-01-01_12-00-00.gz", result);
    }



    [Fact]
    public void GetCompressedFileName_when_nullFile_expected_throwsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _sut.GetCompressedFileName(null!, "zip"));
    }



    [Fact]
    public void GetBundleFileName_when_multipleFiles_expected_minMaxDateRange()
    {
        var files = new List<FileInfo>
        {
            CreateFileInfo(new DateTime(2026, 3, 15, 23, 0, 15)),
            CreateFileInfo(new DateTime(2026, 3, 18, 10, 30, 0)),
            CreateFileInfo(new DateTime(2026, 3, 22, 23, 13, 10))
        };

        var result = _sut.GetBundleFileName("MyApp", files, "zip");

        Assert.Equal("MyApp-2026-03-15_23-00-15 to 2026-03-22_23-13-10.zip", result);
    }



    [Fact]
    public void GetBundleFileName_when_singleFile_expected_sameDateForMinAndMax()
    {
        var files = new List<FileInfo>
        {
            CreateFileInfo(new DateTime(2026, 6, 1, 8, 0, 0))
        };

        var result = _sut.GetBundleFileName("Logs", files, "tar.gz");

        Assert.Equal("Logs-2026-06-01_08-00-00 to 2026-06-01_08-00-00.tar.gz", result);
    }



    [Fact]
    public void GetBundleFileName_when_emptyFiles_expected_throwsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => _sut.GetBundleFileName("Logs", [], "zip"));
    }



    [Fact]
    public void GetBundleFileName_when_nullFolderName_expected_throwsArgumentNullException()
    {
        var files = new List<FileInfo> { CreateFileInfo(DateTime.Now) };

        Assert.Throws<ArgumentNullException>(() => _sut.GetBundleFileName(null!, files, "zip"));
    }



    [Fact]
    public void GetBundleFileName_when_nullFiles_expected_throwsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _sut.GetBundleFileName("Logs", null!, "zip"));
    }



    [Theory]
    [InlineData("My App Logs")]
    [InlineData("App[Prod]")]
    [InlineData("App.v2.0")]
    public void GetBundleFileName_when_specialCharactersInFolderName_expected_includesNameInOutput(string folderName)
    {
        var files = new List<FileInfo>
        {
            CreateFileInfo(new DateTime(2026, 1, 1, 0, 0, 0))
        };

        var result = _sut.GetBundleFileName(folderName, files, "zip");

        Assert.StartsWith(folderName, result);
        Assert.EndsWith(".zip", result);
    }



    [Fact]
    public void GetCompressedFileName_when_nullExtension_expected_throwsArgumentNullException()
    {
        var file = CreateFileInfo(DateTime.Now);

        Assert.Throws<ArgumentNullException>(() => _sut.GetCompressedFileName(file, null!));
    }



    [Fact]
    public void GetCompressedFileName_when_emptyExtension_expected_throwsArgumentException()
    {
        var file = CreateFileInfo(DateTime.Now);

        Assert.Throws<ArgumentException>(() => _sut.GetCompressedFileName(file, ""));
    }



    [Fact]
    public void GetBundleFileName_when_nullExtension_expected_throwsArgumentNullException()
    {
        var files = new List<FileInfo> { CreateFileInfo(DateTime.Now) };

        Assert.Throws<ArgumentNullException>(() => _sut.GetBundleFileName("Logs", files, null!));
    }



    [Fact]
    public void GetCompressedFileName_when_namePrefix_expected_prefixReplacesBaseName()
    {
        var file = new FileInfo(CreateTempFileWithWriteTime(new DateTime(2026, 1, 1, 12, 0, 0)));

        var result = _sut.GetCompressedFileName(file, "zip", namePrefix: "weblogs");

        Assert.Equal("weblogs-2026-01-01_12-00-00.zip", result);
    }



    [Fact]
    public void GetCompressedFileName_when_compressedTimestamp_expected_nowShapedName()
    {
        var file = new FileInfo(CreateTempFileWithWriteTime(new DateTime(2020, 1, 1)));

        var result = _sut.GetCompressedFileName(file, "gz", TimestampSource.Compressed);

        // The job time, not the mtime: shape-asserted since "now" moves.
        Assert.Matches(@"^test-\d{4}-\d{2}-\d{2}_\d{2}-\d{2}-\d{2}\.gz$", result);
        Assert.DoesNotContain("2020-01-01_00-00-00", result, StringComparison.Ordinal);
    }



    [Fact]
    public void GetCompressedFileName_when_compressedTimestamp_expected_stableWithinRun()
    {
        var first = new FileInfo(CreateTempFileWithWriteTime(new DateTime(2020, 1, 1)));
        var second = new FileInfo(CreateTempFileWithWriteTime(new DateTime(2021, 6, 15)));

        var nameA = _sut.GetCompressedFileName(first, "gz", TimestampSource.Compressed, namePrefix: "run");
        var nameB = _sut.GetCompressedFileName(second, "gz", TimestampSource.Compressed, namePrefix: "run");

        // One run, one job time: both archives embed the SAME timestamp even
        // if the two calls straddle a second boundary.
        Assert.Equal(nameA, nameB);
    }



    [Fact]
    public void GetBundleFileName_when_compressedTimestamp_expected_singleTimestampNoRange()
    {
        var files = new List<FileInfo> { CreateFileInfo(new DateTime(2020, 1, 1)) };

        var result = _sut.GetBundleFileName("MyApp", files, "zip", TimestampSource.Compressed, namePrefix: "rollup");

        Assert.Matches(@"^rollup-\d{4}-\d{2}-\d{2}_\d{2}-\d{2}-\d{2}\.zip$", result);
        Assert.DoesNotContain(" to ", result, StringComparison.Ordinal);
    }



    private string CreateTempFileWithWriteTime(DateTime lastWriteTime)
    {
        var tempDir = _tempDir.CreateSubdirectory();
        var path = Path.Combine(tempDir, "test.log");
        File.WriteAllText(path, "content");
        File.SetLastWriteTime(path, lastWriteTime);
        return path;
    }



    private FileInfo CreateFileInfo(DateTime lastWriteTime)
    {
        var path = CreateTempFileWithWriteTime(lastWriteTime);
        return new FileInfo(path);
    }



    public void Dispose()
    {
        _tempDir.Dispose();
    }
}
