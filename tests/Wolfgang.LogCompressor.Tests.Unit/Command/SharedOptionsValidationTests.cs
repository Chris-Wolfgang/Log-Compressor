using System.IO.Compression;
using McMaster.Extensions.CommandLineUtils;
using NSubstitute;
using Wolfgang.LogCompressor.Command;
using Wolfgang.LogCompressor.Model;

namespace Wolfgang.LogCompressor.Tests.Unit.Command;

public sealed class SharedOptionsValidationTests
{
    /// <summary>
    /// Concrete test implementation of <see cref="SharedOptions"/>.
    /// </summary>
    private sealed class TestOptions : SharedOptions;



    [Fact]
    public void ValidateOptions_when_olderThanAndMinDateTime_expected_false()
    {
        var console = Substitute.For<IConsole>();
        console.Error.Returns(new StringWriter());

        var options = new TestOptions
        {
            Path = "/tmp",
            OlderThan = 7,
            MinDateTime = "2026-01-01"
        };

        Assert.False(options.ValidateOptions(console));
    }



    [Fact]
    public void ValidateOptions_when_olderThanAndMaxDateTime_expected_false()
    {
        var console = Substitute.For<IConsole>();
        console.Error.Returns(new StringWriter());

        var options = new TestOptions
        {
            Path = "/tmp",
            OlderThan = 7,
            MaxDateTime = "2026-12-31"
        };

        Assert.False(options.ValidateOptions(console));
    }



    [Fact]
    public void ValidateOptions_when_olderThanAlone_expected_true()
    {
        var console = Substitute.For<IConsole>();

        var options = new TestOptions
        {
            Path = "/tmp",
            OlderThan = 7
        };

        Assert.True(options.ValidateOptions(console));
    }



    [Fact]
    public void ValidateOptions_when_minAndMaxDateTime_expected_true()
    {
        var console = Substitute.For<IConsole>();

        var options = new TestOptions
        {
            Path = "/tmp",
            MinDateTime = "2026-01-01",
            MaxDateTime = "2026-12-31"
        };

        Assert.True(options.ValidateOptions(console));
    }



    [Fact]
    public void ValidateOptions_when_noFilters_expected_true()
    {
        var console = Substitute.For<IConsole>();

        var options = new TestOptions { Path = "/tmp" };

        Assert.True(options.ValidateOptions(console));
    }



    [Fact]
    public void ValidateOptions_when_invalidMinDateTime_expected_false()
    {
        var console = Substitute.For<IConsole>();
        console.Error.Returns(new StringWriter());

        var options = new TestOptions
        {
            Path = "/tmp",
            MinDateTime = "not-a-date"
        };

        Assert.False(options.ValidateOptions(console));
    }



    [Fact]
    public void ValidateOptions_when_invalidMaxDateTime_expected_false()
    {
        var console = Substitute.For<IConsole>();
        console.Error.Returns(new StringWriter());

        var options = new TestOptions
        {
            Path = "/tmp",
            MaxDateTime = "not-a-date"
        };

        Assert.False(options.ValidateOptions(console));
    }



    [Fact]
    public void ValidateOptions_when_invalidFormat_expected_false()
    {
        var console = Substitute.For<IConsole>();
        console.Error.Returns(new StringWriter());

        var options = new TestOptions
        {
            Path = "/tmp",
            Format = "xz"
        };

        Assert.False(options.ValidateOptions(console));
    }



    [Theory]
    [InlineData("zip")]
    [InlineData("gz")]
    [InlineData("gzip")]
    [InlineData("br")]
    [InlineData("brotli")]
    [InlineData("ZIP")]
    [InlineData("Brotli")]
    public void ValidateOptions_when_validFormat_expected_true(string format)
    {
        var console = Substitute.For<IConsole>();

        var options = new TestOptions
        {
            Path = "/tmp",
            Format = format
        };

        Assert.True(options.ValidateOptions(console));
    }



    [Fact]
    public void ValidateOptions_when_invalidLevel_expected_false()
    {
        var console = Substitute.For<IConsole>();
        console.Error.Returns(new StringWriter());

        var options = new TestOptions
        {
            Path = "/tmp",
            Level = "turbo"
        };

        Assert.False(options.ValidateOptions(console));
    }



    [Theory]
    [InlineData("fastest")]
    [InlineData("optimal")]
    [InlineData("smallest")]
    [InlineData("Fastest")]
    [InlineData("OPTIMAL")]
    public void ValidateOptions_when_validLevel_expected_true(string level)
    {
        var console = Substitute.For<IConsole>();

        var options = new TestOptions
        {
            Path = "/tmp",
            Level = level
        };

        Assert.True(options.ValidateOptions(console));
    }



