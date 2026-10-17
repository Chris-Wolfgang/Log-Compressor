using System.IO.Compression;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Wolfgang.LogCompressor.Service;

namespace Wolfgang.LogCompressor.Tests.Unit.Service;

/// <summary>
/// <see cref="ArchiveVerifier.VerifyAsync"/> only returns <c>false</c> on failure, so the reason an
/// archive was rejected is observable solely through the logged exception. These tests pin that
/// reason, so a verifier that rejects an archive for the wrong cause (or with no cause) is caught.
/// </summary>
public sealed class ArchiveVerifierFailureReasonTests : IDisposable
{
    private readonly ILogger<ArchiveVerifier> _logger = Substitute.For<ILogger<ArchiveVerifier>>();
    private readonly ArchiveVerifier _sut;
    private readonly string _tempDir;



    public ArchiveVerifierFailureReasonTests()
    {
        _sut = new ArchiveVerifier(_logger);
        _tempDir = Path.Combine(Path.GetTempPath(), "ArchiveVerifierFailureReasonTests_" + Guid.NewGuid());
        Directory.CreateDirectory(_tempDir);
    }



    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
        {
            Directory.Delete(_tempDir, recursive: true);
        }
    }



    [Fact]
    public async Task VerifyAsync_when_zipEntryDataCorrupted_expected_logsCrcAndLengthMismatch()
    {
        // A stored entry keeps its bytes verbatim, so one flipped byte leaves the central
        // directory readable and only the entry's CRC-32 check can reject the archive.
        var archivePath = Path.Combine(_tempDir, "bad-entry.zip");
        var content = "zip-entry-payload-0123456789"u8.ToArray();
        await using (var fileStream = File.Create(archivePath))
        {
            using var archive = new ZipArchive(fileStream, ZipArchiveMode.Create, leaveOpen: true);
            var entry = archive.CreateEntry("test.txt", CompressionLevel.NoCompression);
            var entryStream = await entry.OpenAsync();
            await using (entryStream)
            {
                await entryStream.WriteAsync(content);
            }
        }

        var bytes = await File.ReadAllBytesAsync(archivePath);
        var offset = bytes.AsSpan().IndexOf(content);
        Assert.True(offset > 0);
        bytes[offset] ^= 0xFF;
        await File.WriteAllBytesAsync(archivePath, bytes);

        var result = await _sut.VerifyAsync(archivePath, "zip");

        Assert.False(result);
        AssertLoggedFailureContains("test.txt");
        AssertLoggedFailureContains("does not match its recorded CRC-32/length");
    }



    [Fact]
    public async Task VerifyAsync_when_gzSmallerThanMinimalMember_expected_logsTooSmallWithActualLength()
    {
        // The canonical empty-content gzip member is 20 bytes; truncated to 16 it still
        // "decompresses" to zero bytes without error, so only the 18-byte floor rejects it.
        byte[] emptyGz =
        [
            0x1F, 0x8B, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0A,
            0x03, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        ];
        var archivePath = Path.Combine(_tempDir, "tiny.gz");
        await File.WriteAllBytesAsync(archivePath, emptyGz[..16]);

        var result = await _sut.VerifyAsync(archivePath, "gz");

        Assert.False(result);
        AssertLoggedFailureContains("too small to be complete");
        AssertLoggedFailureContains("16 bytes");
    }



    [Fact]
    public async Task VerifyAsync_when_gzTruncatedInsideTrailer_expected_logsTrailerMismatch()
    {
        // GZipStream rejects a corrupted trailer itself, but silently returns partial data when the
        // file is cut short. Cutting the last 4 bytes drops the length field, so only the verifier's
        // own CRC-32/length comparison can reject the archive.
        var archivePath = Path.Combine(_tempDir, "truncated.gz");
        var content = "test content"u8.ToArray();
        await using (var fileStream = File.Create(archivePath))
        {
            await using var gzip = new GZipStream(fileStream, CompressionLevel.Optimal, leaveOpen: true);
            await gzip.WriteAsync(content);
        }

        var bytes = await File.ReadAllBytesAsync(archivePath);
        await File.WriteAllBytesAsync(archivePath, bytes[..^4]);

        var result = await _sut.VerifyAsync(archivePath, "gz");

        Assert.False(result);
        AssertLoggedFailureContains("Gzip trailer (CRC-32/length) does not match");
    }



    [Fact]
    public async Task VerifyAsync_when_gzDecompressesToUnexpectedSize_expected_logsBothLengths()
    {
        var archivePath = Path.Combine(_tempDir, "test.gz");
        var content = "test content"u8.ToArray();
        await using (var fileStream = File.Create(archivePath))
        {
            await using var gzip = new GZipStream(fileStream, CompressionLevel.Optimal, leaveOpen: true);
            await gzip.WriteAsync(content);
        }

        var result = await _sut.VerifyAsync(archivePath, "gz", content.Length + 1);

        Assert.False(result);
        AssertLoggedFailureContains("does not match the source length");
        AssertLoggedFailureContains($"({content.Length})");
        AssertLoggedFailureContains($"({content.Length + 1})");
    }



    private void AssertLoggedFailureContains(string expected)
    {
        var messages = _logger.ReceivedCalls()
            .Where(call => string.Equals(call.GetMethodInfo().Name, nameof(ILogger.Log), StringComparison.Ordinal))
            .Select(call => call.GetArguments()[3])
            .OfType<Exception>()
            .Select(exception => exception.Message)
            .ToList();

        Assert.Contains(messages, message => message.Contains(expected, StringComparison.Ordinal));
    }
}
