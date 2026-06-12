using Serilog.Events;

namespace Wolfgang.LogCompressor.Framework;

/// <summary>
/// Options for the Serilog console sink.
/// </summary>
internal sealed class ConsoleSinkOptions
{
    /// <summary>
    /// Whether the console sink is enabled. Defaults to <see langword="true"/>.
    /// </summary>
    public bool Enabled { get; set; } = true;



    /// <summary>
    /// The minimum level emitted to the console. Defaults to <see cref="LogEventLevel.Information"/>.
    /// </summary>
    public LogEventLevel MinimumLevel { get; set; } = LogEventLevel.Information;



    /// <summary>
    /// The message output template.
    /// </summary>
    public string OutputTemplate { get; set; } =
        "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";
}
