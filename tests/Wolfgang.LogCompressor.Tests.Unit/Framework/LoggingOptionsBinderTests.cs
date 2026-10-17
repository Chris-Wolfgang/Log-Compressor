using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Wolfgang.LogCompressor.Framework;

namespace Wolfgang.LogCompressor.Tests.Unit.Framework;

public sealed class LoggingOptionsBinderTests
{
    private static IConfigurationSection Section(params (string Key, string? Value)[] values)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(values.Select(v => new KeyValuePair<string, string?>("Logging:" + v.Key, v.Value)))
            .Build()
            .GetSection("Logging");
    }



    // Every property of the options graph on one line, so two instances compare as text.
    private static string Describe(LoggingOptions o)
    {
        var overrides = string.Join(",", o.Overrides.OrderBy(p => p.Key, StringComparer.Ordinal).Select(p => $"{p.Key}={p.Value}"));
        return $"min={o.MinimumLevel}|ov=[{overrides}]"
            + $"|con={o.Console.Enabled},{o.Console.MinimumLevel},{o.Console.OutputTemplate}"
            + $"|file={o.File.Enabled},{o.File.Path},{o.File.MinimumLevel},{o.File.RollingInterval},{o.File.RollOnFileSizeLimit},{o.File.FileSizeLimitBytes},{o.File.OutputTemplate}";
    }



    [Fact]
    public void Bind_when_sectionNull_expected_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => LoggingOptionsBinder.Bind(null!));
    }



    [Fact]
    public void Bind_when_sectionMissing_expected_allDefaults()
    {
        var options = LoggingOptionsBinder.Bind(Section());

        Assert.Equal(Describe(new LoggingOptions()), Describe(options));
    }



    [Fact]
    public void Bind_when_shippedAppSettingsValues_expected_everyValueRead()
    {
        var options = LoggingOptionsBinder.Bind
        (
            Section
            (
                ("MinimumLevel", "Information"),
                ("Overrides:Microsoft", "Warning"),
                ("Overrides:System", "Error"),
                ("Console:Enabled", "false"),
                ("Console:MinimumLevel", "Warning"),
                ("Console:OutputTemplate", "{Message}"),
                ("File:Enabled", "false"),
                ("File:Path", "out/app-.log"),
                ("File:MinimumLevel", "Debug"),
                ("File:RollingInterval", "Hour"),
                ("File:RollOnFileSizeLimit", "false"),
                ("File:FileSizeLimitBytes", "10485760"),
                ("File:OutputTemplate", "{Level}")
            )
        );

        Assert.Equal(LogEventLevel.Information, options.MinimumLevel);
        Assert.Equal(LogEventLevel.Warning, options.Overrides["Microsoft"]);
        Assert.Equal(LogEventLevel.Error, options.Overrides["System"]);
        Assert.False(options.Console.Enabled);
        Assert.Equal(LogEventLevel.Warning, options.Console.MinimumLevel);
        Assert.Equal("{Message}", options.Console.OutputTemplate);
        Assert.False(options.File.Enabled);
        Assert.Equal("out/app-.log", options.File.Path);
        Assert.Equal(LogEventLevel.Debug, options.File.MinimumLevel);
        Assert.Equal(RollingInterval.Hour, options.File.RollingInterval);
        Assert.False(options.File.RollOnFileSizeLimit);
        Assert.Equal(10_485_760L, options.File.FileSizeLimitBytes);
        Assert.Equal("{Level}", options.File.OutputTemplate);
    }



    [Fact]
    public void Bind_when_onlySomeKeysPresent_expected_theRestKeepTheirDefaults()
    {
        var options = LoggingOptionsBinder.Bind(Section(("File:Path", "only/this.log")));

        var expected = new LoggingOptions();
        expected.File.Path = "only/this.log";
        Assert.Equal(Describe(expected), Describe(options));
    }



    [Fact]
    public void Bind_when_keysAndEnumNamesDifferInCase_expected_stillBound()
    {
        var options = LoggingOptionsBinder.Bind(Section(("minimumlevel", "warning"), ("FILE:rollinginterval", "MINUTE"), ("console:ENABLED", "FALSE")));

        Assert.Equal(LogEventLevel.Warning, options.MinimumLevel);
        Assert.Equal(RollingInterval.Minute, options.File.RollingInterval);
        Assert.False(options.Console.Enabled);
    }



    [Fact]
    public void Bind_when_valueForNonStringIsEmpty_expected_defaultKept()
    {
        var options = LoggingOptionsBinder.Bind(Section(("MinimumLevel", ""), ("Console:Enabled", ""), ("File:FileSizeLimitBytes", ""), ("Overrides:Microsoft", "")));

        Assert.Equal(Describe(new LoggingOptions()), Describe(options));
    }



    [Theory]
    [InlineData("MinimumLevel", "Loud", "Logging:MinimumLevel")]
    [InlineData("Overrides:Microsoft", "Loud", "Logging:Overrides:Microsoft")]
    [InlineData("Console:Enabled", "maybe", "Logging:Console:Enabled")]
    [InlineData("File:RollingInterval", "Fortnight", "Logging:File:RollingInterval")]
    [InlineData("File:FileSizeLimitBytes", "ten", "Logging:File:FileSizeLimitBytes")]
    [InlineData("File:RollOnFileSizeLimit", "1", "Logging:File:RollOnFileSizeLimit")]
    public void Bind_when_valueCannotBeConverted_expected_InvalidOperationNamingThePathAndValue(string key, string value, string path)
    {
        var exception = Assert.Throws<InvalidOperationException>(() => LoggingOptionsBinder.Bind(Section((key, value))));

        Assert.Contains(path, exception.Message, StringComparison.Ordinal);
        Assert.Contains($"'{value}'", exception.Message, StringComparison.Ordinal);
    }



    [Theory]
    [MemberData(nameof(ValidConfigurations))]
    public void Bind_when_inputIsValid_expected_sameResultAsTheFrameworkBinder(string name, (string Key, string? Value)[] values)
    {
        var section = Section(values);

        var expected = section.Get<LoggingOptions>() ?? new LoggingOptions();
        var actual = LoggingOptionsBinder.Bind(section);

        Assert.True(string.Equals(Describe(expected), Describe(actual), StringComparison.Ordinal), $"{name}: framework binder gave {Describe(expected)}, ours gave {Describe(actual)}");
    }



    public static TheoryData<string, (string, string?)[]> ValidConfigurations() => new()
    {
        { "nothing", [] },
        { "shipped", [("MinimumLevel", "Information"), ("Overrides:Microsoft", "Warning"), ("Overrides:System", "Warning"), ("Console:Enabled", "true"), ("Console:MinimumLevel", "Warning"), ("Console:OutputTemplate", "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"), ("File:Enabled", "true"), ("File:Path", "logs/log-.txt"), ("File:MinimumLevel", "Information"), ("File:RollingInterval", "Day"), ("File:RollOnFileSizeLimit", "true"), ("File:FileSizeLimitBytes", "10485760"), ("File:OutputTemplate", "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")] },
        { "lowercase", [("minimumlevel", "verbose"), ("console:enabled", "false"), ("file:rollinginterval", "infinite")] },
        { "enumNumbers", [("MinimumLevel", "4"), ("File:MinimumLevel", "0")] },
        { "overridesOnly", [("Overrides:A", "Debug"), ("Overrides:B", "Error"), ("Overrides:Sys", "fatal")] },
        { "emptyStrings", [("Console:OutputTemplate", ""), ("File:Path", "")] },
        { "bigNumber", [("File:FileSizeLimitBytes", "9223372036854775807")] },
        { "negativeNumber", [("File:FileSizeLimitBytes", "-1")] },
        { "boolCase", [("Console:Enabled", "TRUE"), ("File:Enabled", "False")] }
    };
}
