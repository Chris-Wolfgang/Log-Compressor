using System.Formats.Tar;
using System.IO.Compression;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Wolfgang.LogCompressor.Service;

namespace Wolfgang.LogCompressor.Tests.Unit.Service;

public sealed class ArchiveVerifierTests : IDisposable
{
    private readonly ArchiveVerifier _sut;
    private readonly string _tempDir;



    public ArchiveVerifierTests()
    {
        _sut = new ArchiveVerifier(Substitute.For<ILogger<ArchiveVerifier>>());
        _tempDir = Path.Combine(Path.GetTempPath(), "ArchiveVerifierTests_" + Guid.NewGuid());
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
    public async Task VerifyAsync_when_validZipFile_expected_true()
    {
        var archivePath = Path.Combine(_tempDir, "test.zip");
        await CreateValidZipAsync(archivePath);

        var result = await _sut.VerifyAsync(archivePath, "zip");

        Assert.True(result);
    }



    [Fact]
    public async Task VerifyAsync_when_validGzFile_expected_true()
    {
        var archivePath = Path.Combine(_tempDir, "test.gz");
        await CreateValidGzAsync(archivePath);

        var result = await _sut.VerifyAsync(archivePath, "gz");

        Assert.True(result);
    }



    [Fact]
    public async Task VerifyAsync_when_validBrotliFile_expected_true()
    {
        var archivePath = Path.Combine(_tempDir, "test.br");
        await CreateValidBrotliAsync(archivePath);

        var result = await _sut.VerifyAsync(archivePath, "br");

        Assert.True(result);
    }



    [Fact]
    public async Task VerifyAsync_when_validTarGzFile_expected_true()
    {
        var archivePath = Path.Combine(_tempDir, "test.tar.gz");
        await CreateValidTarGzAsync(archivePath);

        var result = await _sut.VerifyAsync(archivePath, "tar.gz");

        Assert.True(result);
    }



    [Fact]
    public async Task VerifyAsync_when_validTarBrFile_expected_true()
    {
        var archivePath = Path.Combine(_tempDir, "test.tar.br");
        await CreateValidTarBrAsync(archivePath);

        var result = await _sut.VerifyAsync(archivePath, "tar.br");

        Assert.True(result);
    }



    [Fact]
    public async Task VerifyAsync_when_corruptedZipFile_expected_false()
    {
        var archivePath = Path.Combine(_tempDir, "corrupt.zip");
        await File.WriteAllBytesAsync(archivePath, [0x00, 0x01, 0x02, 0x03, 0xFF, 0xFE]);

        var result = await _sut.VerifyAsync(archivePath, "zip");

        Assert.False(result);
    }



    [Fact]
    public async Task VerifyAsync_when_corruptedGzFile_expected_false()
    {
        var archivePath = Path.Combine(_tempDir, "corrupt.gz");
        await File.WriteAllBytesAsync(archivePath, [0x00, 0x01, 0x02, 0x03, 0xFF, 0xFE]);

        var result = await _sut.VerifyAsync(archivePath, "gz");

        Assert.False(result);
    }



    [Fact]
    public async Task VerifyAsync_when_corruptedBrotliFile_expected_false()
    {
        var archivePath = Path.Combine(_tempDir, "corrupt.br");

        // Brotli requires specific magic bytes to fail; generate a large enough random payload
        var random = new Random(42);
        var garbage = new byte[1024];
        random.NextBytes(garbage);
        // Ensure the first byte is not a valid Brotli window size indicator
        garbage[0] = 0xFF;
        await File.WriteAllBytesAsync(archivePath, garbage);

        var result = await _sut.VerifyAsync(archivePath, "br");

        Assert.False(result);
    }



    [Fact]
    public async Task VerifyAsync_when_corruptedTarGzFile_expected_false()
    {
        var archivePath = Path.Combine(_tempDir, "corrupt.tar.gz");
        await File.WriteAllBytesAsync(archivePath, [0x00, 0x01, 0x02, 0x03, 0xFF, 0xFE]);

        var result = await _sut.VerifyAsync(archivePath, "tar.gz");

        Assert.False(result);
    }



    [Fact]
    public async Task VerifyAsync_when_nullPath_expected_throwsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>
        (
            () => _sut.VerifyAsync(null!, "zip")
        );
    }