    [Fact]
    public void BuildOptions_when_allFieldsSet_expected_correctMapping()
    {
        var sourcePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "logs");
        var outputPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "output");

        var options = new TestOptions
        {
            Path = sourcePath,
            Output = outputPath,
            Recurse = true,
            OlderThan = 30,
            Format = "gz",
            Level = "fastest"
        };

        var result = options.BuildOptions();

        Assert.Equal(System.IO.Path.GetFullPath(sourcePath), result.SourcePath);
        Assert.Equal(System.IO.Path.GetFullPath(outputPath), result.OutputPath);
        Assert.True(result.Recurse);
        Assert.Equal(30, result.OlderThanDays);
        Assert.Equal(CompressionFormat.Gz, result.Format);
        Assert.Equal(CompressionLevel.Fastest, result.Level);
    }



    [Fact]
    public void BuildOptions_when_dateTimesSet_expected_parsedCorrectly()
    {
        var options = new TestOptions
        {
            Path = "/tmp/logs",
            MinDateTime = "2026-01-15",
            MaxDateTime = "2026-06-30"
        };

        var result = options.BuildOptions();

        Assert.NotNull(result.MinDateTime);
        Assert.NotNull(result.MaxDateTime);
        Assert.Equal(new DateTime(2026, 1, 15), result.MinDateTime);
        Assert.Equal(new DateTime(2026, 6, 30), result.MaxDateTime);
    }



    [Fact]
    public void BuildOptions_when_defaults_expected_zipFormatNullOptionals()
    {
        var options = new TestOptions { Path = "/tmp" };

        var result = options.BuildOptions();

        Assert.Equal(CompressionFormat.Zip, result.Format);
        Assert.Equal(CompressionLevel.SmallestSize, result.Level);
        Assert.Null(result.OutputPath);
        Assert.Null(result.OlderThanDays);
        Assert.Null(result.MinDateTime);
        Assert.Null(result.MaxDateTime);
        Assert.False(result.Recurse);
    }



    [Fact]
    public void ValidateOptions_when_validReportFormat_expected_true()
    {
        var console = Substitute.For<IConsole>();

        var options = new TestOptions
        {
            Path = "/tmp",
            Report = "json"
        };

        Assert.True(options.ValidateOptions(console));
    }



    [Fact]
    public void ValidateOptions_when_invalidReportFormat_expected_false()
    {
        var console = Substitute.For<IConsole>();
        console.Error.Returns(new StringWriter());

        var options = new TestOptions
        {
            Path = "/tmp",
            Report = "xml"
        };

        Assert.False(options.ValidateOptions(console));
    }



    [Fact]
    public void ValidateOptions_when_reportPathWithoutReport_expected_false()
    {
        var console = Substitute.For<IConsole>();
        console.Error.Returns(new StringWriter());

        var options = new TestOptions
        {
            Path = "/tmp",
            ReportPath = "/tmp/report.json"
        };

        Assert.False(options.ValidateOptions(console));
    }



    [Fact]
    public void ValidateOptions_when_reportPathWithReport_expected_true()
    {
        var console = Substitute.For<IConsole>();

        var options = new TestOptions
        {
            Path = "/tmp",
            Report = "csv",
            ReportPath = "/tmp/report.csv"
        };

        Assert.True(options.ValidateOptions(console));
    }



    [Fact]
    public void ValidateOptions_when_noVerifySet_expected_true()
    {
        var console = Substitute.For<IConsole>();

        var options = new TestOptions
        {
            Path = "/tmp",
            NoVerify = true
        };

        Assert.True(options.ValidateOptions(console));
    }



    [Fact]
    public void BuildOptions_when_noVerifySet_expected_verifyFalse()
    {
        var options = new TestOptions
        {
            Path = "/tmp",
            NoVerify = true
        };

        var result = options.BuildOptions();

        Assert.False(result.Verify);
    }



    [Fact]
    public void BuildOptions_when_noVerifyNotSet_expected_verifyTrue()
    {
        var options = new TestOptions { Path = "/tmp" };

        var result = options.BuildOptions();

        Assert.True(result.Verify);
    }



    [Fact]
    public void BuildOptions_when_includePatterns_expected_mappedCorrectly()
    {
        var options = new TestOptions
        {
            Path = "/tmp",
            Include = ["*.log", "*.txt"]
        };

        var result = options.BuildOptions();

        Assert.Equal(2, result.IncludePatterns.Count);
        Assert.Contains("*.log", result.IncludePatterns);
        Assert.Contains("*.txt", result.IncludePatterns);
    }



    [Fact]
    public void BuildOptions_when_excludePatterns_expected_mappedCorrectly()
    {
        var options = new TestOptions
        {
            Path = "/tmp",
            Exclude = ["*.tmp"]
        };

        var result = options.BuildOptions();

        Assert.Single(result.ExcludePatterns);
        Assert.Contains("*.tmp", result.ExcludePatterns);
    }



    [Fact]
    public void BuildOptions_when_noPatternsSet_expected_emptyLists()
    {
        var options = new TestOptions { Path = "/tmp" };

        var result = options.BuildOptions();

        Assert.Empty(result.IncludePatterns);
        Assert.Empty(result.ExcludePatterns);
    }



    [Fact]
    public void BuildOptions_when_deleteArchivesOlderThanSet_expected_mappedCorrectly()
    {
        var options = new TestOptions
        {
            Path = "/tmp",
            DeleteArchivesOlderThan = 90
        };

        var result = options.BuildOptions();

        Assert.Equal(90, result.DeleteArchivesOlderThanDays);
    }



    [Fact]
    public void BuildOptions_when_deleteArchivesOlderThanNotSet_expected_null()
    {
        var options = new TestOptions { Path = "/tmp" };

        var result = options.BuildOptions();

        Assert.Null(result.DeleteArchivesOlderThanDays);
    }



    [Fact]
    public void BuildOptions_when_reportFormatSet_expected_mappedCorrectly()
    {
        var options = new TestOptions
        {
            Path = "/tmp",
            Report = "json",
            ReportPath = "/tmp/report.json"
        };

        var result = options.BuildOptions();

        Assert.Equal("json", result.ReportFormat);
        Assert.Equal("/tmp/report.json", result.ReportPath);
    }
}
