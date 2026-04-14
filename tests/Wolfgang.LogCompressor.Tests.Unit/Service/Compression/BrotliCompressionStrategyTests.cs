using System.Formats.Tar;
using System.IO.Compression;
using Wolfgang.LogCompressor.Service.Compression;

namespace Wolfgang.LogCompressor.Tests.Unit.Service.Compression;

public sealed class BrotliCompressionStrategyTests
{
    private readonly BrotliCompressionStrategy _sut = new();



    [Fact]
    public void FileExtension_when_accessed_expected_br()
    {
        Assert.Equal("br", _sut.FileExtension);
    }



    [Fact]
    public void BundleFileExtension_when_accessed_expected_tarBr()
    {
        Assert.Equal("tar.br", _sut.BundleFileExtension);
    }



    [Fact]
    public async Task CompressFileAsync_when_validStream_expected_validBrotliOutput()
    {
        var content = "Brotli test content for log compression"u8.ToArray();
        using var inputStream = new MemoryStream(content);
        using var outputStream = new MemoryStream();

        await _sut.CompressFileAsync(inputStream, outputStream, "test.log");

        outputStream.Position = 0;
        await using var brotliStream = new BrotliStream(outputStream, CompressionMode.Decompress, leaveOpen: true);
        using var reader = new StreamReader(brotliStream);
        var decompressed = await reader.ReadToEndAsync();
        Assert.Equal("Brotli test content for log compression", decompressed);
    }



    [Fact]
    public async Task CompressFilesAsync_when_multipleStreams_expected_tarBrOutput()
    {
        var inputs = new List<(Stream Stream, string EntryName)>
        {
            (new MemoryStream("File X"u8.ToArray()), "x.log"),
            (new MemoryStream("File Y"u8.ToArray()), "y.log")
        };

        using var outputStream = new MemoryStream();

        await _sut.CompressFilesAsync(inputs, outputStream);

        outputStream.Position = 0;
        await using var brotliStream = new BrotliStream(outputStream, CompressionMode.Decompress, leaveOpen: true);
        await using var tarReader = new TarReader(brotliStream);

        var entries = new List<string>();
        while (await tarReader.GetNextEntryAsync() is { } entry)
        {
            entries.Add(entry.Name);
        }

        Assert.Equal(2, entries.Count);
        Assert.Contains("x.log", entries);
        Assert.Contains("y.log", entries);

        foreach (var input in inputs)
        {
            input.Stream.Dispose();
        }
    }



    [Fact]
    public async Task CompressFileAsync_when_emptyStream_expected_validBrotliOutput()
    {
        using var inputStream = new MemoryStream([]);
        using var outputStream = new MemoryStream();

        await _sut.CompressFileAsync(inputStream, outputStream, "empty.log");

        outputStream.Position = 0;
        await using var brotliStream = new BrotliStream(outputStream, CompressionMode.Decompress, leaveOpen: true);
        using var reader = new StreamReader(brotliStream);
        var decompressed = await reader.ReadToEndAsync();
        Assert.Equal(string.Empty, decompressed);
    }
}
