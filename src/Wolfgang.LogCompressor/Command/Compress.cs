using System.Diagnostics;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Wolfgang.LogCompressor.Service;

namespace Wolfgang.LogCompressor.Command;

/// <summary>
/// Compresses files individually, producing one archive per source file.
/// </summary>
[Command
(
    Description = "Compress files individually - one archive per source file",
    ResponseFileHandling = ResponseFileHandling.ParseArgsAsLineSeparated
)]
internal class Compress : SharedOptions
{
    /// <summary>
    /// Executes the compress command.
    /// </summary>
    /// <param name="console">The console.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="compressService">The compression service.</param>
    /// <param name="reportService">The report service.</param>
    /// <param name="retentionService">The retention service.</param>
    /// <returns>An exit code indicating success or failure.</returns>
    internal async Task<int> OnExecuteAsync
    (
        IConsole console,
        ILogger<Compress> logger,
        CompressService compressService,
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
            var results = await compressService.ExecuteAsync(options).ConfigureAwait(false);
            sw.Stop();

            var succeeded = results.Count(r => r.Success);
            var failed = results.Count(r => !r.Success);

#pragma warning disable CA1849, VSTHRD103 // McMaster IConsole has no async overloads
            console.WriteLine($"Compressed {succeeded} file(s) successfully.");

            if (failed > 0)
            {
                console.Error.WriteLine($"{failed} file(s) failed to compress.");
            }
#pragma warning restore CA1849, VSTHRD103

            if (options.ReportFormat != null)
            {
                var reportPath = options.ReportPath
                    ?? $"compress-report.{options.ReportFormat.ToLowerInvariant()}";

                await reportService.WriteReportAsync(results, options.ReportFormat, reportPath, sw.Elapsed)
                    .ConfigureAwait(false);

                logger.LogInformation("Report written to {Path}", reportPath);
            }

            if (options.DeleteArchivesOlderThanDays.HasValue)
            {
                var archiveDir = options.OutputPath ?? System.IO.Path.GetDirectoryName(options.SourcePath) ?? ".";
                retentionService.DeleteOldArchives(archiveDir, options.DeleteArchivesOlderThanDays.Value);
            }

            logger.LogDebug("Completed {Command}", GetType().Name);

            return failed > 0 ? ExitCode.ApplicationError : ExitCode.Success;
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
