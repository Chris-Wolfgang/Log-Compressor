using System.Diagnostics;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Wolfgang.LogCompressor.Service;

namespace Wolfgang.LogCompressor.Command;

/// <summary>
/// Bundles all matching files into a single compressed archive.
/// </summary>
[Command
(
    Description = "Bundle all matching files into a single archive",
    ResponseFileHandling = ResponseFileHandling.ParseArgsAsLineSeparated
)]
internal class Bundle : SharedOptions
{
    /// <summary>
    /// Executes the bundle command.
    /// </summary>
    /// <param name="console">The console.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="bundleService">The bundle service.</param>
    /// <param name="reportService">The report service.</param>
    /// <param name="retentionService">The retention service.</param>
    /// <returns>An exit code indicating success or failure.</returns>
    internal async Task<int> OnExecuteAsync
    (
        IConsole console,
        ILogger<Bundle> logger,
        BundleService bundleService,
        ReportService reportService,
        RetentionService retentionService
    )
    {
        logger.LogDebug("Starting {Command}", GetType().Name);

        if (!ValidateOptions(console))
        {
            return ExitCode.InvalidArguments;
        }

        try
        {
            var options = BuildOptions();
            var sw = Stopwatch.StartNew();
            var result = await bundleService.ExecuteAsync(options).ConfigureAwait(false);
            sw.Stop();

#pragma warning disable CA1849, VSTHRD103 // McMaster IConsole has no async overloads
            if (result.Success)
            {
                console.WriteLine($"Bundled files to {result.OutputPath}");
                console.WriteLine($"  Original: {result.OriginalSize:N0} bytes");
                console.WriteLine($"  Compressed: {result.CompressedSize:N0} bytes");
            }
            else
            {
                console.Error.WriteLine($"Bundle failed: {result.ErrorMessage}");
            }
#pragma warning restore CA1849, VSTHRD103

            if (options.ReportFormat != null)
            {
                var reportPath = options.ReportPath
                    ?? $"bundle-report.{options.ReportFormat.ToLowerInvariant()}";

                await reportService.WriteReportAsync([result], options.ReportFormat, reportPath, sw.Elapsed)
                    .ConfigureAwait(false);

                logger.LogInformation("Report written to {Path}", reportPath);
            }

            if (options.DeleteArchivesOlderThanDays.HasValue)
            {
                var archiveDir = options.OutputPath ?? System.IO.Path.GetDirectoryName(options.SourcePath) ?? ".";
                retentionService.DeleteOldArchives(archiveDir, options.DeleteArchivesOlderThanDays.Value);
            }

            logger.LogDebug("Completed {Command}", GetType().Name);

            return result.Success ? ExitCode.Success : ExitCode.ApplicationError;
        }
        catch (Exception e)
        {
            logger.LogCritical(e, "Unhandled error: {Message}", e.Message);
#pragma warning disable CA1849, VSTHRD103
            console.Error.WriteLine(e.Message);
#pragma warning restore CA1849, VSTHRD103
            return ExitCode.ApplicationError;
        }
    }
}