    [Fact]
    public async Task VerifyAsync_when_emptyPath_expected_throwsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentException>
        (
            () => _sut.VerifyAsync("", "zip")
        );
    }



    [Fact]
    public async Task VerifyAsync_when_nullFormat_expected_throwsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>
        (
            () => _sut.VerifyAsync("some-path.zip", null!)
        );
    }



    [Fact]
    public async Task VerifyAsync_when_emptyFormat_expected_throwsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentException>
        (
            () => _sut.VerifyAsync("some-path.zip", "")
        );
    }



    [Fact]
    public async Task VerifyAsync_when_unknownFormat_expected_fallbackReadable()
    {
        var filePath = Path.Combine(_tempDir, "test.txt");
        await File.WriteAllTextAsync(filePath, "hello world");

        var result = await _sut.VerifyAsync(filePath, "unknown");

        Assert.True(result);
    }



    [Fact]
    public async Task VerifyAsync_when_fileNotFound_expected_false()
    {
        var result = await _sut.VerifyAsync
        (
            Path.Combine(_tempDir, "nonexistent.zip"),
            "zip"
        );

        Assert.False(result);
    }



    [Fact]
    public async Task VerifyAsync_when_truncatedGzFile_expected_false()
    {
        // Regression for the fuzz finding (#68): GZipStream returns partial
        // data on truncated input instead of throwing, so the verifier must
        // check the gzip CRC/length trailer itself.
        var archivePath = Path.Combine(_tempDir, "test.gz");
        await CreateValidGzAsync(archivePath);
        await TruncateAsync(archivePath, bytesToRemove: 4);

        var result = await _sut.VerifyAsync(archivePath, "gz");

        Assert.False(result);
    }



    [Fact]
    public async Task VerifyAsync_when_truncatedBrotliFile_with_expectedSize_expected_false()
    {
        // Brotli has no checksum in the format; the expected-size comparison
        // is what catches a truncated stream that decodes without error.
        var archivePath = Path.Combine(_tempDir, "test.br");
        await CreateValidBrotliAsync(archivePath);
        await TruncateAsync(archivePath, bytesToRemove: 2);

        var result = await _sut.VerifyAsync(archivePath, "br", "test content"u8.Length);

        Assert.False(result);
    }



    [Fact]
    public async Task VerifyAsync_when_gzDecompressesToUnexpectedSize_expected_false()
    {
        var archivePath = Path.Combine(_tempDir, "test.gz");
        await CreateValidGzAsync(archivePath);

        var result = await _sut.VerifyAsync(archivePath, "gz", "test content"u8.Length + 1);

        Assert.False(result);
    }



    [Fact]
    public async Task VerifyAsync_when_truncatedTarGzFile_expected_false()
    {
        var archivePath = Path.Combine(_tempDir, "test.tar.gz");
        await CreateValidTarGzAsync(archivePath);
        await TruncateAsync(archivePath, bytesToRemove: 4);

        var result = await _sut.VerifyAsync(archivePath, "tar.gz");

        Assert.False(result);
    }



    [Fact]
    public async Task VerifyAsync_when_negativeExpectedSize_expected_throws()
    {
        var archivePath = Path.Combine(_tempDir, "test.gz");
        await CreateValidGzAsync(archivePath);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>
        (
            () => _sut.VerifyAsync(archivePath, "gz", -1)
        );
    }



    [Fact]
    public async Task VerifyAsync_when_gzIsizeTrailerPatched_expected_false()
    {
        // A stream whose CRC matches but whose ISIZE field lies: only the
        // length-trailer comparison can catch this, so patch the last 4 bytes
        // of an otherwise valid archive.
        var archivePath = Path.Combine(_tempDir, "test.gz");
        await CreateValidGzAsync(archivePath);
        var bytes = await File.ReadAllBytesAsync(archivePath);
        bytes[^4] ^= 0xFF;
        await File.WriteAllBytesAsync(archivePath, bytes);

        var result = await _sut.VerifyAsync(archivePath, "gz");

        Assert.False(result);
    }



    [Fact]
    public async Task VerifyAsync_when_gzSmallerThanMinimalMember_expected_false()
    {
        // The canonical empty-content gzip member is 20 bytes (10-byte header,
        // "\x03\x00" empty final deflate block, 8-byte trailer). Truncated to
        // 16 bytes it still "decompresses" to zero bytes without error, so the
        // 18-byte size floor is the check that fires.
        byte[] emptyGz =
        [
            0x1F, 0x8B, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0A,
            0x03, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        ];
        var archivePath = Path.Combine(_tempDir, "test.gz");
        await File.WriteAllBytesAsync(archivePath, emptyGz[..16]);

        var result = await _sut.VerifyAsync(archivePath, "gz");

        Assert.False(result);
    }



    private static async Task TruncateAsync(string path, int bytesToRemove)
    {
        var bytes = await File.ReadAllBytesAsync(path);
        await File.WriteAllBytesAsync(path, bytes[..^bytesToRemove]);
    }



    private static async Task CreateValidZipAsync(string path)
    {
        await using var fileStream = File.Create(path);
        using var archive = new ZipArchive(fileStream, ZipArchiveMode.Create, leaveOpen: true);
        var entry = archive.CreateEntry("test.txt", CompressionLevel.Fastest);
        var entryStream = await entry.OpenAsync();
        await using (entryStream)
        {
            await entryStream.WriteAsync("test content"u8.ToArray());
        }
    }



    private static async Task CreateValidGzAsync(string path)
    {
        await using var fileStream = File.Create(path);
        await using var gzStream = new GZipStream(fileStream, CompressionLevel.Fastest, leaveOpen: true);
        await gzStream.WriteAsync("test content"u8.ToArray());
    }



    private static async Task CreateValidBrotliAsync(string path)
    {
        await using var fileStream = File.Create(path);
        await using var brStream = new BrotliStream(fileStream, CompressionLevel.Fastest, leaveOpen: true);
        await brStream.WriteAsync("test content"u8.ToArray());
    }



    private static async Task CreateValidTarGzAsync(string path)
    {
        await using var fileStream = File.Create(path);
        await using var gzStream = new GZipStream(fileStream, CompressionLevel.Fastest, leaveOpen: true);
        await using var tarWriter = new TarWriter(gzStream, leaveOpen: true);

        var contentBytes = "test content"u8.ToArray();
        var entry = new PaxTarEntry(TarEntryType.RegularFile, "test.txt")
        {
            DataStream = new MemoryStream(contentBytes)
        };

        await tarWriter.WriteEntryAsync(entry);
    }



    private static async Task CreateValidTarBrAsync(string path)
    {
        await using var fileStream = File.Create(path);
        await using var brStream = new BrotliStream(fileStream, CompressionLevel.Fastest, leaveOpen: true);
        await using var tarWriter = new TarWriter(brStream, leaveOpen: true);

        var contentBytes = "test content"u8.ToArray();
        var entry = new PaxTarEntry(TarEntryType.RegularFile, "test.txt")
        {
            DataStream = new MemoryStream(contentBytes)
        };

        await tarWriter.WriteEntryAsync(entry);
    }

    [Theory]
    [InlineData("zst")]
    [InlineData("lz4")]
    public async Task VerifyAsync_when_validZstdOrLz4_expected_true(string format)
    {
        var archivePath = Path.Combine(_tempDir, "test." + format);
        await using (var fileStream = File.Create(archivePath))
        {
            Stream compressor = string.Equals(format, "zst", StringComparison.Ordinal)
                ? new ZstdSharp.CompressionStream(fileStream, leaveOpen: true)
                : K4os.Compression.LZ4.Streams.LZ4Stream.Encode(fileStream, leaveOpen: true);
            await using (compressor)
            {
                await compressor.WriteAsync("test content"u8.ToArray());
            }
        }

        var result = await _sut.VerifyAsync(archivePath, format, "test content"u8.Length);

        Assert.True(result);
    }



    [Theory]
    [InlineData("zst")]
    [InlineData("lz4")]
    public async Task VerifyAsync_when_truncatedZstdOrLz4_with_expectedSize_expected_false(string format)
    {
        // Neither format guarantees a content checksum, so — like brotli —
        // the expected-size comparison is the completeness gate (#3/#4
        // revival on top of the hardened verifier).
        var archivePath = Path.Combine(_tempDir, "test." + format);
        await using (var fileStream = File.Create(archivePath))
        {
            Stream compressor = string.Equals(format, "zst", StringComparison.Ordinal)
                ? new ZstdSharp.CompressionStream(fileStream, leaveOpen: true)
                : K4os.Compression.LZ4.Streams.LZ4Stream.Encode(fileStream, leaveOpen: true);
            await using (compressor)
            {
                await compressor.WriteAsync("test content"u8.ToArray());
            }
        }

        await TruncateAsync(archivePath, bytesToRemove: 4);

        var result = await _sut.VerifyAsync(archivePath, format, "test content"u8.Length);

        Assert.False(result);
    }

}
