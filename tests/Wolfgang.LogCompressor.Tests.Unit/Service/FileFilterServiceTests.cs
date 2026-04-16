using Wolfgang.LogCompressor.Service;

namespace Wolfgang.LogCompressor.Tests.Unit.Service;

public sealed class FileFilterServiceTests
{
    private readonly FileFilterService _sut = new();



    [Fact]
    public void Apply_when_noFiltersSpecified_expected_allFilesReturned()
    {
        var today = DateTime.Today;
        var files = CreateFiles
        (
            today.AddDays(-1),
            today.AddDays(-10),
            today.AddDays(-30)
        );

        var result = _sut.Apply(files, null, null, null);

        Assert.Equal(3, result.Count);
    }



    [Fact]
    public void Apply_when_olderThanDays_expected_onlyOlderFilesReturned()
    {
        var today = DateTime.Today;
        var files = CreateFiles
        (
            today.AddDays(-1),
            today.AddDays(-10),
            today.AddDays(-30)
        );

        var result = _sut.Apply(files, olderThanDays: 7, null, null);

        Assert.Equal(2, result.Count);
        Assert.All(result, f => Assert.True(f.LastWriteTime < today.AddDays(-7)));
    }



    [Fact]
    public void Apply_when_minDateTimeSet_expected_filesOnOrAfterReturned()
    {
        var today = DateTime.Today;
        var threshold = today.AddDays(-15);
        var files = CreateFiles
        (
            today.AddDays(-1),
            today.AddDays(-10),
            today.AddDays(-30)
        );

        var result = _sut.Apply(files, null, minDateTime: threshold, null);

        Assert.Equal(2, result.Count);
        Assert.All(result, f => Assert.True(f.LastWriteTime >= threshold));
    }



    [Fact]
    public void Apply_when_maxDateTimeSet_expected_filesOnOrBeforeReturned()
    {
        var today = DateTime.Today;
        var threshold = today.AddDays(-5);
        var files = CreateFiles
        (
            today.AddDays(-1),
            today.AddDays(-10),
            today.AddDays(-30)
        );

        var result = _sut.Apply(files, null, null, maxDateTime: threshold);

        Assert.Equal(2, result.Count);
        Assert.All(result, f => Assert.True(f.LastWriteTime <= threshold));
    }



    [Fact]
    public void Apply_when_minAndMaxDateTimeSet_expected_filesInRangeReturned()
    {
        var today = DateTime.Today;
        var min = today.AddDays(-20);
        var max = today.AddDays(-5);
        var files = CreateFiles
        (
            today.AddDays(-1),
            today.AddDays(-10),
            today.AddDays(-30)
        );

        var result = _sut.Apply(files, null, minDateTime: min, maxDateTime: max);

        Assert.Single(result);
        Assert.True(result[0].LastWriteTime >= min);
        Assert.True(result[0].LastWriteTime <= max);
    }



    [Fact]
    public void Apply_when_noFilesMatchFilter_expected_emptyListReturned()
    {
        var today = DateTime.Today;
        var files = CreateFiles(today.AddDays(-1));

        var result = _sut.Apply(files, olderThanDays: 30, null, null);

        Assert.Empty(result);
    }



    [Fact]
    public void Apply_when_emptyInput_expected_emptyListReturned()
    {
        var result = _sut.Apply([], null, null, null);

        Assert.Empty(result);
    }



