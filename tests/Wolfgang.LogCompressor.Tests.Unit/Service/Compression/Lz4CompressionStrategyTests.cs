using System.Formats.Tar;
using System.IO.Compression;
using K4os.Compression.LZ4.Streams;
using Wolfgang.LogCompressor.Service.Compression;

namespace Wolfgang.LogCompressor.Tests.Unit.Service.Compression;

public sealed class Lz4CompressionStrategyTests
{
    private readonly Lz4CompressionStrategy _sut = new();



    [Fact]
    public void FileExtension_when_accessed_expected_lz4()
    {
        Assert.Equal("lz4", _sut.FileExtension);
    }



    [Fact]
    public void BundleFileExtension_when_accessed_expected_tarLz4()
    {
        Assert.Equal("tar.lz4", _sut.BundleFileExtension);
    }



    [Fact]
    public async Task CompressFileAsync_when_validStream_expected_validLz4Output()
    {
        var content = "LZ4 test content for log compression"u8.ToArray();
        using var inputStream = new MemoryStream(content);
        using var outputStream = new MemoryStream();

        await _sut.CompressFileAsync(inputStream, outputStream, "test.log");

        outputStream.Position = 0;
        await using var lz4Stream = LZ4Stream.Decode(outputStream);
        using var reader = new StreamReader(lz4Stream);
        var decompressed = await reader.ReadToEndAsync();
        Assert.Equal("LZ4 test content for log compression", decompressed);
    }



    [Fact]
    public async Task CompressFilesAsync_when_multipleStreams_expected_tarLz4Output()
    {
        var inputs = new List<(Stream Stream, string EntryName)>
        {
            (new MemoryStream("File A"u8.ToArray()), "a.log"),
            (new MemoryStream("File B"u8.ToArray()), "b.log")
        };

        using var outputStream = new MemoryStream();

        await _sut.CompressFilesAsync(inputs.ToAsyncEnumerable(), outputStream);

        outputStream.Position = 0;
        await using var lz4Stream = LZ4Stream.Decode(outputStream);
        await using var tarReader = new TarReader(lz4Stream);

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
    public async Task CompressFileAsync_when_emptyStream_expected_validLz4Output()
    {
        using var inputStream = new MemoryStream([]);
        using var outputStream = new MemoryStream();

        await _sut.CompressFileAsync(inputStream, outputStream, "empty.log");

        outputStream.Position = 0;
        await using var lz4Stream = LZ4Stream.Decode(outputStream);
        using var reader = new StreamReader(lz4Stream);
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
        var sut = new Lz4CompressionStrategy(level);
        using var inputStream = new MemoryStream("level mapping round-trip"u8.ToArray());
        using var outputStream = new MemoryStream();

        await sut.CompressFileAsync(inputStream, outputStream, "x.log");

        outputStream.Position = 0;
        await using var lz4Stream = LZ4Stream.Decode(outputStream);
        using var reader = new StreamReader(lz4Stream);
        Assert.Equal("level mapping round-trip", await reader.ReadToEndAsync());
    }
}
