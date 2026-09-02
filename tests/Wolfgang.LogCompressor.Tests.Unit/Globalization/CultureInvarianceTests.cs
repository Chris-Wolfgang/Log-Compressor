using System.Globalization;
using McMaster.Extensions.CommandLineUtils;
using NSubstitute;
using Wolfgang.LogCompressor.Command;
using Wolfgang.LogCompressor.Model;
using Wolfgang.LogCompressor.Service;

namespace Wolfgang.LogCompressor.Tests.Unit.Globalization;

/// <summary>
/// Asserts the culture-invariance contract (fleet issue #83): every output the
/// tool produces — archive names, bundle names, report files — is byte-identical
/// under hostile cultures (Turkish dotted-I, German decimal comma, Chinese
/// collation, Arabic Umm al-Qura calendar + digit shapes, Japanese). The one
/// documented exception is <c>--min-datetime</c>/<c>--max-datetime</c>, which
/// parse with the operator's local culture by design (ADR-0004).
/// </summary>
public sealed class CultureInvarianceTests : IDisposable
{
    private readonly TempDirectory _tempDir = new();



    public static TheoryData<string> HostileCultures =>
        new("en-US", "tr-TR", "de-DE", "zh-CN", "ar-SA", "ja-JP");



    private static void RunUnder(string cultureName, Action action)
    {
        var originalCulture = CultureInfo.CurrentCulture;
        var originalUiCulture = CultureInfo.CurrentUICulture;

        try
        {
            var culture = new CultureInfo(cultureName);
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
            action();
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
            CultureInfo.CurrentUICulture = originalUiCulture;
        }
    }



    private static async Task RunUnderAsync(string cultureName, Func<Task> action)
    {
        var originalCulture = CultureInfo.CurrentCulture;
        var originalUiCulture = CultureInfo.CurrentUICulture;

        try
        {
            // CurrentCulture flows with the ExecutionContext, so it survives
            // awaits inside the action and the finally restores it afterwards.
            var culture = new CultureInfo(cultureName);
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
            await action();
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
            CultureInfo.CurrentUICulture = originalUiCulture;
        }
    }



    [Theory]
    [MemberData(nameof(HostileCultures))]
    public void GetCompressedFileName_when_hostileCulture_expected_invariantTimestamp(string cultureName)
    {
        // ar-SA's default calendar is Umm al-Qura — without an invariant
        // format this would embed a Hijri year (e.g. 1447) in the name.
        var path = _tempDir.WriteFile("app.log", "content");
        File.SetLastWriteTime(path, new DateTime(2026, 1, 5, 9, 30, 0));
        var file = new FileInfo(path);
        var sut = new FileNamingService();

        RunUnder(cultureName, () =>
        {
            var result = sut.GetCompressedFileName(file, "zip");

            Assert.Equal("app-2026-01-05_09-30-00.zip", result);
        });
    }



    [Theory]
    [MemberData(nameof(HostileCultures))]
    public void GetBundleFileName_when_hostileCulture_expected_invariantTimestamps(string cultureName)
    {
        var first = _tempDir.WriteFile("a.log", "1");
        File.SetLastWriteTime(first, new DateTime(2026, 3, 15, 23, 0, 15));
        var second = _tempDir.WriteFile("b.log", "2");
        File.SetLastWriteTime(second, new DateTime(2026, 3, 22, 23, 13, 10));
        var files = new List<FileInfo> { new(first), new(second) };
        var sut = new FileNamingService();

        RunUnder(cultureName, () =>
        {
            var result = sut.GetBundleFileName("MyApp", files, "zip");

            Assert.Equal
            (
                "MyApp-2026-03-15_23-00-15 to 2026-03-22_23-13-10.zip",
                result
            );
        });
    }



