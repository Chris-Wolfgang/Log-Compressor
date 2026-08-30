using Serilog;
using Serilog.Events;
using Wolfgang.LogCompressor.Framework;

namespace Wolfgang.LogCompressor.Tests.Unit.Framework;

public sealed class SerilogConfiguratorTests : IDisposable
{
    private readonly string _tempDir =
        Path.Combine(Path.GetTempPath(), "logc-serilog-tests-" + Guid.NewGuid().ToString("N"));



    public SerilogConfiguratorTests()
    {
        Directory.CreateDirectory(_tempDir);
    }



    public void Dispose()
    {
        try
        {
            Directory.Delete(_tempDir, recursive: true);
        }
        catch (DirectoryNotFoundException)
        {
            // already gone
        }
    }



    [Fact]
    public void Apply_when_configuration_is_null_throws_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>
        (
            () => ((LoggerConfiguration)null!).Apply(new LoggingOptions())
        );
    }



    [Fact]
    public void Apply_when_options_is_null_throws_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>
        (
            () => new LoggerConfiguration().Apply(null!)
        );
    }



    [Fact]
    public void Apply_when_file_sink_enabled_writes_log_to_file()
    {
        var path = Path.Combine(_tempDir, "enabled.log");
        var options = NewOptions(consoleEnabled: false, filePath: path);

        var logger = new LoggerConfiguration().Apply(options).CreateLogger();
        logger.Information("file-marker-{Id}", 42);
        logger.Dispose();

        Assert.Contains("file-marker-42", File.ReadAllText(path), StringComparison.Ordinal);
    }



    [Fact]
    public void Apply_when_file_sink_disabled_does_not_create_file()
    {
        var path = Path.Combine(_tempDir, "disabled.log");
        var options = NewOptions(consoleEnabled: false, filePath: path);
        options.File.Enabled = false;

        var logger = new LoggerConfiguration().Apply(options).CreateLogger();
        logger.Information("should-not-be-written");
        logger.Dispose();

        Assert.False(File.Exists(path));
    }



    [Fact]
    public void Apply_when_minimum_level_set_filters_lower_level_events()
    {
        var path = Path.Combine(_tempDir, "filtered.log");
        var options = NewOptions(consoleEnabled: false, filePath: path);
        options.MinimumLevel = LogEventLevel.Warning;

        var logger = new LoggerConfiguration().Apply(options).CreateLogger();
        logger.Information("below-threshold");
        logger.Warning("above-threshold");
        logger.Dispose();

        var contents = File.ReadAllText(path);

        Assert.DoesNotContain("below-threshold", contents, StringComparison.Ordinal);
        Assert.Contains("above-threshold", contents, StringComparison.Ordinal);
    }



    [Fact]
    public void Apply_when_overrides_specified_are_honored()
    {
        var path = Path.Combine(_tempDir, "overrides.log");
        var options = NewOptions(consoleEnabled: false, filePath: path);
        options.MinimumLevel = LogEventLevel.Debug;
        options.Overrides["Wolfgang.LogCompressor.Tests.Unit.Framework.Noisy"] = LogEventLevel.Error;

        var logger = new LoggerConfiguration().Apply(options).CreateLogger();
        logger
            .ForContext(Serilog.Core.Constants.SourceContextPropertyName, "Wolfgang.LogCompressor.Tests.Unit.Framework.Noisy")
            .Information("suppressed-by-override");
        logger.Information("not-suppressed");
        logger.Dispose();

        var contents = File.ReadAllText(path);

        Assert.DoesNotContain("suppressed-by-override", contents, StringComparison.Ordinal);
        Assert.Contains("not-suppressed", contents, StringComparison.Ordinal);
    }



    [Fact]
    public void Apply_when_console_sink_enabled_writes_to_console()
    {
        var options = NewOptions(consoleEnabled: true, filePath: Path.Combine(_tempDir, "unused.log"));
        options.File.Enabled = false;
        options.Console.MinimumLevel = LogEventLevel.Information;

        var captured = new StringWriter();
        var original = Console.Out;
        Console.SetOut(captured);
        try
        {
            var logger = new LoggerConfiguration().Apply(options).CreateLogger();
            logger.Warning("console-marker");
            logger.Dispose();
        }
        finally
        {
            Console.SetOut(original);
        }

        Assert.Contains("console-marker", captured.ToString(), StringComparison.Ordinal);
    }



    [Fact]
    public void Dispose_when_tempDirAlreadyDeleted_expected_noThrow()
    {
        // Exercises the DirectoryNotFoundException catch in this fixture's own
        // Dispose: deleting the directory here means xunit's dispose call after
        // the test hits the already-gone path.
        Directory.Delete(_tempDir, recursive: true);

        Assert.False(Directory.Exists(_tempDir));
    }



    private static LoggingOptions NewOptions(bool consoleEnabled, string filePath)
    {
        return new LoggingOptions
        {
            MinimumLevel = LogEventLevel.Debug,
            Console = { Enabled = consoleEnabled, MinimumLevel = LogEventLevel.Debug },
            File =
            {
                Enabled = true,
                Path = filePath,
                MinimumLevel = LogEventLevel.Debug,
                RollingInterval = RollingInterval.Infinite,
                RollOnFileSizeLimit = false
            }
        };
    }
}
