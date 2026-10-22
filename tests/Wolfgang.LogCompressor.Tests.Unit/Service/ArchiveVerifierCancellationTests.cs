using System.IO.Compression;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Wolfgang.LogCompressor.Model;
using Wolfgang.LogCompressor.Service;
using Wolfgang.LogCompressor.Service.Compression;

namespace Wolfgang.LogCompressor.Tests.Unit.Service;

/// <summary>
/// Verification observes the run's cancellation token and reports a cancellation as
/// <see cref="OperationCanceledException"/>, never as a failed verification (#354).
/// </summary>
public sealed class ArchiveVerifierCancellationTests : IDisposable
{
    private readonly TempDirectory _tempDir = new();
    private readonly ArchiveVerifier _sut = new(Substitute.For<ILogger<ArchiveVerifier>>());



    [Theory]
    [InlineData(CompressionFormat.Zip, false)]
    [InlineData(CompressionFormat.Gz, false)]
    [InlineData(CompressionFormat.Brotli, false)]
    [InlineData(CompressionFormat.Zstd, false)]
    [InlineData(CompressionFormat.Lz4, false)]
    [InlineData(CompressionFormat.Zip, true)]
    [InlineData(CompressionFormat.Gz, true)]
    [InlineData(CompressionFormat.Brotli, true)]
    [InlineData(CompressionFormat.Zstd, true)]
    [InlineData(CompressionFormat.Lz4, true)]
    internal async Task VerifyAsync_when_tokenCanceled_expected_throwsOperationCanceled(CompressionFormat format, bool bundle)
    {
        var (path, extension) = await CreateArchiveAsync(format, bundle);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>
        (
            () => _sut.VerifyAsync(path, extension, expectedUncompressedSize: null, cts.Token)
        );
    }



    [Fact]
    public async Task VerifyAsync_when_unknownFormatAndTokenCanceled_expected_throwsOperationCanceled()
    {
        var path = _tempDir.WriteFile("plain.bin", "not an archive");
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>
        (
            () => _sut.VerifyAsync(path, "unknown", expectedUncompressedSize: null, cts.Token)
        );
    }



    [Theory]
    [InlineData(CompressionFormat.Gz, true)]
    [InlineData(CompressionFormat.Gz, false)]
    internal async Task VerifyAsync_when_tokenNotCanceled_expected_validArchiveVerifies(CompressionFormat format, bool bundle)
    {
        var (path, extension) = await CreateArchiveAsync(format, bundle);
        using var cts = new CancellationTokenSource();

        Assert.True(await _sut.VerifyAsync(path, extension, expectedUncompressedSize: null, cts.Token));
    }



    [Fact]
    public async Task VerifyAsync_when_tarGzTrailerCorrupt_expected_falseFromTheSinglePass()
    {
        // The tar walk and the gzip CRC share one read: a CRC mismatch in the
        // trailer must still fail even though the tar structure is intact.
        var (path, extension) = await CreateArchiveAsync(CompressionFormat.Gz, bundle: true);
        var bytes = await File.ReadAllBytesAsync(path);
        bytes[^8] ^= 0xFF;
        await File.WriteAllBytesAsync(path, bytes);

        Assert.False(await _sut.VerifyAsync(path, extension));
    }



    private async Task<(string Path, string Extension)> CreateArchiveAsync(CompressionFormat format, bool bundle)
    {
        var strategy = new CompressionStrategyFactory().Create(format, CompressionLevel.Fastest);
        var extension = bundle ? strategy.BundleFileExtension : strategy.FileExtension;
        var path = Path.Combine(_tempDir.Path, Guid.NewGuid().ToString("N") + "." + extension);
        var content = System.Text.Encoding.UTF8.GetBytes(new string('a', 2048));

        var output = File.Create(path);
        await using (output.ConfigureAwait(false))
        {
            if (bundle)
            {
                await strategy.CompressFilesAsync(Inputs(content), output);
            }
            else
            {
                using var input = new MemoryStream(content);
                await strategy.CompressFileAsync(input, output, "a.log");
            }
        }

        return (path, extension);
    }



    private static async IAsyncEnumerable<(Stream Stream, string EntryName)> Inputs(byte[] content)
    {
        await Task.Yield();
        yield return (new MemoryStream(content), "a.log");
        yield return (new MemoryStream(content), "b.log");
    }



    public void Dispose()
    {
        _tempDir.Dispose();
    }
}