    [Fact]
    public void Apply_when_nullFiles_expected_throwsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _sut.Apply(null!, null, null, null));
    }



    [Fact]
    public void Apply_when_fileModifiedExactlyAtMinDateTime_expected_fileIncluded()
    {
        var exactTime = new DateTime(2026, 6, 15, 12, 0, 0);
        var files = CreateFiles(exactTime);

        var result = _sut.Apply(files, null, minDateTime: exactTime, null);

        Assert.Single(result);
    }



    [Fact]
    public void Apply_when_fileModifiedExactlyAtMaxDateTime_expected_fileIncluded()
    {
        var exactTime = new DateTime(2026, 6, 15, 12, 0, 0);
        var files = CreateFiles(exactTime);

        var result = _sut.Apply(files, null, null, maxDateTime: exactTime);

        Assert.Single(result);
    }



    [Fact]
    public void Apply_when_fileModifiedExactlyAtOlderThanThreshold_expected_fileExcluded()
    {
        var today = DateTime.Today;
        var threshold = today.AddDays(-7);
        var files = CreateFiles(threshold);

        var result = _sut.Apply(files, olderThanDays: 7, null, null);

        Assert.Empty(result);
    }



    [Fact]
    public void Apply_when_includePattern_expected_onlyMatchingFilesReturned()
    {
        var files = CreateNamedFiles("app.log", "error.log", "data.csv", "readme.txt");

        var result = _sut.Apply(files, null, null, null, includePatterns: ["*.log"]);

        Assert.Equal(2, result.Count);
        Assert.All(result, f => Assert.EndsWith(".log", f.Name));
    }



    [Fact]
    public void Apply_when_excludePattern_expected_matchingFilesExcluded()
    {
        var files = CreateNamedFiles("app.log", "error.log", "data.csv", "readme.txt");

        var result = _sut.Apply(files, null, null, null, excludePatterns: ["*.csv"]);

        Assert.Equal(3, result.Count);
        Assert.DoesNotContain(result, f => f.Name.EndsWith(".csv", StringComparison.OrdinalIgnoreCase));
    }



    [Fact]
    public void Apply_when_includeAndExcludePatterns_expected_bothApplied()
    {
        var files = CreateNamedFiles("app.log", "error.log", "debug.log", "data.csv");

        var result = _sut.Apply
        (
            files,
            null,
            null,
            null,
            includePatterns: ["*.log"],
            excludePatterns: ["debug.*"]
        );

        Assert.Equal(2, result.Count);
        Assert.Contains(result, f => f.Name == "app.log");
        Assert.Contains(result, f => f.Name == "error.log");
        Assert.DoesNotContain(result, f => f.Name == "debug.log");
    }



    [Fact]
    public void Apply_when_multipleIncludePatterns_expected_unionOfMatches()
    {
        var files = CreateNamedFiles("app.log", "data.csv", "readme.txt", "config.json");

        var result = _sut.Apply(files, null, null, null, includePatterns: ["*.log", "*.csv"]);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, f => f.Name == "app.log");
        Assert.Contains(result, f => f.Name == "data.csv");
    }



    [Fact]
    public void Apply_when_excludePatternMatchesAll_expected_emptyResult()
    {
        var files = CreateNamedFiles("app.log", "error.log");

        var result = _sut.Apply(files, null, null, null, excludePatterns: ["*.log"]);

        Assert.Empty(result);
    }



    [Fact]
    public void Apply_when_emptyIncludePatterns_expected_allFilesReturned()
    {
        var files = CreateNamedFiles("app.log", "data.csv");

        var result = _sut.Apply(files, null, null, null, includePatterns: []);

        Assert.Equal(2, result.Count);
    }



    [Fact]
    public void Apply_when_nullIncludePatterns_expected_allFilesReturned()
    {
        var files = CreateNamedFiles("app.log", "data.csv");

        var result = _sut.Apply(files, null, null, null, includePatterns: null);

        Assert.Equal(2, result.Count);
    }



    private static List<FileInfo> CreateFiles(params DateTime[] lastWriteTimes)
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        var files = new List<FileInfo>();

        for (var i = 0; i < lastWriteTimes.Length; i++)
        {
            var path = Path.Combine(tempDir, $"file{i}.log");
            File.WriteAllText(path, $"test content {i}");
            File.SetLastWriteTime(path, lastWriteTimes[i]);
            files.Add(new FileInfo(path));
        }

        return files;
    }



    private static List<FileInfo> CreateNamedFiles(params string[] fileNames)
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        var files = new List<FileInfo>();

        foreach (var name in fileNames)
        {
            var path = Path.Combine(tempDir, name);
            File.WriteAllText(path, "test content");
            files.Add(new FileInfo(path));
        }

        return files;
    }
}
