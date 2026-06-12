using Serilog.Events;

namespace Wolfgang.LogCompressor.Framework;

/// <summary>
/// Strongly-typed logging configuration bound from the <c>Logging</c> section of
/// <c>AppSettings.json</c>. Replaces the reflection-based
/// <c>Serilog.Settings.Configuration</c> schema so the binding is performed by the
/// source-generated configuration binder (trim- and single-file-safe).
/// </summary>
internal sealed class LoggingOptions
{
    /// <summary>
    /// The global minimum level. Defaults to <see cref="LogEventLevel.Information"/>.
    /// </summary>
    public LogEventLevel MinimumLevel { get; set; } = LogEventLevel.Information;



    /// <summary>
    /// Per-source minimum-level overrides (e.g. <c>Microsoft</c> or <c>System</c>).
    /// </summary>
    public Dictionary<string, LogEventLevel> Overrides { get; set; } = new(StringComparer.Ordinal);



    /// <summary>
    /// Console sink options.
    /// </summary>
    public ConsoleSinkOptions Console { get; set; } = new();



    /// <summary>
    /// Rolling-file sink options.
    /// </summary>
    public FileSinkOptions File { get; set; } = new();
}
