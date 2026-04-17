using System.Formats.Tar;
using System.IO.Compression;
using Microsoft.Extensions.Logging;
using Wolfgang.LogCompressor.Abstraction;

namespace Wolfgang.LogCompressor.Service;

/// <summary>
/// Verifies compressed archive integrity by attempting to read the archive contents.
/// </summary>
internal sealed class ArchiveVerifier : IArchiveVerifier
{
    private readonly ILogger<ArchiveVerifier> _logger;



    /// <summary>
    /// Initializes a new instance of the <see cref="ArchiveVerifier"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public ArchiveVerifier(ILogger<ArchiveVerifier> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        _logger = logger;
    }



    /// <inheritdoc />
    public async Task<bool> VerifyAsync(string archivePath, string format)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(archivePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(format);

        try
        {
            switch (format.ToLowerInvariant())
            {
                case "zip":
                    await VerifyZipAsync(archivePath).ConfigureAwait(false);
                    break;
                case "tar.gz":
                    await VerifyTarStreamAsync<GZipStream>(archivePath).ConfigureAwait(false);
                    break;
                case "tar.br":
                    await VerifyTarStreamAsync<BrotliStream>(archivePath).ConfigureAwait(false);
                    break;
                case "gz":
                    await VerifyDecompressionAsync<GZipStream>(archivePath).ConfigureAwait(false);
                    break;
                case "br":
                    await VerifyDecompressionAsync<BrotliStream>(archivePath).ConfigureAwait(false);
                    break;
                default:
                    await VerifyReadableAsync(archivePath).ConfigureAwait(false);
                    break;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Archive verification failed for {Path}: {Message}", archivePath, ex.Message);
            return false;
        }
    }



    private static async Task VerifyZipAsync(string path)
    {
        await using var stream = File.OpenRead(path);
        using var archive = new ZipArchive(stream, ZipArchiveMode.Read);

        foreach (var entry in archive.Entries)
        {
            var entryStream = await entry.OpenAsync().ConfigureAwait(false);
            await using (entryStream.ConfigureAwait(false))
            {
                await entryStream.CopyToAsync(Stream.Null).ConfigureAwait(false);
            }
        }
    }



    private static async Task VerifyTarStreamAsync<TStream>(string path)
        where TStream : Stream
    {
        await using var fileStream = File.OpenRead(path);
        var decompressionStream = (Stream)Activator.CreateInstance
        (
            typeof(TStream),
            fileStream,
            CompressionMode.Decompress,
            true
        )!;

        await using (decompressionStream.ConfigureAwait(false))
        {
            await using var tarReader = new TarReader(decompressionStream);

            while (await tarReader.GetNextEntryAsync().ConfigureAwait(false) is { } entry)
            {
                if (entry.DataStream != null)
                {
                    await entry.DataStream.CopyToAsync(Stream.Null).ConfigureAwait(false);
                }
            }
        }
    }



    private static async Task VerifyDecompressionAsync<TStream>(string path)
        where TStream : Stream
    {
        await using var fileStream = File.OpenRead(path);
        var decompressionStream = (Stream)Activator.CreateInstance
        (
            typeof(TStream),
            fileStream,
            CompressionMode.Decompress,
            true
        )!;

        await using (decompressionStream.ConfigureAwait(false))
        {
            await decompressionStream.CopyToAsync(Stream.Null).ConfigureAwait(false);
        }
    }



    private static async Task VerifyReadableAsync(string path)
    {
        await using var stream = File.OpenRead(path);
        await stream.CopyToAsync(Stream.Null).ConfigureAwait(false);
    }
}
