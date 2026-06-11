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
}
