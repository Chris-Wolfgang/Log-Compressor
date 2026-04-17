using System.Diagnostics.CodeAnalysis;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;

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



/// <summary>
/// Generates a compress configuration file.
/// </summary>
[Command
(
    Name = "compress",
    Description = "Generate a compress configuration file"
)]
[ExcludeFromCodeCoverage]
internal class InitCompress
{
    /// <summary>
    /// Gets or sets the output path for the config file.
    /// </summary>
    [Option
    (
        "-o|--output",
        Description = "Output path for the config file (default: compress.rsp)"
    )]
    public string Output { get; set; } = "compress.rsp";



    /// <summary>
    /// Generates the compress configuration file.
    /// </summary>
    /// <param name="console">The console.</param>
    /// <param name="logger">The logger.</param>
    /// <returns>The exit code.</returns>
    internal async Task<int> OnExecuteAsync(IConsole console, ILogger<InitCompress> logger)
    {
        var content = """
                      # logc compress configuration
                      # Usage: logc compress @compress.rsp
                      #
                      # Uncomment and modify the options below.

                      # Source path (required)
                      # C:\Logs\MyApp

                      # Recurse into subdirectories
                      # --recurse

                      # Output directory (defaults to source directory)
                      # --output C:\ArchivedLogs\MyApp

                      # Compression format: zip, gz, brotli
                      # --format zip

                      # Compression level: fastest, optimal, smallest
                      # --level optimal

                      # Only compress files older than N days
                      # --older-than 7

                      # Date range filter (mutually exclusive with --older-than)
                      # --min-datetime 2026-01-01
                      # --max-datetime 2026-12-31

                      # Include only specific file patterns
                      # --include *.log
                      # --include *.txt

                      # Exclude specific file patterns
                      # --exclude current.log

                      # Skip archive verification before deleting originals
                      # --no-verify

                      # Generate a summary report
                      # --report json
                      # --report-path ./compress-report.json

                      # Delete old archives after compression
                      # --delete-archives-older-than 365
                      """;

        await File.WriteAllTextAsync(Output, content).ConfigureAwait(false);

#pragma warning disable CA1849, VSTHRD103 // McMaster IConsole has no async overloads
        console.WriteLine($"Configuration file created: {Output}");
        console.WriteLine($"Usage: logc compress @{Output}");
#pragma warning restore CA1849, VSTHRD103

        logger.LogInformation("Created config file: {Path}", Output);
        return ExitCode.Success;
    }
}



/// <summary>
/// Generates a bundle configuration file.
/// </summary>
[Command
(
    Name = "bundle",
    Description = "Generate a bundle configuration file"
)]
[ExcludeFromCodeCoverage]
internal class InitBundle
{
    /// <summary>
    /// Gets or sets the output path for the config file.
    /// </summary>
    [Option
    (
        "-o|--output",
        Description = "Output path for the config file (default: bundle.rsp)"
    )]
    public string Output { get; set; } = "bundle.rsp";



    /// <summary>
    /// Generates the bundle configuration file.
    /// </summary>
    /// <param name="console">The console.</param>
    /// <param name="logger">The logger.</param>
    /// <returns>The exit code.</returns>
    internal async Task<int> OnExecuteAsync(IConsole console, ILogger<InitBundle> logger)
    {
        var content = """
                      # logc bundle configuration
                      # Usage: logc bundle @bundle.rsp
                      #
                      # Uncomment and modify the options below.

                      # Source path (required)
                      # C:\Logs\MyApp

                      # Recurse into subdirectories
                      # --recurse

                      # Output directory (defaults to source parent directory)
                      # --output C:\ArchivedLogs

                      # Compression format: zip, gz, brotli
                      # --format zip

                      # Compression level: fastest, optimal, smallest
                      # --level optimal

                      # Only bundle files older than N days
                      # --older-than 7

                      # Date range filter (mutually exclusive with --older-than)
                      # --min-datetime 2026-01-01
                      # --max-datetime 2026-12-31

                      # Include only specific file patterns
                      # --include *.log

                      # Exclude specific file patterns
                      # --exclude current.log

                      # Skip archive verification before deleting originals
                      # --no-verify

                      # Generate a summary report
                      # --report json
                      # --report-path ./bundle-report.json

                      # Delete old archives after bundling
                      # --delete-archives-older-than 365
                      """;

        await File.WriteAllTextAsync(Output, content).ConfigureAwait(false);

#pragma warning disable CA1849, VSTHRD103 // McMaster IConsole has no async overloads
        console.WriteLine($"Configuration file created: {Output}");
        console.WriteLine($"Usage: logc bundle @{Output}");
#pragma warning restore CA1849, VSTHRD103

        logger.LogInformation("Created config file: {Path}", Output);
        return ExitCode.Success;
    }
}
