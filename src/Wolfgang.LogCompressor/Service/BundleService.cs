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
    private readonly CompressionStrategyFactory _strategyFactory;
    private readonly ILogger<BundleService> _logger;



    /// <summary>
    /// Initializes a new instance of the <see cref="BundleService"/> class.
    /// </summary>
    /// <param name="fileSystem">The file system abstraction.</param>
    /// <param name="fileFilter">The file filter service.</param>
    /// <param name="fileNamer">The file naming service.</param>
    /// <param name="strategyFactory">The compression strategy factory.</param>
    /// <param name="logger">The logger.</param>
    public BundleService
    (
        IFileSystem fileSystem,
        IFileFilter fileFilter,
        IFileNamer fileNamer,
        CompressionStrategyFactory strategyFactory,
        ILogger<BundleService> logger
    )
    {
        _fileSystem = fileSystem;
        _fileFilter = fileFilter;
        _fileNamer = fileNamer;
        _strategyFactory = strategyFactory;
        _logger = logger;
    }



    /// <summary>
    /// Bundles files into a single archive according to the specified options.
    /// </summary>
    /// <param name="options">The compression options.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The compression result.</returns>
    public virtual async Task<CompressionResult> ExecuteAsync
    (
        CompressionOptions options,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(options);

        var strategy = _strategyFactory.Create(options.Format, options.Level);
        var files = EnumerateSourceFiles(options);
        var filtered = _fileFilter.Apply(files, options.OlderThanDays, options.MinDateTime, options.MaxDateTime);

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

        var folderName = Path.GetFileName(options.SourcePath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        var outputDir = options.OutputPath ?? Path.GetDirectoryName(options.SourcePath)!;
        var outputFileName = _fileNamer.GetBundleFileName(folderName, filtered, strategy.BundleFileExtension);
        var outputPath = Path.Combine(outputDir, outputFileName);

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



    private async Task<CompressionResult> CompressAndDeleteAsync
    (
        IReadOnlyList<FileInfo> filtered,
        string outputPath,
        ICompressionStrategy strategy,
        CompressionOptions options,
        CancellationToken cancellationToken
    )
    {
        var streams = new List<Stream>(filtered.Count);
        var inputs = new List<(Stream Stream, string EntryName)>(filtered.Count);

        try
        {
            foreach (var file in filtered)
            {
                var stream = _fileSystem.OpenRead(file.FullName);
                streams.Add(stream);
                inputs.Add((stream, file.Name));
            }

            await using var outputStream = _fileSystem.CreateWrite(outputPath);

            await strategy.CompressFilesAsync(inputs, outputStream, cancellationToken).ConfigureAwait(false);
            await outputStream.FlushAsync(cancellationToken).ConfigureAwait(false);

            var totalOriginalSize = filtered.Sum(f => f.Length);
            var compressedSize = outputStream.Length;

            foreach (var file in filtered)
            {
                _fileSystem.DeleteFile(file.FullName);
            }

            _logger.LogInformation
            (
                "Bundled {FileCount} file(s) -> {Output} ({OriginalSize:N0} -> {CompressedSize:N0} bytes)",
                filtered.Count,
                outputPath,
                totalOriginalSize,
                compressedSize
            );

            return new CompressionResult
            {
                SourcePath = options.SourcePath,
                OutputPath = outputPath,
                OriginalSize = totalOriginalSize,
                CompressedSize = compressedSize,
                Success = true
            };
        }
        finally
        {
            foreach (var stream in streams)
            {
                await stream.DisposeAsync().ConfigureAwait(false);
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
            .Select(p => _fileSystem.GetFileInfo(p))
            .ToList();
    }
}
