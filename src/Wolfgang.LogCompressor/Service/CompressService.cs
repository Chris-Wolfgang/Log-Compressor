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
    public virtual async Task<IReadOnlyList<CompressionResult>> ExecuteAsync
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

        foreach (var file in filtered)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var result = await CompressFileAsync(file, options, strategy, cancellationToken).ConfigureAwait(false);
            results.Add(result);
        }

        return results;
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
            .Select(p => _fileSystem.GetFileInfo(p))
            .ToList();
    }



    private async Task<CompressionResult> CompressFileAsync
    (
        FileInfo sourceFile,
        CompressionOptions options,
        ICompressionStrategy strategy,
        CancellationToken cancellationToken
    )
    {
        var outputDir = options.OutputPath ?? sourceFile.DirectoryName ?? Directory.GetCurrentDirectory();
        var outputFileName = _fileNamer.GetCompressedFileName(sourceFile, strategy.FileExtension);
        var outputPath = Path.Combine(outputDir, outputFileName);

        try
        {
            if (options.OutputPath != null && !_fileSystem.DirectoryExists(options.OutputPath))
            {
                _fileSystem.CreateDirectory(options.OutputPath);
            }

            await using var inputStream = _fileSystem.OpenRead(sourceFile.FullName);
            await using var outputStream = _fileSystem.CreateWrite(outputPath);

            await strategy.CompressFileAsync
            (
                inputStream,
                outputStream,
                sourceFile.Name,
                cancellationToken
            ).ConfigureAwait(false);

            await outputStream.FlushAsync(cancellationToken).ConfigureAwait(false);

            var compressedSize = outputStream.Length;

            if (options.Verify && !await _archiveVerifier.VerifyAsync(outputPath, strategy.FileExtension).ConfigureAwait(false))
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
