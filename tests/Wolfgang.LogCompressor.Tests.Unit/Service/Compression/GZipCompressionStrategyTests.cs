using System.Formats.Tar;
using System.IO.Compression;
using Wolfgang.LogCompressor.Service.Compression;

namespace Wolfgang.LogCompressor.Tests.Unit.Service.Compression;

public sealed class GZipCompressionStrategyTests
{
    private readonly GZipCompressionStrategy _sut = new();



    [Fact]
    public void FileExtension_when_accessed_expected_gz()
    {
        Assert.Equal("gz", _sut.FileExtension);
    }



    [Fact]
    public void BundleFileExtension_when_accessed_expected_tarGz()
    {
        Assert.Equal("tar.gz", _sut.BundleFileExtension);
    }



    [Fact]
    public async Task CompressFileAsync_when_validStream_expected_validGzOutput()
    {
        var content = "GZip test content for log compression"u8.ToArray();
        using var inputStream = new MemoryStream(content);
        using var outputStream = new MemoryStream();

        await _sut.CompressFileAsync(inputStream, outputStream, "test.log");

        outputStream.Position = 0;
        await using var gzipStream = new GZipStream(outputStream, CompressionMode.Decompress, leaveOpen: true);
        using var reader = new StreamReader(gzipStream);
        var decompressed = await reader.ReadToEndAsync();
        Assert.Equal("GZip test content for log compression", decompressed);
    }



    [Fact]
    public async Task CompressFilesAsync_when_multipleStreams_expected_tarGzOutput()
    {
        var inputs = new List<(Stream Stream, string EntryName)>
        {
            (new MemoryStream("File A"u8.ToArray()), "a.log"),
            (new MemoryStream("File B"u8.ToArray()), "b.log")
        };

        using var outputStream = new MemoryStream();

        await _sut.CompressFilesAsync(inputs, outputStream);

        outputStream.Position = 0;
        await using var gzipStream = new GZipStream(outputStream, CompressionMode.Decompress, leaveOpen: true);
        await using var tarReader = new TarReader(gzipStream);

        var entries = new List<string>();
        while (await tarReader.GetNextEntryAsync() is { } entry)
        {
            entries.Add(entry.Name);
        }

        Assert.Equal(2, entries.Count);
        Assert.Contains("a.log", entries);
        Assert.Contains("b.log", entries);

        foreach (var input in inputs)
        {
            input.Stream.Dispose();
        }
    }



    [Fact]
    public async Task CompressFileAsync_when_emptyStream_expected_validGzOutput()
    {
        using var inputStream = new MemoryStream([]);
        using var outputStream = new MemoryStream();

        await _sut.CompressFileAsync(inputStream, outputStream, "empty.log");

        outputStream.Position = 0;
        await using var gzipStream = new GZipStream(outputStream, CompressionMode.Decompress, leaveOpen: true);
        using var reader = new StreamReader(gzipStream);
        var decompressed = await reader.ReadToEndAsync();
        Assert.Equal(string.Empty, decompressed);
    }
}
