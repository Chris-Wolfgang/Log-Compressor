using System.Formats.Tar;
using System.IO.Compression;
using Wolfgang.LogCompressor.Service.Compression;
using ZstdSharp;

namespace Wolfgang.LogCompressor.Tests.Unit.Service.Compression;

public sealed class ZstdCompressionStrategyTests
{
    private readonly ZstdCompressionStrategy _sut = new();



    [Fact]
    public void FileExtension_when_accessed_expected_zst()
    {
        Assert.Equal("zst", _sut.FileExtension);
    }



    [Fact]
    public void BundleFileExtension_when_accessed_expected_tarZst()
    {
        Assert.Equal("tar.zst", _sut.BundleFileExtension);
    }



    [Fact]
    public async Task CompressFileAsync_when_validStream_expected_validZstOutput()
    {
        var content = "Zstd test content for log compression"u8.ToArray();
        using var inputStream = new MemoryStream(content);
        using var outputStream = new MemoryStream();

        await _sut.CompressFileAsync(inputStream, outputStream, "test.log");

        outputStream.Position = 0;
        await using var zstdStream = new DecompressionStream(outputStream);
        using var reader = new StreamReader(zstdStream);
        var decompressed = await reader.ReadToEndAsync();
        Assert.Equal("Zstd test content for log compression", decompressed);
    }



    [Fact]
    public async Task CompressFilesAsync_when_multipleStreams_expected_tarZstOutput()
    {
        var inputs = new List<(Stream Stream, string EntryName)>
        {
            (new MemoryStream("File A"u8.ToArray()), "a.log"),
            (new MemoryStream("File B"u8.ToArray()), "b.log")
        };

        using var outputStream = new MemoryStream();

        await _sut.CompressFilesAsync(inputs, outputStream);

        outputStream.Position = 0;
        await using var zstdStream = new DecompressionStream(outputStream);
        await using var tarReader = new TarReader(zstdStream);

        var entries = new List<string>();
        while (await tarReader.GetNextEntryAsync() is { } entry)
        {
            entries.Add(entry.Name);
        }

        Assert.Equal(2, entries.Count);
        Assert.Contains("a.log", entries);
        Assert.Contains("b.log", entries);
    }



    [Fact]
    public async Task CompressFileAsync_when_emptyStream_expected_validZstOutput()
    {
        using var inputStream = new MemoryStream([]);
        using var outputStream = new MemoryStream();

        await _sut.CompressFileAsync(inputStream, outputStream, "empty.log");

        outputStream.Position = 0;
        await using var zstdStream = new DecompressionStream(outputStream);
        using var reader = new StreamReader(zstdStream);
        var decompressed = await reader.ReadToEndAsync();
        Assert.Equal(string.Empty, decompressed);
    }



    [Theory]
    [InlineData(CompressionLevel.Fastest)]
    [InlineData(CompressionLevel.Optimal)]
    [InlineData(CompressionLevel.SmallestSize)]
    [InlineData(CompressionLevel.NoCompression)]
    public async Task CompressFileAsync_when_eachLevel_expected_roundTrips(CompressionLevel level)
    {
        var sut = new ZstdCompressionStrategy(level);
        using var inputStream = new MemoryStream("level mapping round-trip"u8.ToArray());
        using var outputStream = new MemoryStream();

        await sut.CompressFileAsync(inputStream, outputStream, "x.log");

        outputStream.Position = 0;
        await using var zstdStream = new DecompressionStream(outputStream);
        using var reader = new StreamReader(zstdStream);
        Assert.Equal("level mapping round-trip", await reader.ReadToEndAsync());
    }
}
