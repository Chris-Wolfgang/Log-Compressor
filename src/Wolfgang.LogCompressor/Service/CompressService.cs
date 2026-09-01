using Microsoft.Extensions.Logging;
using Wolfgang.LogCompressor.Abstraction;
using Wolfgang.LogCompressor.Model;
using Wolfgang.LogCompressor.Service.Compression;

namespace Wolfgang.LogCompressor.Service;

/// <summary>
/// Compresses files individually, producing one archive per source file.
/// </summary>
internal class CompressService
{
    private readonly IFileSystem _fileSystem;
    private readonly IFileFilter _fileFilter;
    private readonly IFileNamer _fileNamer;
    private readonly IArchiveVerifier _archiveVerifier;
    private readonly CompressionStrategyFactory _strategyFactory;
    private readonly ILogger<CompressService> _logger;



    /// <summary>
    /// Initializes a new instance of the <see cref="CompressService"/> class.
    /// </summary>
    /// <param name="fileSystem">The file system abstraction.</param>
    /// <param name="fileFilter">The file filter service.</param>
    /// <param name="fileNamer">The file naming service.</param>
    /// <param name="archiveVerifier">The archive verifier.</param>
    /// <param name="strategyFactory">The compression strategy factory.</param>
    /// <param name="logger">The logger.</param>
    public CompressService
    (
        IFileSystem fileSystem,
        IFileFilter fileFilter,
        IFileNamer fileNamer,
        IArchiveVerifier archiveVerifier,
        CompressionStrategyFactory strategyFactory,
        ILogger<CompressService> logger
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
    /// Compresses files according to the specified options, one archive per source file.
    /// </summary>
    /// <param name="options">The compression options.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A list of compression results.</returns>
#pragma warning disable MA0051 // Linear orchestration with parallel logging; splitting hurts readability
    public virtual async Task<IReadOnlyList<CompressionResult>> ExecuteAsync
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

        var results = new List<CompressionResult>(filtered.Count);
        // Output paths already produced THIS run: with --name every file shares
        // a base name, and even without it a recursed tree can hold same-named
        // files with identical mtimes — either way two sources must never
        // resolve to one archive path (the second would overwrite the first's
        // archive after its original was already deleted). Keyed by FULL path,
        // not file name: same-named recursed files landing in different
        // directories (no --output) don't collide and must not be renamed.
        var usedPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var file in filtered)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var result = await CompressWithPolicyAsync(file, options, strategy, usedPaths, cancellationToken).ConfigureAwait(false);
            results.Add(result);

            if (!result.Success && options.OnError.Mode == OnErrorMode.Fail)
            {
                _logger.LogError("Stopping after failure of {File} (--on-error fail)", file.FullName);
                break;
            }
        }

        return results;
    }



    private async Task<CompressionResult> CompressWithPolicyAsync
    (
        FileInfo sourceFile,
        CompressionOptions options,
        ICompressionStrategy strategy,
        HashSet<string> usedPaths,
        CancellationToken cancellationToken
    )
    {
        var outputDir = options.OutputPath ?? sourceFile.DirectoryName ?? Directory.GetCurrentDirectory();
        var outputFileName = _fileNamer.GetCompressedFileName(sourceFile, strategy.FileExtension, options.TimestampSource, options.NamePrefix);
        var outputPath = MakeUniqueThisRun(outputDir, outputFileName, usedPaths);

        // Never overwrite an existing archive — a name collision (e.g. two
        // same-named sources sharing a last-write second under --output) would
        // otherwise clobber the first archive and then delete both originals.
        // Checked once BEFORE the first attempt, not per attempt: a
        // pre-existing archive is deterministic (retrying cannot help), and
        // clearing it up front lets retries safely delete whatever a failed
        // attempt of OURS left at this now-reserved path.
        if (_fileSystem.FileExists(outputPath))
        {
            _logger.LogError("Output archive already exists, skipping {Source} to avoid overwrite: {Output}", sourceFile.FullName, outputPath);

            return new CompressionResult
            {
                SourcePath = sourceFile.FullName,
                OutputPath = outputPath,
                OriginalSize = sourceFile.Length,
                Success = false,
                ErrorMessage = $"Output archive already exists: {outputPath}"
            };
        }

        var result = await CompressFileAsync(sourceFile, outputPath, options, strategy, cancellationToken).ConfigureAwait(false);

        for (var attempt = 1; !result.Success && attempt <= options.OnError.RetryCount; attempt++)
        {
            _logger.LogWarning("Retrying {File} (attempt {Attempt} of {Max})", sourceFile.FullName, attempt, options.OnError.RetryCount);

            // Every attempt reuses the SAME reserved path. The path was free
            // before the first attempt, so anything here now is the failed
            // attempt's partial or unverified output — clear it for a clean
            // retry.
            if (_fileSystem.FileExists(outputPath))
            {
                _fileSystem.DeleteFile(outputPath);
            }

            result = await CompressFileAsync(sourceFile, outputPath, options, strategy, cancellationToken).ConfigureAwait(false);
        }

        return result;
    }



    private static string MakeUniqueThisRun(string outputDir, string fileName, HashSet<string> usedPaths)
    {
        var path = Path.Combine(outputDir, fileName);
        if (usedPaths.Add(path))
        {
            return path;
        }

        // Insert the counter before the FINAL extension only: a dotted stem
        // ("my.app-….zip") must become "my.app-…-2.zip", not "my-2.app-….zip".
        var stem = fileName;
        var extension = string.Empty;
        var dot = fileName.LastIndexOf('.');
        if (dot > 0)
        {
            stem = fileName[..dot];
            extension = fileName[dot..];
        }

        for (var i = 2; ; i++)
        {
            var candidate = Path.Combine(outputDir, $"{stem}-{i}{extension}");
            if (usedPaths.Add(candidate))
            {
                return candidate;
            }
        }
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
            // the same directory does not re-compress (and then delete) its own
            // output, and the single-instance lock file — this run's own live lock
            // sits in the source directory and must never be compressed/deleted
            // (#172).
            .Where(p => !RetentionService.IsArchiveFile(p) && !ProcessLock.IsLockFile(p))
            .Select(p => _fileSystem.GetFileInfo(p))
            .ToList();
    }



