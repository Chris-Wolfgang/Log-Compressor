using System.Formats.Tar;
using System.IO.Compression;
using Microsoft.Extensions.Logging;
using Wolfgang.LogCompressor.Abstraction;
using Wolfgang.LogCompressor.Model;

namespace Wolfgang.LogCompressor.Service;

/// <summary>
/// Extracts archives produced by <see cref="CompressService"/> and
/// <see cref="BundleService"/> — every supported format, single archives and
/// tar bundles alike.
/// </summary>
/// <remarks>
/// Safety mirror of the compress side's verify-then-delete (ADR-0003): an
/// archive is deleted only after every entry extracted without error, and
/// never when <see cref="DecompressionOptions.KeepArchives"/> is set. An
/// existing file at an extraction target fails that archive (kept intact)
/// unless <see cref="DecompressionOptions.Force"/> allows overwriting.
/// Entry paths are confined to the destination directory — an entry that
/// would escape it (zip-slip / tar-slip) fails the archive. A failed tar or
/// multi-entry extraction can leave already-written entries behind; the
/// archive itself is always kept on any failure, so no data is lost.
/// </remarks>
internal class DecompressService
{
    private readonly IFileSystem _fileSystem;
    private readonly IFileFilter _fileFilter;
    private readonly ILogger<DecompressService> _logger;

    // Longest-match-first so app.tar.gz resolves as a tar bundle, not raw gzip.
    private static readonly (string Suffix, ArchiveKind Kind)[] KnownSuffixes =
    [
        (".tar.gz", ArchiveKind.TarGz),
        (".tar.br", ArchiveKind.TarBrotli),
        (".tar.zst", ArchiveKind.TarZstd),
        (".tar.lz4", ArchiveKind.TarLz4),
        (".zip", ArchiveKind.Zip),
        (".gz", ArchiveKind.RawGz),
        (".br", ArchiveKind.RawBrotli),
        (".zst", ArchiveKind.RawZstd),
        (".lz4", ArchiveKind.RawLz4)
    ];



    private enum ArchiveKind
    {
        Unknown,
        Zip,
        TarGz,
        TarBrotli,
        TarZstd,
        TarLz4,
        RawGz,
        RawBrotli,
        RawZstd,
        RawLz4
    }



    /// <summary>
    /// Initializes a new instance of the <see cref="DecompressService"/> class.
    /// </summary>
    /// <param name="fileSystem">The file system abstraction.</param>
    /// <param name="fileFilter">The file filter for include/exclude patterns.</param>
    /// <param name="logger">The logger.</param>
    public DecompressService(IFileSystem fileSystem, IFileFilter fileFilter, ILogger<DecompressService> logger)
    {
        ArgumentNullException.ThrowIfNull(fileSystem);
        ArgumentNullException.ThrowIfNull(fileFilter);
        ArgumentNullException.ThrowIfNull(logger);

        _fileSystem = fileSystem;
        _fileFilter = fileFilter;
        _logger = logger;
    }



    /// <summary>
    /// Extracts every archive selected by the options.
    /// </summary>
    /// <param name="options">The decompression options.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>One result per archive. <see cref="CompressionResult.OriginalSize"/> is the
    /// archive size and <see cref="CompressionResult.CompressedSize"/> the extracted byte count.</returns>
    public virtual async Task<IReadOnlyList<CompressionResult>> ExecuteAsync
    (
        DecompressionOptions options,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(options);

        var archives = GetArchives(options);
        var results = new List<CompressionResult>(archives.Count);

        foreach (var archive in archives)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var result = await DecompressOneAsync(archive, options, cancellationToken).ConfigureAwait(false);

            for (var attempt = 1; !result.Success && attempt <= options.OnError.RetryCount; attempt++)
            {
                _logger.LogWarning("Retrying {Archive} (attempt {Attempt} of {Max})", archive.FullName, attempt, options.OnError.RetryCount);
                // Back off before retrying — transient conditions don't clear
                // in microseconds.
                await Task.Delay(ErrorPolicy.RetryDelay(attempt), cancellationToken).ConfigureAwait(false);
                result = await DecompressOneAsync(archive, options, cancellationToken).ConfigureAwait(false);
            }

            results.Add(result);

            if (!result.Success && options.OnError.Mode == OnErrorMode.Fail)
            {
                _logger.LogError("Stopping after failure of {Archive} (--on-error fail)", archive.FullName);
                break;
            }
        }