    [Theory]
    [MemberData(nameof(HostileCultures))]
    public async Task WriteReportAsync_when_csvUnderHostileCulture_expected_invariantContent(string cultureName)
    {
        var sut = new ReportService();
        var reportPath = Path.Combine(_tempDir.Path, $"report-{cultureName}.csv");
        var results = new List<CompressionResult>
        {
            new()
            {
                SourcePath = "/logs/app.log",
                OutputPath = "/logs/app.zip",
                OriginalSize = 1234567890123,
                CompressedSize = 98765432109,
                Success = true
            }
        };

        var content = string.Empty;

        await RunUnderAsync(cultureName, async () =>
        {
            await sut.WriteReportAsync(results, "csv", reportPath, TimeSpan.FromMinutes(5));
            content = await File.ReadAllTextAsync(reportPath);
        });

        // Large longs would betray group separators or native digit shaping.
        Assert.Contains(",1234567890123,98765432109,", content, StringComparison.Ordinal);
    }



    [Theory]
    [MemberData(nameof(HostileCultures))]
    public async Task WriteReportAsync_when_jsonUnderHostileCulture_expected_invariantNumbers(string cultureName)
    {
        var sut = new ReportService();
        var reportPath = Path.Combine(_tempDir.Path, $"report-{cultureName}.json");
        var results = new List<CompressionResult>
        {
            new()
            {
                SourcePath = "/logs/app.log",
                OutputPath = "/logs/app.zip",
                OriginalSize = 1234567890123,
                CompressedSize = 98765432109,
                Success = true
            }
        };

        var content = string.Empty;

        await RunUnderAsync(cultureName, async () =>
        {
            await sut.WriteReportAsync(results, "json", reportPath, TimeSpan.FromMinutes(5));
            content = await File.ReadAllTextAsync(reportPath);
        });

        Assert.Contains("\"originalSizeBytes\": 1234567890123", content, StringComparison.Ordinal);
        Assert.Contains("\"duration\": \"00:05:00\"", content, StringComparison.Ordinal);
    }



    [Theory]
    [MemberData(nameof(HostileCultures))]
    public void BuildOptions_when_formatUppercaseUnderHostileCulture_expected_parsed(string cultureName)
    {
        // tr-TR is the interesting row: a culture-sensitive lowercase of "ZIP"
        // would produce "zıp" (dotless ı) and fail the format switch.
        RunUnder(cultureName, () =>
        {
            var options = new TestOptions
            {
                Path = "/tmp/logs",
                Format = "ZIP"
            };

            var result = options.BuildOptions();

            Assert.Equal(CompressionFormat.Zip, result.Format);
        });
    }



    [Fact]
    public void ValidateOptions_when_isoDateUnderGregorianCultures_expected_valid()
    {
        // Documented allowlist (ADR-0004): the datetime flags parse with the
        // operator's culture. ISO 8601 works under every Gregorian-calendar
        // culture in the matrix.
        foreach (var cultureName in new[] { "en-US", "tr-TR", "de-DE", "zh-CN", "ja-JP" })
        {
            RunUnder(cultureName, () =>
            {
                var console = Substitute.For<IConsole>();
                console.Error.Returns(new StringWriter());

                var options = new TestOptions
                {
                    Path = "/tmp/logs",
                    MinDateTime = "2026-01-15"
                };

                Assert.True(options.ValidateOptions(console));
                Assert.Equal(new DateTime(2026, 1, 15), options.BuildOptions().MinDateTime);
            });
        }
    }



    [Fact]
    public void ValidateOptions_when_isoDateUnderUmAlQuraCalendar_expected_rejected()
    {
        // The known limitation of culture-aware parsing: ar-SA's Umm al-Qura
        // calendar can't represent Gregorian year 2026, so even ISO 8601 input
        // is rejected there. Recorded in ADR-0004; an ar-SA operator supplies
        // dates in the local calendar instead.
        RunUnder("ar-SA", () =>
        {
            var console = Substitute.For<IConsole>();
            console.Error.Returns(new StringWriter());

            var options = new TestOptions
            {
                Path = "/tmp/logs",
                MinDateTime = "2026-01-15"
            };

            Assert.False(options.ValidateOptions(console));
        });
    }



    public void Dispose()
    {
        _tempDir.Dispose();
    }



    private sealed class TestOptions : SharedOptions;
}
