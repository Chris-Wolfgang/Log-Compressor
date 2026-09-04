using System.Diagnostics.CodeAnalysis;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;

namespace Wolfgang.LogCompressor.Command;

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
    // The generated starter file. A const keeps OnExecuteAsync focused on I/O
    // (and under MA0051's 60-line cap) as the template grows with new flags.
    private const string TemplateContent = """
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

        # Compression format: zip, gz, brotli, zstd, lz4
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

        # Timestamp embedded in archive names: modified (default), compressed
        # --timestamp modified

        # Custom base name for archives (colliding names get -2, -3, ...)
        # --name weblogs

        # When an item fails: skip (default), fail, or retry:N (1-100, then skip)
        # --on-error retry:3

        # Skip archive verification before deleting originals
        # --no-verify

        # Generate a summary report
        # --report json
        # --report-path ./bundle-report.json

        # Delete old archives after bundling
        # --delete-archives-older-than 365
        """;



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

        await File.WriteAllTextAsync(Output, TemplateContent).ConfigureAwait(false);

#pragma warning disable CA1849, VSTHRD103 // McMaster IConsole has no async overloads
        console.WriteLine($"Configuration file created: {Output}");
        console.WriteLine($"Usage: logc bundle @{Output}");
#pragma warning restore CA1849, VSTHRD103

        logger.LogInformation("Created config file: {Path}", Output);
        return ExitCode.Success;
    }
}
