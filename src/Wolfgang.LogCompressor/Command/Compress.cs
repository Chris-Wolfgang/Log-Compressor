using System.Diagnostics;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Wolfgang.LogCompressor.Model;
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
    /// <param name="cancellationToken">Signaled on Ctrl+C / host shutdown; the run stops cleanly.</param>
    /// <returns>An exit code indicating success or failure.</returns>
#pragma warning disable MA0051 // Linear command orchestration; splitting hurts readability
    internal async Task<int> OnExecuteAsync
#pragma warning restore MA0051
    (
        IConsole console,
        ILogger<Compress> logger,
        CompressService compressService,
        ReportService reportService,
        RetentionService retentionService,
        CancellationToken cancellationToken = default
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
            var results = await compressService.ExecuteAsync(options, cancellationToken).ConfigureAwait(false);
            sw.Stop();

            var succeeded = results.Count(r => r.Success);
            var failed = results.Count(r => !r.Success);

            await console.Out.WriteLineAsync($"Compressed {succeeded} file(s) successfully.");

            if (failed > 0)
            {
                await console.Error.WriteLineAsync($"{failed} file(s) failed to compress.");
            }

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
                // Retention must scan where archives are actually written: alongside
                // each source (the source directory itself when SourcePath is a
                // directory), or the source's directory when SourcePath is a file.
                var archiveDir = options.OutputPath
                    ?? (Directory.Exists(options.SourcePath)
                        ? options.SourcePath
                        : System.IO.Path.GetDirectoryName(options.SourcePath))
                    ?? ".";
                retentionService.DeleteOldArchives(archiveDir, options.DeleteArchivesOlderThanDays.Value);
            }

            logger.LogDebug("Completed {Command}", GetType().Name);

            if (failed > 0)
            {
                // Under skip/retry the run completed by policy: signal a
                // partial outcome distinctly so schedulers can tell degraded
                // from broken (--on-error fail keeps the hard error).
                return options.OnError.Mode == OnErrorMode.Fail
                    ? ExitCode.ApplicationError
                    : ExitCode.CompletedWithSkips;
            }

            return ExitCode.Success;
        }
        catch (Exception e)
        {
            logger.LogCritical(e, "Unhandled error: {Message}", e.Message);
            await console.Error.WriteLineAsync(e.Message);
            return ExitCode.ApplicationError;
        }
    }
}
