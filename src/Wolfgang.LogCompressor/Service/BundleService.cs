using Microsoft.Extensions.Logging;
using Wolfgang.LogCompressor.Abstraction;
using Wolfgang.LogCompressor.Model;
using Wolfgang.LogCompressor.Service.Compression;

namespace Wolfgang.LogCompressor.Service;

/// <summary>
/// Bundles all matching files into a single compressed archive.
/// </summary>
internal class BundleService
{
    private readonly IFileSystem _fileSystem;
    private readonly IFileFilter _fileFilter;
    private readonly IFileNamer _fileNamer;
    private readonly IArchiveVerifier _archiveVerifier;
    private readonly CompressionStrategyFactory _strategyFactory;
    private readonly ILogger<BundleService> _logger;



    /// <summary>
    /// Initializes a new instance of the <see cref="BundleService"/> class.
    /// </summary>
    /// <param name="fileSystem">The file system abstraction.</param>
    /// <param name="fileFilter">The file filter service.</param>
    /// <param name="fileNamer">The file naming service.</param>
    /// <param name="archiveVerifier">The archive verifier.</param>
    /// <param name="strategyFactory">The compression strategy factory.</param>
    /// <param name="logger">The logger.</param>
    public BundleService
    (
        IFileSystem fileSystem,
        IFileFilter fileFilter,
        IFileNamer fileNamer,
        IArchiveVerifier archiveVerifier,
        CompressionStrategyFactory strategyFactory,
        ILogger<BundleService> logger
    )
    {
        ArgumentNullException.ThrowIfNull(fileSystem);
        ArgumentNullException.ThrowIfNull(fileFilter);
        ArgumentNullException.ThrowIfNull(fileNamer);
        ArgumentNullException.ThrowIfNull(archiveVerifier);
        ArgumentNullException.ThrowIfNull(strategyFactory);
        ArgumentNullException.ThrowIfNull(logger);

        _fileSystem = fileSystem;
        _fileFilter = fileFilter;
        _fileNamer = fileNamer;
        _archiveVerifier = archiveVerifier;
        _strategyFactory = strategyFactory;
        _logger = logger;
    }



