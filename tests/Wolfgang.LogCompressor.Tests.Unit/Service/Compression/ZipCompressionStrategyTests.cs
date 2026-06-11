using System.IO.Compression;
using Wolfgang.LogCompressor.Service.Compression;

namespace Wolfgang.LogCompressor.Tests.Unit.Service.Compression;

public sealed class ZipCompressionStrategyTests
{
    private readonly ZipCompressionStrategy _sut = new();



    [Fact]
    public void FileExtension_when_accessed_expected_zip()
    {
        Assert.Equal("zip", _sut.FileExtension);
    }



    [Fact]
    public void BundleFileExtension_when_accessed_expected_zip()
    {
        Assert.Equal("zip", _sut.BundleFileExtension);
    }



    [Fact]
    public async Task CompressFileAsync_when_validStream_expected_validZipOutput()
    {
        var content = "Hello, this is a test log file content."u8.ToArray();
        using var inputStream = new MemoryStream(content);
        using var outputStream = new MemoryStream();

        await _sut.CompressFileAsync(inputStream, outputStream, "test.log");

        outputStream.Position = 0;
        using var archive = new ZipArchive(outputStream, ZipArchiveMode.Read);
        Assert.Single(archive.Entries);
        Assert.Equal("test.log", archive.Entries[0].Name);

        await using var entryStream = await archive.Entries[0].OpenAsync();
        using var reader = new StreamReader(entryStream);
        var decompressed = await reader.ReadToEndAsync();
        Assert.Equal("Hello, this is a test log file content.", decompressed);
    }



    [Fact]
    public async Task CompressFilesAsync_when_multipleStreams_expected_allEntriesInZip()
    {
        var inputs = new List<(Stream Stream, string EntryName)>
        {
            (new MemoryStream("File 1 content"u8.ToArray()), "file1.log"),
            (new MemoryStream("File 2 content"u8.ToArray()), "file2.log"),
            (new MemoryStream("File 3 content"u8.ToArray()), "file3.log")
        };

        using var outputStream = new MemoryStream();

        await _sut.CompressFilesAsync(inputs, outputStream);

        outputStream.Position = 0;
        using var archive = new ZipArchive(outputStream, ZipArchiveMode.Read);
        Assert.Equal(3, archive.Entries.Count);
        Assert.Equal("file1.log", archive.Entries[0].Name);
        Assert.Equal("file2.log", archive.Entries[1].Name);
        Assert.Equal("file3.log", archive.Entries[2].Name);

        foreach (var stream in inputs)
        {
            stream.Stream.Dispose();
        }
    }



    [Fact]
    public async Task CompressFileAsync_when_emptyStream_expected_validZipWithEmptyEntry()
    {
        using var inputStream = new MemoryStream([]);
        using var outputStream = new MemoryStream();

        await _sut.CompressFileAsync(inputStream, outputStream, "empty.log");

        outputStream.Position = 0;
        using var archive = new ZipArchive(outputStream, ZipArchiveMode.Read);
        Assert.Single(archive.Entries);
        Assert.Equal(0, archive.Entries[0].Length);
    }
}
