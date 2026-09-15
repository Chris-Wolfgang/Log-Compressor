using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Wolfgang.LogCompressor.Model;
using Wolfgang.LogCompressor.Service;

namespace Wolfgang.LogCompressor.Command;

/// <summary>
/// Extracts archives produced by the compress and bundle commands.
/// </summary>
[Command
(
    Description = "Extract logc archives - all formats, single archives and tar bundles",
    ResponseFileHandling = ResponseFileHandling.ParseArgsAsLineSeparated
)]
internal class Decompress
{
    /// <summary>
    /// Gets or sets the archive file or directory of archives to extract.
    /// </summary>
    [Argument(0, Description = "Archive file, or directory containing archives")]
    [Required]
    public string Path { get; set; } = string.Empty;



    /// <summary>
    /// Gets or sets the output directory. Defaults to each archive's own directory.
    /// </summary>
    [Option("-o|--output", Description = "Output directory (default: alongside each archive)")]
    public string? Output { get; set; }



    /// <summary>
    /// Gets or sets a value indicating whether to recurse into subdirectories.
    /// </summary>
    [Option("-r|--recurse", Description = "Recurse into subdirectories")]
    public bool Recurse { get; set; }



    /// <summary>
    /// Gets or sets the include glob patterns.
    /// </summary>
    [Option("--include <PATTERN>", Description = "Only extract archives matching the pattern (repeatable)")]
    public string[] Include { get; set; } = [];



    /// <summary>
    /// Gets or sets the exclude glob patterns.
    /// </summary>
    [Option("--exclude <PATTERN>", Description = "Skip archives matching the pattern (repeatable)")]
    public string[] Exclude { get; set; } = [];



    /// <summary>
    /// Gets or sets a value indicating whether existing files may be overwritten.
    /// </summary>
    [Option("--force", Description = "Overwrite existing files at extraction targets")]
    public bool Force { get; set; }



    /// <summary>
    /// Gets or sets a value indicating whether archives are kept after extraction.
    /// </summary>
    [Option("--keep-archives", Description = "Keep archives after successful extraction (default: delete them)")]
    public bool KeepArchives { get; set; }



    /// <summary>
    /// Gets or sets a value indicating whether the single-instance lock is skipped.
    /// </summary>
    [Option("--no-lock", Description = "Skip the single-instance directory lock")]
    public bool NoLock { get; set; }



    /// <summary>
    /// Gets or sets the report format.
    /// </summary>
    [Option("--report <FORMAT>", Description = "Write a summary report: json or csv")]
    public string? Report { get; set; }



    /// <summary>
    /// Gets or sets the report output path.
    /// </summary>
    [Option("--report-path <PATH>", Description = "Report output path (default: decompress-report.<format>)")]
    public string? ReportPath { get; set; }



    /// <summary>
    /// Gets or sets the batch error policy.
    /// </summary>
    [Option("--on-error <POLICY>", Description = "When an archive fails: skip, fail, or retry:N (1-100, then skip) (default: skip)")]
    public string OnError { get; set; } = "skip";



    /// <summary>
    /// Executes the decompress command.
    /// </summary>
    /// <param name="console">The console.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="decompressService">The decompression service.</param>
    /// <param name="reportService">The report service.</param>
    /// <param name="cancellationToken">Signaled on Ctrl+C / host shutdown; the run stops cleanly.</param>
    /// <returns>An exit code indicating success or failure.</returns>
#pragma warning disable MA0051 // Linear command orchestration; splitting hurts readability
    internal async Task<int> OnExecuteAsync
#pragma warning restore MA0051
    (
        IConsole console,
        ILogger<Decompress> logger,
        DecompressService decompressService,
        ReportService reportService,
        CancellationToken cancellationToken = default
    )
    {
        logger.LogDebug("Starting {Command}", GetType().Name);

        if (Report != null && Report is not ("json" or "csv"))
        {
            await console.Error.WriteLineAsync($"Error: Unsupported report format: '{Report}'. Supported: json, csv");
            return ExitCode.InvalidArguments;
        }

        if (!ErrorPolicy.TryParse(OnError, out var onError))
        {
            await console.Error.WriteLineAsync($"Error: Unsupported error policy: '{OnError}'. Supported: skip, fail, retry:N (1-100)");
            return ExitCode.InvalidArguments;
        }

        if (ReportPath != null && Report == null)
        {
            await console.Error.WriteLineAsync("Error: --report-path requires --report (matches compress/bundle validation).");
            return ExitCode.InvalidArguments;
        }

        var options = new DecompressionOptions
        {
            SourcePath = System.IO.Path.GetFullPath(Path),
            OutputPath = Output is null ? null : System.IO.Path.GetFullPath(Output),
            Recurse = Recurse,
            IncludePatterns = Include,
            ExcludePatterns = Exclude,
            Force = Force,
            KeepArchives = KeepArchives,
            NoLock = NoLock,
            ReportFormat = Report,
            ReportPath = ReportPath,
            OnError = onError
        };

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
            var results = await decompressService.ExecuteAsync(options, cancellationToken).ConfigureAwait(false);
            sw.Stop();

            var succeeded = results.Count(r => r.Success);
            var failed = results.Count - succeeded;

            await console.Out.WriteLineAsync($"Extracted {succeeded} archive(s) successfully.");

            if (failed > 0)
            {
                await console.Error.WriteLineAsync($"{failed} archive(s) failed to extract.");
            }

            if (options.ReportFormat != null)
            {
                var reportPath = options.ReportPath
                    ?? $"decompress-report.{options.ReportFormat.ToLowerInvariant()}";

                await reportService.WriteReportAsync(results, options.ReportFormat, reportPath, sw.Elapsed)
                    .ConfigureAwait(false);

                logger.LogInformation("Report written to {Path}", reportPath);
            }

            logger.LogDebug("Completed {Command}", GetType().Name);

            if (failed > 0)
            {
                return onError.Mode == OnErrorMode.Fail
                    ? ExitCode.ApplicationError
                    : ExitCode.CompletedWithSkips;
            }

            return ExitCode.Success;
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            logger.LogInformation(ex, "Run canceled.");
            await console.Error.WriteLineAsync("Canceled.");
            return ExitCode.Canceled;
        }
        catch (Exception e)
        {
            logger.LogCritical(e, "Unhandled error: {Message}", e.Message);
            await console.Error.WriteLineAsync(e.Message);
            return ExitCode.ApplicationError;
        }
    }
}
