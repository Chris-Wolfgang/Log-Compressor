using System.Diagnostics.CodeAnalysis;
using McMaster.Extensions.CommandLineUtils;

namespace Wolfgang.LogCompressor.Command;

/// <summary>
/// Generates a starter response file (.rsp) for compression configuration.
/// </summary>
[Command
(
    Description = "Generate a starter configuration file",
    ResponseFileHandling = ResponseFileHandling.Disabled
)]
[Subcommand(typeof(InitCompress))]
[Subcommand(typeof(InitBundle))]
[ExcludeFromCodeCoverage]
internal class Init
{
    /// <summary>
    /// Shows help when no sub-command is specified.
    /// </summary>
    /// <param name="application">The command line application.</param>
    /// <returns>The exit code.</returns>
    internal int OnExecute(CommandLineApplication application)
    {
        application.ShowHelp();
        return ExitCode.Success;
    }
}