    /// <summary>
    /// Bundles files into a single archive according to the specified options.
    /// </summary>
    /// <param name="options">The compression options.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The compression result.</returns>
#pragma warning disable MA0051 // Linear orchestration with logging branches; splitting hurts readability
    public virtual async Task<CompressionResult> ExecuteAsync
#pragma warning restore MA0051
    (
        CompressionOptions options,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(options);

        var strategy = _strategyFactory.Create(options.Format, options.Level);
        var files = EnumerateSourceFiles(options);
        var filtered = _fileFilter.Apply
        (
            files,
            options.OlderThanDays,
            options.MinDateTime,
            options.MaxDateTime,
            options.IncludePatterns,
            options.ExcludePatterns
        );

        _logger.LogInformation
        (
            "Found {TotalCount} file(s), {FilteredCount} after filtering",
            files.Count,
            filtered.Count
        );

        if (filtered.Count == 0)
        {
            _logger.LogWarning("No files matched the specified criteria");

            return new CompressionResult
            {
                SourcePath = options.SourcePath,
                OutputPath = string.Empty,
                Success = false,
                ErrorMessage = "No files matched the specified criteria."
            };
        }

        string folderName;
        string outputDir;

        if (_fileSystem.FileExists(options.SourcePath))
        {
            var fileInfo = _fileSystem.GetFileInfo(options.SourcePath);
            folderName = System.IO.Path.GetFileNameWithoutExtension(fileInfo.Name);
            outputDir = options.OutputPath ?? fileInfo.DirectoryName ?? Directory.GetCurrentDirectory();
        }
        else
        {
            var sourceDirectory = new DirectoryInfo(options.SourcePath);
            folderName = string.IsNullOrWhiteSpace(sourceDirectory.Name) ? "archive" : sourceDirectory.Name;
            outputDir = options.OutputPath ?? sourceDirectory.Parent?.FullName ?? sourceDirectory.FullName;
        }
        var outputFileName = _fileNamer.GetBundleFileName(folderName, filtered, strategy.BundleFileExtension);
        var outputPath = Path.Combine(outputDir, outputFileName);

        // Never overwrite an existing archive — refuse rather than clobber it and
        // delete the sources that were folded into the replacement.
        if (_fileSystem.FileExists(outputPath))
        {
            _logger.LogError("Output archive already exists, refusing to overwrite: {Output}", outputPath);

            return new CompressionResult
            {
                SourcePath = options.SourcePath,
                OutputPath = outputPath,
                Success = false,
                ErrorMessage = $"Output archive already exists: {outputPath}"
            };
        }

        try
        {
            if (options.OutputPath != null && !_fileSystem.DirectoryExists(options.OutputPath))
            {
                _fileSystem.CreateDirectory(options.OutputPath);
            }

            return await CompressAndDeleteAsync(filtered, outputPath, strategy, options, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Failed to bundle files from {Source}: {Message}", options.SourcePath, ex.Message);

            return new CompressionResult
            {
                SourcePath = options.SourcePath,
                OutputPath = outputPath,
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }



#pragma warning disable MA0051 // Linear compress/verify/delete with stream cleanup; splitting hurts readability
    private async Task<CompressionResult> CompressAndDeleteAsync
#pragma warning restore MA0051
    (
        IReadOnlyList<FileInfo> filtered,
        string outputPath,
        ICompressionStrategy strategy,
        CompressionOptions options,
        CancellationToken cancellationToken
    )
    {
        var bundled = new List<FileInfo>(filtered.Count);
        var skipped = 0;

        // Open each source lazily as the strategy writes it; the strategy disposes
        // each stream immediately, so only one source handle is open at a time —
        // safe for bundles of many thousands of files. Unreadable files (locked,
        // no read permission) are skipped and recorded so they are never deleted.
        IEnumerable<(Stream Stream, string EntryName)> OpenInputs()
        {
            foreach (var file in filtered)
            {
                Stream stream;

                try
                {
                    stream = _fileSystem.OpenRead(file.FullName);
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
                {
                    skipped++;
                    _logger.LogWarning("Skipping unreadable file {File}: {Message}", file.FullName, ex.Message);
                    continue;
                }

                bundled.Add(file);
                yield return (stream, file.Name);
            }
        }

        long compressedSize;

        await using (var outputStream = _fileSystem.CreateWrite(outputPath))
        {
            await strategy.CompressFilesAsync(OpenInputs(), outputStream, cancellationToken).ConfigureAwait(false);
            await outputStream.FlushAsync(cancellationToken).ConfigureAwait(false);
            compressedSize = outputStream.Length;
        }

        if (bundled.Count == 0)
        {
            // Every candidate file was unreadable — remove the empty archive.
            _fileSystem.DeleteFile(outputPath);
            _logger.LogError("No readable files to bundle from {Source}; {Skipped} file(s) skipped", options.SourcePath, skipped);

            return new CompressionResult
            {
                SourcePath = options.SourcePath,
                OutputPath = outputPath,
                Success = false,
                ErrorMessage = $"No readable files to bundle ({skipped} file(s) could not be read)."
            };
        }

        var totalOriginalSize = bundled.Sum(f => f.Length);

        // Verify after the write stream is closed (the verifier opens the archive
        // for reading) and before any deletion.
        if (options.Verify && !await _archiveVerifier.VerifyAsync(outputPath, strategy.BundleFileExtension).ConfigureAwait(false))
        {
            _logger.LogError("Archive verification failed for {Output}, original files preserved", outputPath);

            return new CompressionResult
            {
                SourcePath = options.SourcePath,
                OutputPath = outputPath,
                OriginalSize = totalOriginalSize,
                CompressedSize = compressedSize,
                Success = false,
                ErrorMessage = "Archive verification failed."
            };
        }

        // Delete only the files that were successfully bundled; skipped (unreadable)
        // files are left in place.
        foreach (var file in bundled)
        {
            _fileSystem.DeleteFile(file.FullName);
        }

        _logger.LogInformation
        (
            "Bundled {FileCount} file(s) -> {Output} ({OriginalSize:N0} -> {CompressedSize:N0} bytes)",
            bundled.Count,
            outputPath,
            totalOriginalSize,
            compressedSize
        );

        // A bundle that skipped one or more unreadable files is a partial success:
        // the archive was written and the readable originals deleted, but the caller
        // must still see a non-success result (and exit code).
        if (skipped > 0)
        {
            return new CompressionResult
            {
                SourcePath = options.SourcePath,
                OutputPath = outputPath,
                OriginalSize = totalOriginalSize,
                CompressedSize = compressedSize,
                Success = false,
                ErrorMessage = $"{skipped} file(s) could not be read and were skipped (left in place); {bundled.Count} file(s) bundled to {outputPath}."
            };
        }

        return new CompressionResult
        {
            SourcePath = options.SourcePath,
            OutputPath = outputPath,
            OriginalSize = totalOriginalSize,
            CompressedSize = compressedSize,
            Success = true
        };
    }



    private List<FileInfo> EnumerateSourceFiles(CompressionOptions options)
    {
        if (_fileSystem.FileExists(options.SourcePath))
        {
            return [_fileSystem.GetFileInfo(options.SourcePath)];
        }

        if (!_fileSystem.DirectoryExists(options.SourcePath))
        {
            throw new FileNotFoundException($"Source path not found: {options.SourcePath}");
        }

        var searchOption = options.Recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        return _fileSystem
            .EnumerateFiles(options.SourcePath, "*", searchOption)
            // Skip files that are already compressed archives so a repeated run over
            // the same directory does not re-bundle (and then delete) its own
            // output, and the single-instance lock file — this run's own live lock
            // sits in the source directory and must never be bundled/deleted
            // (#172).
            .Where(p => !RetentionService.IsArchiveFile(p) && !ProcessLock.IsLockFile(p))
            .Select(p => _fileSystem.GetFileInfo(p))
            .ToList();
    }
}