#pragma warning disable MA0051 // Linear compress/verify/delete with try/catch; splitting hurts readability
    private async Task<CompressionResult> CompressFileAsync
#pragma warning restore MA0051
    (
        FileInfo sourceFile,
        string outputPath,
        CompressionOptions options,
        ICompressionStrategy strategy,
        CancellationToken cancellationToken
    )
    {
        try
        {
            if (options.OutputPath != null && !_fileSystem.DirectoryExists(options.OutputPath))
            {
                _fileSystem.CreateDirectory(options.OutputPath);
            }

            long compressedSize;

            await using (var inputStream = _fileSystem.OpenRead(sourceFile.FullName))
            await using (var outputStream = _fileSystem.CreateWrite(outputPath))
            {
                await strategy.CompressFileAsync
                (
                    inputStream,
                    outputStream,
                    sourceFile.Name,
                    cancellationToken
                ).ConfigureAwait(false);

                await outputStream.FlushAsync(cancellationToken).ConfigureAwait(false);

                compressedSize = outputStream.Length;
            }

            // Verify (and delete the original) only after the input/output streams
            // are closed. The verifier opens the archive for reading, which fails
            // while the write handle is still open, and the source can't be deleted
            // while its read handle is open. Both must run after the using block.
            // The source length lets the verifier catch truncated gz/br output
            // (their decompressors return partial data instead of failing).
            if (options.Verify && !await _archiveVerifier.VerifyAsync(outputPath, strategy.FileExtension, sourceFile.Length).ConfigureAwait(false))
            {
                _logger.LogError("Archive verification failed for {Output}, original file preserved", outputPath);

                return new CompressionResult
                {
                    SourcePath = sourceFile.FullName,
                    OutputPath = outputPath,
                    OriginalSize = sourceFile.Length,
                    CompressedSize = compressedSize,
                    Success = false,
                    ErrorMessage = "Archive verification failed."
                };
            }

            _fileSystem.DeleteFile(sourceFile.FullName);

            _logger.LogInformation
            (
                "Compressed {Source} -> {Output} ({OriginalSize:N0} -> {CompressedSize:N0} bytes)",
                sourceFile.FullName,
                outputPath,
                sourceFile.Length,
                compressedSize
            );

            return new CompressionResult
            {
                SourcePath = sourceFile.FullName,
                OutputPath = outputPath,
                OriginalSize = sourceFile.Length,
                CompressedSize = compressedSize,
                Success = true
            };
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Failed to compress {Source}: {Message}", sourceFile.FullName, ex.Message);

            return new CompressionResult
            {
                SourcePath = sourceFile.FullName,
                OutputPath = outputPath,
                OriginalSize = sourceFile.Length,
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}
