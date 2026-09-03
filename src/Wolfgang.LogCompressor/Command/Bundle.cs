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
#pragma warning disable MA0051 // Linear command orchestration; splitting hurts readability
    internal async Task<int> OnExecuteAsync
#pragma warning restore MA0051
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

        var options = BuildOptions();

        using var processLock = new ProcessLock
        (
            ProcessLock.LockDirectoryFor(options.SourcePath),
            logger
        );

        if (!options.NoLock && !processLock.TryAcquire())
        {
            await console.Error.WriteLineAsync("Another instance is already processing this directory.");
            return ExitCode.AlreadyRunning;
        }

        try
        {
            var sw = Stopwatch.StartNew();
            var result = await bundleService.ExecuteAsync(options).ConfigureAwait(false);
            sw.Stop();

            if (result.Success)
            {
                await console.Out.WriteLineAsync($"Bundled files to {result.OutputPath}");
                await console.Out.WriteLineAsync($"  Original: {result.OriginalSize:N0} bytes");
                await console.Out.WriteLineAsync($"  Compressed: {result.CompressedSize:N0} bytes");
            }
            else
            {
                await console.Error.WriteLineAsync($"Bundle failed: {result.ErrorMessage}");
            }

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

            if (result.Success)
            {
                return ExitCode.Success;
            }

            // Unreadable inputs were skipped but the bundle itself was written
            // and verified — degraded, not broken (fail mode throws instead of
            // reaching here).
            return result.SkippedCount > 0 ? ExitCode.CompletedWithSkips : ExitCode.ApplicationError;
        }
        catch (Exception e)
        {
            logger.LogCritical(e, "Unhandled error: {Message}", e.Message);
            await console.Error.WriteLineAsync(e.Message);
            return ExitCode.ApplicationError;
        }
    }
}
