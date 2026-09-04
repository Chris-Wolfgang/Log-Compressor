using Serilog;
using Serilog.Events;

namespace Wolfgang.LogCompressor.Framework;

/// <summary>
/// Applies a strongly-typed <see cref="LoggingOptions"/> to a Serilog
/// <see cref="LoggerConfiguration"/> in code. This replaces
/// <c>LoggerConfiguration.ReadFrom.Configuration(...)</c> (which instantiates sinks
/// by name via reflection) with an explicit, trim-safe configuration.
/// </summary>
internal static class SerilogConfigurator
{
    /// <summary>
    /// Configures <paramref name="configuration"/> from <paramref name="options"/>:
    /// minimum level, per-source overrides, log-context enrichment, and the console
    /// and rolling-file sinks (each emitted only when enabled).
    /// </summary>
    /// <param name="configuration">The Serilog logger configuration to mutate.</param>
    /// <param name="options">The bound logging options.</param>
    /// <returns>The same <paramref name="configuration"/>, for chaining.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="configuration"/> or <paramref name="options"/> is <see langword="null"/>.
    /// </exception>
    public static LoggerConfiguration Apply
    (
        this LoggerConfiguration configuration,
        LoggingOptions options
    )
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(options);

        configuration.MinimumLevel.Is(options.MinimumLevel);

        foreach (var (source, level) in options.Overrides)
        {
            configuration.MinimumLevel.Override(source, level);
        }

        configuration
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", "Wolfgang.LogCompressor");

        if (options.Console.Enabled)
        {
            configuration.WriteTo.Console
            (
                restrictedToMinimumLevel: options.Console.MinimumLevel,
                outputTemplate: options.Console.OutputTemplate,
                // All log events to stderr: stdout is reserved for command
                // results so the tool composes in a pipeline.
                standardErrorFromLevel: LevelAlias.Minimum
            );
        }

        if (options.File.Enabled)
        {
            configuration.WriteTo.File
            (
                path: options.File.Path,
                restrictedToMinimumLevel: options.File.MinimumLevel,
                outputTemplate: options.File.OutputTemplate,
                fileSizeLimitBytes: options.File.FileSizeLimitBytes,
                rollingInterval: options.File.RollingInterval,
                rollOnFileSizeLimit: options.File.RollOnFileSizeLimit
            );
        }

        return configuration;
    }
}
