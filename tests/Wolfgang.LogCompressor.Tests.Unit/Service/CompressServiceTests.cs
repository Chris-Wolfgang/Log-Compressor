using Microsoft.Extensions.Logging;
using NSubstitute;
using Wolfgang.LogCompressor.Abstraction;
using Wolfgang.LogCompressor.Model;
using Wolfgang.LogCompressor.Service;
using Wolfgang.LogCompressor.Service.Compression;

namespace Wolfgang.LogCompressor.Tests.Unit.Service;

public sealed class CompressServiceTests
{
    private readonly IFileSystem _fileSystem = Substitute.For<IFileSystem>();
    private readonly IFileFilter _fileFilter = Substitute.For<IFileFilter>();
    private readonly IFileNamer _fileNamer = Substitute.For<IFileNamer>();
    private readonly ICompressionStrategy _strategy = Substitute.For<ICompressionStrategy>();
    private readonly CompressionStrategyFactory _strategyFactory;
    private readonly CompressService _sut;



    public CompressServiceTests()
    {
        _strategyFactory = Substitute.For<CompressionStrategyFactory>();
        _strategyFactory.Create(Arg.Any<CompressionFormat>()).Returns(_strategy);
        _strategy.FileExtension.Returns("zip");

        _sut = new CompressService
        (
            _fileSystem,
            _fileFilter,
            _fileNamer,
            _strategyFactory,
            Substitute.For<ILogger<CompressService>>()
        );
    }



    [Fact]
    public async Task ExecuteAsync_when_singleFile_expected_oneArchiveCreated()
    {
        var tempFile = CreateTempFile();
        var fileInfo = new FileInfo(tempFile);

        _fileSystem.FileExists(tempFile).Returns(true);
        _fileSystem.GetFileInfo(tempFile).Returns(fileInfo);
        _fileFilter.Apply(Arg.Any<IEnumerable<FileInfo>>(), null, null, null).Returns([fileInfo]);
        _fileNamer.GetCompressedFileName(fileInfo, "zip").Returns("test-2026-01-01_00-00-00.zip");
        _fileSystem.OpenRead(tempFile).Returns(new MemoryStream("content"u8.ToArray()));
        _fileSystem.CreateWrite(Arg.Any<string>()).Returns(new MemoryStream());

        var options = new CompressionOptions { SourcePath = tempFile };
        var results = await _sut.ExecuteAsync(options);

        Assert.Single(results);
        Assert.True(results[0].Success);
        _fileSystem.Received(1).DeleteFile(tempFile);
    }



    [Fact]
    public async Task ExecuteAsync_when_directory_expected_oneArchivePerFile()
    {
        var dir = Path.GetTempPath();
        var files = new[] { CreateTempFile(), CreateTempFile() };
        var fileInfos = files.Select(f => new FileInfo(f)).ToList();

        _fileSystem.FileExists(dir).Returns(false);
        _fileSystem.DirectoryExists(dir).Returns(true);
        _fileSystem.EnumerateFiles(dir, "*", SearchOption.TopDirectoryOnly).Returns(files);
        _fileSystem.GetFileInfo(files[0]).Returns(fileInfos[0]);
        _fileSystem.GetFileInfo(files[1]).Returns(fileInfos[1]);
        _fileFilter.Apply(Arg.Any<IEnumerable<FileInfo>>(), null, null, null).Returns(fileInfos);
        _fileNamer.GetCompressedFileName(Arg.Any<FileInfo>(), "zip").Returns("out.zip");
        _fileSystem.OpenRead(Arg.Any<string>()).Returns(_ => new MemoryStream("content"u8.ToArray()));
        _fileSystem.CreateWrite(Arg.Any<string>()).Returns(_ => new MemoryStream());

        var options = new CompressionOptions { SourcePath = dir };
        var results = await _sut.ExecuteAsync(options);

        Assert.Equal(2, results.Count);
        Assert.All(results, r => Assert.True(r.Success));
    }



