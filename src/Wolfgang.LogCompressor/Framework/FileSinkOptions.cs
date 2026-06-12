using Serilog;
using Serilog.Events;

namespace Wolfgang.LogCompressor.Framework;

/// <summary>
/// Options for the Serilog rolling-file sink.
/// </summary>
internal sealed class FileSinkOptions
{
    /// <summary>
    /// Whether the file sink is enabled. Defaults to <see langword="true"/>.
    /// </summary>
    public bool Enabled { get; set; } = true;



    /// <summary>
    /// The log file path. A date is inserted at the trailing <c>-</c> when rolling.
    /// </summary>
    public string Path { get; set; } = "logs/log-.txt";



    /// <summary>
    /// The minimum level written to the file. Defaults to <see cref="LogEventLevel.Debug"/>.
    /// </summary>
    public LogEventLevel MinimumLevel { get; set; } = LogEventLevel.Debug;



    /// <summary>
    /// How often a new file is started. Defaults to <see cref="RollingInterval.Day"/>.
    /// </summary>
    public RollingInterval RollingInterval { get; set; } = RollingInterval.Day;



    /// <summary>
    /// Whether to also roll when <see cref="FileSizeLimitBytes"/> is reached.
    /// </summary>
    public bool RollOnFileSizeLimit { get; set; } = true;



    /// <summary>
    /// The per-file size limit in bytes. Defaults to 10 MiB.
    /// </summary>
    public long FileSizeLimitBytes { get; set; } = 10 * 1024 * 1024;



    /// <summary>
    /// The message output template.
    /// </summary>
    public string OutputTemplate { get; set; } =
        "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";
}
