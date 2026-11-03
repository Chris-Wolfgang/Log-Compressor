using Serilog;
using Serilog.Events;
using Wolfgang.LogCompressor.Framework;

namespace Wolfgang.LogCompressor.Tests.Unit.Framework;

public sealed class SerilogConfiguratorTests : IDisposable
{
    // Console.SetError swaps process-wide state; tests that redirect it
    // must not overlap under xunit's parallel execution.
    private static readonly Lock ConsoleRedirectGate = new();

    private readonly TempDirectory _temp = new();



    public void Dispose()
    {
        _temp.Dispose();
    }



    [Fact]
    public void Apply_when_configurationNull_expected_throwsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>
        (
            () => ((LoggerConfiguration)null!).Apply(new LoggingOptions())
        );
    }



    [Fact]
    public void Apply_when_optionsNull_expected_throwsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>
        (
            () => new LoggerConfiguration().Apply(null!)
        );
    }



    [Fact]
    public void Apply_when_fileSinkEnabled_expected_logWrittenToFile()
    {
        var path = Path.Combine(_temp.Path, "enabled.log");
        var options = NewOptions(consoleEnabled: false, filePath: path);

        var logger = new LoggerConfiguration().Apply(options).CreateLogger();
        logger.Information("file-marker-{Id}", 42);
        logger.Dispose();

        Assert.Contains("file-marker-42", File.ReadAllText(path), StringComparison.Ordinal);
    }



    [Fact]
    public void Apply_when_fileSinkDisabled_expected_noFileCreated()
    {
        var path = Path.Combine(_temp.Path, "disabled.log");
        var options = NewOptions(consoleEnabled: false, filePath: path);
        options.File.Enabled = false;

        var logger = new LoggerConfiguration().Apply(options).CreateLogger();
        logger.Information("should-not-be-written");
        logger.Dispose();

        Assert.False(File.Exists(path));
    }



    [Fact]
    public void Apply_when_minimumLevelSet_expected_lowerLevelEventsFiltered()
    {
        var path = Path.Combine(_temp.Path, "filtered.log");
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
    public void Apply_when_overridesSpecified_expected_honored()
    {
        var path = Path.Combine(_temp.Path, "overrides.log");
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
    public void Apply_when_consoleSinkEnabled_expected_writtenToConsole()
    {
        var options = NewOptions(consoleEnabled: true, filePath: Path.Combine(_temp.Path, "unused.log"));
        options.File.Enabled = false;
        options.Console.MinimumLevel = LogEventLevel.Information;

        // Log output goes to STDERR — stdout is reserved for command results
        // so the tool composes in a pipeline. Console.Error is process-wide
        // state: serialize the redirection so parallel tests can't observe it.
        var captured = new StringWriter();
        lock (ConsoleRedirectGate)
        {
            var original = Console.Error;
            Console.SetError(captured);
            try
            {
                var logger = new LoggerConfiguration().Apply(options).CreateLogger();
                logger.Warning("console-marker");
                logger.Dispose();
            }
            finally
            {
                Console.SetError(original);
            }
        }

        Assert.Contains("console-marker", captured.ToString(), StringComparison.Ordinal);
    }



    [Fact]
    public void Apply_when_event_logged_expected_applicationPropertyEnriched()
    {
        var options = NewOptions(consoleEnabled: false, filePath: Path.Combine(_temp.Path, "enrich.log"));
        var sink = new CapturingSink();

        var logger = new LoggerConfiguration()
            .Apply(options)
            .WriteTo.Sink(sink)
            .CreateLogger();
        logger.Information("enrich-marker");
        logger.Dispose();

        var logEvent = Assert.Single(sink.Events);
        Assert.Equal
        (
            "\"Wolfgang.LogCompressor\"",
            logEvent.Properties["Application"].ToString()
        );
    }



    private sealed class CapturingSink : Serilog.Core.ILogEventSink
    {
        public List<LogEvent> Events { get; } = [];



        public void Emit(LogEvent logEvent) => Events.Add(logEvent);
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