    [Fact]
    public async Task ExecuteAsync_when_recurse_expected_subdirectoriesIncluded()
    {
        var dir = Path.GetTempPath();
        var file = CreateTempFile();
        var fileInfo = new FileInfo(file);

        _fileSystem.FileExists(dir).Returns(false);
        _fileSystem.DirectoryExists(dir).Returns(true);
        _fileSystem.EnumerateFiles(dir, "*", SearchOption.AllDirectories).Returns([file]);
        _fileSystem.GetFileInfo(file).Returns(fileInfo);
        _fileFilter.Apply(Arg.Any<IEnumerable<FileInfo>>(), null, null, null).Returns([fileInfo]);
        _fileNamer.GetCompressedFileName(Arg.Any<FileInfo>(), "zip").Returns("out.zip");
        _fileSystem.OpenRead(Arg.Any<string>()).Returns(new MemoryStream("content"u8.ToArray()));
        _fileSystem.CreateWrite(Arg.Any<string>()).Returns(new MemoryStream());

        var options = new CompressionOptions { SourcePath = dir, Recurse = true };
        var results = await _sut.ExecuteAsync(options);

        _fileSystem.Received(1).EnumerateFiles(dir, "*", SearchOption.AllDirectories);
        Assert.Single(results);
    }



    [Fact]
    public async Task ExecuteAsync_when_outputPathSpecified_expected_archivesInOutputDir()
    {
        var tempFile = CreateTempFile();
        var fileInfo = new FileInfo(tempFile);
        var outputDir = Path.Combine(Path.GetTempPath(), "output");

        _fileSystem.FileExists(tempFile).Returns(true);
        _fileSystem.GetFileInfo(tempFile).Returns(fileInfo);
        _fileFilter.Apply(Arg.Any<IEnumerable<FileInfo>>(), null, null, null).Returns([fileInfo]);
        _fileNamer.GetCompressedFileName(fileInfo, "zip").Returns("out.zip");
        _fileSystem.DirectoryExists(outputDir).Returns(false);
        _fileSystem.OpenRead(tempFile).Returns(new MemoryStream("content"u8.ToArray()));
        _fileSystem.CreateWrite(Arg.Any<string>()).Returns(new MemoryStream());

        var options = new CompressionOptions { SourcePath = tempFile, OutputPath = outputDir };
        var results = await _sut.ExecuteAsync(options);

        _fileSystem.Received(1).CreateDirectory(outputDir);
        Assert.Single(results);
        Assert.Contains(outputDir, results[0].OutputPath);
    }



    [Fact]
    public async Task ExecuteAsync_when_compressionFails_expected_originalNotDeleted()
    {
        var tempFile = CreateTempFile();
        var fileInfo = new FileInfo(tempFile);

        _fileSystem.FileExists(tempFile).Returns(true);
        _fileSystem.GetFileInfo(tempFile).Returns(fileInfo);
        _fileFilter.Apply(Arg.Any<IEnumerable<FileInfo>>(), null, null, null).Returns([fileInfo]);
        _fileNamer.GetCompressedFileName(fileInfo, "zip").Returns("out.zip");
        _fileSystem.OpenRead(tempFile).Returns(new MemoryStream("content"u8.ToArray()));
        _fileSystem.CreateWrite(Arg.Any<string>()).Returns(_ => throw new IOException("disk full"));

        var options = new CompressionOptions { SourcePath = tempFile };
        var results = await _sut.ExecuteAsync(options);

        Assert.Single(results);
        Assert.False(results[0].Success);
        Assert.Equal("disk full", results[0].ErrorMessage);
        _fileSystem.DidNotReceive().DeleteFile(tempFile);
    }



    [Fact]
    public async Task ExecuteAsync_when_noFilesMatch_expected_emptyResults()
    {
        var dir = Path.GetTempPath();

        _fileSystem.FileExists(dir).Returns(false);
        _fileSystem.DirectoryExists(dir).Returns(true);
        _fileSystem.EnumerateFiles(dir, "*", SearchOption.TopDirectoryOnly).Returns([]);
        _fileFilter.Apply(Arg.Any<IEnumerable<FileInfo>>(), null, null, null).Returns([]);

        var options = new CompressionOptions { SourcePath = dir };
        var results = await _sut.ExecuteAsync(options);

        Assert.Empty(results);
    }



    [Fact]
    public async Task ExecuteAsync_when_sourceNotFound_expected_throwsFileNotFoundException()
    {
        _fileSystem.FileExists("nonexistent").Returns(false);
        _fileSystem.DirectoryExists("nonexistent").Returns(false);

        var options = new CompressionOptions { SourcePath = "nonexistent" };

        await Assert.ThrowsAsync<FileNotFoundException>(() => _sut.ExecuteAsync(options));
    }



    [Fact]
    public async Task ExecuteAsync_when_nullOptions_expected_throwsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.ExecuteAsync(null!));
    }



    private static string CreateTempFile()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".log");
        File.WriteAllText(path, "test content");
        return path;
    }
}
