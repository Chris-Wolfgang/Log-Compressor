using McMaster.Extensions.CommandLineUtils;

namespace Wolfgang.LogCompressor.Tests.Unit.Framework;

/// <summary>
/// A minimal command in the same response-file mode as the app, for exercising the real parser.
/// </summary>
[Command(ResponseFileHandling = ResponseFileHandling.ParseArgsAsLineSeparated, UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.Throw)]
internal sealed class ProbeCommand
{
    [Argument(0)]
    // ReSharper disable once UnusedAutoPropertyAccessor.Global — McMaster sets the bound properties via reflection
    public string? Path { get; set; }



    [Option("--recurse")]
    // ReSharper disable once UnusedAutoPropertyAccessor.Global — McMaster sets the bound properties via reflection
    public bool Recurse { get; set; }



    [Option("--older-than")]
    // ReSharper disable once UnusedAutoPropertyAccessor.Global — McMaster sets the bound properties via reflection
    public int? OlderThan { get; set; }
}