        return results;
    }



    private List<FileInfo> GetArchives(DecompressionOptions options)
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
        var candidates = _fileSystem
            .EnumerateFiles(options.SourcePath, "*", searchOption)
            .Where(p => DetectByName(p) != ArchiveKind.Unknown)
            .Select(p => _fileSystem.GetFileInfo(p));

        return _fileFilter
            .Apply(candidates, olderThanDays: null, minDateTime: null, maxDateTime: null, options.IncludePatterns, options.ExcludePatterns)
            .ToList();
    }



    private async Task<CompressionResult> DecompressOneAsync
    (
        FileInfo archive,
        DecompressionOptions options,
        CancellationToken cancellationToken
    )
    {
        var outputDir = options.OutputPath ?? archive.DirectoryName ?? Directory.GetCurrentDirectory();
        // FileInfo.Length is lazy — materialize it BEFORE extraction: on the
        // success path the archive is deleted before the result is built, and
        // a first Length access after deletion throws FileNotFoundException.
        var archiveSize = archive.Length;

        try
        {
            var kind = DetectByName(archive.FullName);
            if (kind == ArchiveKind.Unknown)
            {
                kind = await SniffKindAsync(archive.FullName, cancellationToken).ConfigureAwait(false);
            }

            if (!_fileSystem.DirectoryExists(outputDir))
            {
                _fileSystem.CreateDirectory(outputDir);
            }

            var extractedBytes = await ExtractAsync(kind, archive, outputDir, options, cancellationToken).ConfigureAwait(false);

            if (!options.KeepArchives)
            {
                _fileSystem.DeleteFile(archive.FullName);
            }

            _logger.LogInformation
            (
                "Extracted {Archive} -> {Output} ({ArchiveSize:N0} -> {ExtractedSize:N0} bytes)",
                archive.FullName,
                outputDir,
                archiveSize,
                extractedBytes
            );

            return new CompressionResult
            {
                SourcePath = archive.FullName,
                OutputPath = outputDir,
                OriginalSize = archiveSize,
                CompressedSize = extractedBytes,
                Success = true
            };
        }
        catch (Exception ex) when (ex is IOException or InvalidDataException or UnauthorizedAccessException or NotSupportedException)
        {
            _logger.LogError(ex, "Extraction failed for {Archive}, archive preserved: {Message}", archive.FullName, ex.Message);

            return new CompressionResult
            {
                SourcePath = archive.FullName,
                OutputPath = outputDir,
                OriginalSize = archiveSize,
                CompressedSize = 0,
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }



    private async Task<long> ExtractAsync
    (
        ArchiveKind kind,
        FileInfo archive,
        string outputDir,
        DecompressionOptions options,
        CancellationToken cancellationToken
    )
    {
        return kind switch
        {
            ArchiveKind.Zip => await ExtractZipAsync(archive, outputDir, options, cancellationToken).ConfigureAwait(false),
            ArchiveKind.TarGz => await ExtractTarAsync(archive, outputDir, options, static s => new GZipStream(s, CompressionMode.Decompress, leaveOpen: true), cancellationToken).ConfigureAwait(false),
            ArchiveKind.TarBrotli => await ExtractTarAsync(archive, outputDir, options, static s => new BrotliStream(s, CompressionMode.Decompress, leaveOpen: true), cancellationToken).ConfigureAwait(false),
            ArchiveKind.TarZstd => await ExtractTarAsync(archive, outputDir, options, static s => new ZstdSharp.DecompressionStream(s, leaveOpen: true), cancellationToken).ConfigureAwait(false),
            ArchiveKind.TarLz4 => await ExtractTarAsync(archive, outputDir, options, static s => K4os.Compression.LZ4.Streams.LZ4Stream.Decode(s, leaveOpen: true), cancellationToken).ConfigureAwait(false),
            ArchiveKind.RawGz => await ExtractRawAsync(archive, outputDir, options, static s => new GZipStream(s, CompressionMode.Decompress, leaveOpen: true), cancellationToken).ConfigureAwait(false),
            ArchiveKind.RawBrotli => await ExtractRawAsync(archive, outputDir, options, static s => new BrotliStream(s, CompressionMode.Decompress, leaveOpen: true), cancellationToken).ConfigureAwait(false),
            ArchiveKind.RawZstd => await ExtractRawAsync(archive, outputDir, options, static s => new ZstdSharp.DecompressionStream(s, leaveOpen: true), cancellationToken).ConfigureAwait(false),
            ArchiveKind.RawLz4 => await ExtractRawAsync(archive, outputDir, options, static s => K4os.Compression.LZ4.Streams.LZ4Stream.Decode(s, leaveOpen: true), cancellationToken).ConfigureAwait(false),
            // ArchiveKind.Unknown lands here: DetectByName and SniffKind both
            // failed to identify the file.
            _ => throw new NotSupportedException(
                $"Cannot identify the archive format of {archive.FullName} — no known extension and no recognizable signature (note: brotli has no signature and requires a .br extension).")
        };
    }



    private static ArchiveKind DetectByName(string path)
    {
        foreach (var (suffix, kind) in KnownSuffixes)
        {
            if (path.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
            {
                return kind;
            }
        }

        return ArchiveKind.Unknown;
    }



    private async Task<ArchiveKind> SniffKindAsync(string path, CancellationToken cancellationToken)
    {
        // Content-sniff fallback for an explicitly named file with an
        // unrecognized extension. Brotli has no magic number, so a renamed
        // .br archive cannot be sniffed — the error message says so.
        var magic = new byte[4];
        var stream = _fileSystem.OpenRead(path);
        await using (stream.ConfigureAwait(false))
        {
            var read = await stream.ReadAtLeastAsync(magic, 4, throwOnEndOfStream: false, cancellationToken).ConfigureAwait(false);
            if (read < 4)
            {
                throw new InvalidDataException($"File too small to identify: {path}");
            }
        }

        return magic switch
        {
            [0x50, 0x4B, 0x03, 0x04] => ArchiveKind.Zip,
            [0x1F, 0x8B, _, _] => ArchiveKind.RawGz,
            [0x28, 0xB5, 0x2F, 0xFD] => ArchiveKind.RawZstd,
            [0x04, 0x22, 0x4D, 0x18] => ArchiveKind.RawLz4,
            // Unrecognizable signature — ExtractAsync's Unknown arm raises the
            // descriptive error so there is a single unidentified-format path.
            _ => ArchiveKind.Unknown
        };
    }



    private string ResolveDestination(string outputDir, string entryName, DecompressionOptions options)
    {
        // Confine every entry to the destination (zip-slip / tar-slip guard):
        // the resolved full path must stay under the destination directory.
        // Trim any trailing separator first — GetFullPath preserves it, and a
        // root of "/out/" would build a "/out//" prefix that rejects every
        // valid entry as an escape (review finding on --output paths with a
        // trailing slash).
        var destinationRoot = Path.TrimEndingDirectorySeparator(Path.GetFullPath(outputDir));
        var destination = Path.GetFullPath(Path.Combine(destinationRoot, entryName));

        // Single StartsWith guard, deliberately: this is the exact sanitizer
        // shape CodeQL's cs/zipslip query recognizes as a barrier. A file
        // entry always has a name component, so destination == root only for
        // degenerate entry names ("", "."), which are rightly rejected too.
        if (!destination.StartsWith(destinationRoot + Path.DirectorySeparatorChar, StringComparison.Ordinal))
        {
            throw new InvalidDataException($"Archive entry escapes the destination directory: {entryName}");
        }

        if (_fileSystem.FileExists(destination) && !options.Force)
        {
            throw new IOException($"Extraction target already exists: {destination} (use --force to overwrite)");
        }

        var parent = Path.GetDirectoryName(destination);
        if (!string.IsNullOrEmpty(parent) && !_fileSystem.DirectoryExists(parent))
        {
            _fileSystem.CreateDirectory(parent);
        }

        return destination;
    }



    private async Task<long> ExtractZipAsync
    (
        FileInfo archive,
        string outputDir,
        DecompressionOptions options,
        CancellationToken cancellationToken
    )
    {
        long total = 0;

        var stream = _fileSystem.OpenRead(archive.FullName);
        await using (stream.ConfigureAwait(false))
        {
            using var zip = new ZipArchive(stream, ZipArchiveMode.Read);

            foreach (var entry in zip.Entries)
            {
                // Directory entries have an empty Name; their paths are
                // created on demand by the file entries beneath them.
                if (entry.Name.Length == 0)
                {
                    continue;
                }

                var destination = ResolveDestination(outputDir, entry.FullName, options);

                var entryStream = await entry.OpenAsync(cancellationToken).ConfigureAwait(false);
                await using (entryStream.ConfigureAwait(false))
                {
                    total += await WriteEntryAsync(entryStream, destination, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        return total;
    }



    private async Task<long> ExtractTarAsync
    (
        FileInfo archive,
        string outputDir,
        DecompressionOptions options,
        Func<Stream, Stream> decompressorFactory,
        CancellationToken cancellationToken
    )
    {
        long total = 0;

        var stream = _fileSystem.OpenRead(archive.FullName);
        await using (stream.ConfigureAwait(false))
        {
            var decompressed = decompressorFactory(stream);
            await using (decompressed.ConfigureAwait(false))
            {
                await using var tar = new TarReader(decompressed);

                while (await tar.GetNextEntryAsync(cancellationToken: cancellationToken).ConfigureAwait(false) is { } entry)
                {
                    if (entry.EntryType is not TarEntryType.RegularFile and not TarEntryType.V7RegularFile)
                    {
                        continue;
                    }

                    var destination = ResolveDestination(outputDir, entry.Name, options);

                    if (entry.DataStream is null)
                    {
                        // Zero-byte entry: create the empty file.
                        var empty = _fileSystem.CreateWrite(destination);
                        await using (empty.ConfigureAwait(false))
                        {
                            // Opening the stream creates the file - nothing to write.
                        }

                        continue;
                    }

                    total += await WriteEntryAsync(entry.DataStream, destination, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        return total;
    }



    private async Task<long> ExtractRawAsync
    (
        FileInfo archive,
        string outputDir,
        DecompressionOptions options,
        Func<Stream, Stream> decompressorFactory,
        CancellationToken cancellationToken
    )
    {
        // Raw single-stream formats carry no entry name; the output name is
        // the archive name minus its compression suffix. The original file
        // extension is not recoverable — compress derives the archive name
        // from the source's name WITHOUT its extension (FileNamingService),
        // so app.log became app-<timestamp>.gz and extracts as
        // app-<timestamp>.
        var suffixLength = GetMatchedSuffixLength(archive.Name);
        // A sniffed file with an unrecognized extension keeps its name plus a
        // marker — stripping nothing would make the destination the archive
        // itself (self-overwrite under --force).
        var outputName = suffixLength > 0
            ? archive.Name[..^suffixLength]
            : archive.Name + ".extracted";
        var destination = ResolveDestination(outputDir, outputName, options);

        var stream = _fileSystem.OpenRead(archive.FullName);
        await using (stream.ConfigureAwait(false))
        {
            var decompressed = decompressorFactory(stream);
            await using (decompressed.ConfigureAwait(false))
            {
                return await WriteEntryAsync(decompressed, destination, cancellationToken).ConfigureAwait(false);
            }
        }
    }



    private static int GetMatchedSuffixLength(string name)
    {
        foreach (var (suffix, _) in KnownSuffixes)
        {
            if (name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
            {
                return suffix.Length;
            }
        }

        // Sniffed file with an unknown extension: keep the name as-is by
        // stripping nothing, but disambiguate against the source itself.
        return 0;
    }



    private async Task<long> WriteEntryAsync(Stream source, string destination, CancellationToken cancellationToken)
    {
        var output = _fileSystem.CreateWrite(destination);
        await using (output.ConfigureAwait(false))
        {
            await source.CopyToAsync(output, cancellationToken).ConfigureAwait(false);
            return output.Length;
        }
    }
}
