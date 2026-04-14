using Microsoft.Extensions.Logging;
using NSubstitute;
using Wolfgang.LogCompressor.Abstraction;
using Wolfgang.LogCompressor.Model;
using Wolfgang.LogCompressor.Service;
using Wolfgang.LogCompressor.Service.Compression;

namespace Wolfgang.LogCompressor.Tests.Unit.Service;

public sealed class BundleServiceTests
{
    private readonly IFileSystem _fileSystem = Substitute.For<IFileSystem>();
    private readonly IFileFilter _fileFilter = Substitute.For<IFileFilter>();
    private readonly IFileNamer _fileNamer = Substitute.For<IFileNamer>();
    private readonly ICompressionStrategy _strategy = Substitute.For<ICompressionStrategy>();
    private readonly CompressionStrategyFactory _strategyFactory;
    private readonly BundleService _sut;



    public BundleServiceTests()
    {
        _strategyFactory = Substitute.For<CompressionStrategyFactory>();
        _strategyFactory.Create(Arg.Any<CompressionFormat>()).Returns(_strategy);
        _strategy.BundleFileExtension.Returns("zip");

        _sut = new BundleService
        (
            _fileSystem,
            _fileFilter,
            _fileNamer,
            _strategyFactory,
            Substitute.For<ILogger<BundleService>>()
        );
    }



    [Fact]
    public async Task ExecuteAsync_when_multipleFiles_expected_singleArchive()
    {
        var dir = "/tmp/logs/MyApp";
        var files = CreateTempFiles(3);
        var fileInfos = files.Select(f => new FileInfo(f)).ToList();

        SetupDirectory(dir, files, fileInfos);
        _fileNamer.GetBundleFileName("MyApp", Arg.Any<IReadOnlyList<FileInfo>>(), "zip").Returns("bundle.zip");
        _fileSystem.CreateWrite(Arg.Any<string>()).Returns(new MemoryStream());

        var options = new CompressionOptions { SourcePath = dir };
        var result = await _sut.ExecuteAsync(options);

        Assert.True(result.Success);
        foreach (var file in files)
        {
            _fileSystem.Received(1).DeleteFile(file);
        }
    }



    [Fact]
    public async Task ExecuteAsync_when_filterApplied_expected_onlyMatchingFilesBundled()
    {
        var dir = "/tmp/logs/MyApp";
        var allFiles = CreateTempFiles(3);
        var allInfos = allFiles.Select(f => new FileInfo(f)).ToList();
        var filteredInfos = new List<FileInfo> { allInfos[0] };

        _fileSystem.FileExists(dir).Returns(false);
        _fileSystem.DirectoryExists(dir).Returns(true);
        _fileSystem.EnumerateFiles(dir, "*", SearchOption.TopDirectoryOnly).Returns(allFiles);
        foreach (var file in allFiles)
        {
            _fileSystem.GetFileInfo(file).Returns(allInfos[allFiles.ToList().IndexOf(file)]);
        }

        _fileFilter.Apply(Arg.Any<IEnumerable<FileInfo>>(), 7, null, null).Returns(filteredInfos);
        _fileNamer.GetBundleFileName("MyApp", Arg.Any<IReadOnlyList<FileInfo>>(), "zip").Returns("bundle.zip");
        _fileSystem.OpenRead(Arg.Any<string>()).Returns(new MemoryStream("content"u8.ToArray()));
        _fileSystem.CreateWrite(Arg.Any<string>()).Returns(new MemoryStream());

        var options = new CompressionOptions { SourcePath = dir, OlderThanDays = 7 };
        var result = await _sut.ExecuteAsync(options);

        Assert.True(result.Success);
        _fileSystem.Received(1).DeleteFile(allFiles[0]);
        _fileSystem.DidNotReceive().DeleteFile(allFiles[1]);
        _fileSystem.DidNotReceive().DeleteFile(allFiles[2]);
    }



    [Fact]
    public async Task ExecuteAsync_when_outputPathSpecified_expected_archiveInOutputDir()
    {
        var dir = "/tmp/logs/MyApp";
        var outputDir = "/tmp/archive";
        var files = CreateTempFiles(1);
        var fileInfos = files.Select(f => new FileInfo(f)).ToList();

        SetupDirectory(dir, files, fileInfos);
        _fileSystem.DirectoryExists(outputDir).Returns(false);
        _fileNamer.GetBundleFileName("MyApp", Arg.Any<IReadOnlyList<FileInfo>>(), "zip").Returns("bundle.zip");
        _fileSystem.CreateWrite(Arg.Any<string>()).Returns(new MemoryStream());

        var options = new CompressionOptions { SourcePath = dir, OutputPath = outputDir };
        var result = await _sut.ExecuteAsync(options);

        Assert.True(result.Success);
        _fileSystem.Received(1).CreateDirectory(outputDir);
        Assert.Contains(outputDir, result.OutputPath);
    }



    [Fact]
    public async Task ExecuteAsync_when_noFilesMatch_expected_failureResult()
    {
        var dir = "/tmp/logs/MyApp";

        _fileSystem.FileExists(dir).Returns(false);
        _fileSystem.DirectoryExists(dir).Returns(true);
        _fileSystem.EnumerateFiles(dir, "*", SearchOption.TopDirectoryOnly).Returns([]);
        _fileFilter.Apply(Arg.Any<IEnumerable<FileInfo>>(), null, null, null).Returns([]);

        var options = new CompressionOptions { SourcePath = dir };
        var result = await _sut.ExecuteAsync(options);

        Assert.False(result.Success);
        Assert.Equal("No files matched the specified criteria.", result.ErrorMessage);
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



    [Fact]
    public async Task ExecuteAsync_when_compressionFails_expected_failureResult()
    {
        var dir = "/tmp/logs/MyApp";
        var files = CreateTempFiles(1);
        var fileInfos = files.Select(f => new FileInfo(f)).ToList();

        SetupDirectory(dir, files, fileInfos);
        _fileNamer.GetBundleFileName("MyApp", Arg.Any<IReadOnlyList<FileInfo>>(), "zip").Returns("bundle.zip");
        _fileSystem.CreateWrite(Arg.Any<string>()).Returns(_ => throw new IOException("disk full"));

        var options = new CompressionOptions { SourcePath = dir };
        var result = await _sut.ExecuteAsync(options);

        Assert.False(result.Success);
        Assert.Equal("disk full", result.ErrorMessage);
    }



    [Fact]
    public async Task ExecuteAsync_when_singleFile_expected_bundleContainsSingleFile()
    {
        var file = CreateTempFiles(1)[0];
        var fileInfo = new FileInfo(file);

        _fileSystem.FileExists(file).Returns(true);
        _fileSystem.GetFileInfo(file).Returns(fileInfo);
        _fileFilter.Apply(Arg.Any<IEnumerable<FileInfo>>(), null, null, null).Returns(new List<FileInfo> { fileInfo });
        _fileNamer.GetBundleFileName(Arg.Any<string>(), Arg.Any<IReadOnlyList<FileInfo>>(), "zip").Returns("bundle.zip");
        _fileSystem.OpenRead(file).Returns(new MemoryStream("content"u8.ToArray()));
        _fileSystem.CreateWrite(Arg.Any<string>()).Returns(new MemoryStream());

        var options = new CompressionOptions { SourcePath = file };
        var result = await _sut.ExecuteAsync(options);

        Assert.True(result.Success);
        _fileSystem.Received(1).DeleteFile(file);
    }



    [Fact]
    public async Task ExecuteAsync_when_recurse_expected_allDirectoriesSearched()
    {
        var dir = "/tmp/logs/MyApp";
        var files = CreateTempFiles(1);
        var fileInfos = files.Select(f => new FileInfo(f)).ToList();

        _fileSystem.FileExists(dir).Returns(false);
        _fileSystem.DirectoryExists(dir).Returns(true);
        _fileSystem.EnumerateFiles(dir, "*", SearchOption.AllDirectories).Returns(files);
        _fileSystem.GetFileInfo(files[0]).Returns(fileInfos[0]);
        _fileSystem.OpenRead(files[0]).Returns(new MemoryStream("content"u8.ToArray()));
        _fileFilter.Apply(Arg.Any<IEnumerable<FileInfo>>(), null, null, null).Returns(fileInfos);
        _fileNamer.GetBundleFileName("MyApp", Arg.Any<IReadOnlyList<FileInfo>>(), "zip").Returns("bundle.zip");
        _fileSystem.CreateWrite(Arg.Any<string>()).Returns(new MemoryStream());

        var options = new CompressionOptions { SourcePath = dir, Recurse = true };
        var result = await _sut.ExecuteAsync(options);

        _fileSystem.Received(1).EnumerateFiles(dir, "*", SearchOption.AllDirectories);
        Assert.True(result.Success);
    }



    private void SetupDirectory(string dir, string[] files, List<FileInfo> fileInfos)
    {
        _fileSystem.FileExists(dir).Returns(false);
        _fileSystem.DirectoryExists(dir).Returns(true);
        _fileSystem.EnumerateFiles(dir, "*", SearchOption.TopDirectoryOnly).Returns(files);

        for (var i = 0; i < files.Length; i++)
        {
            _fileSystem.GetFileInfo(files[i]).Returns(fileInfos[i]);
            _fileSystem.OpenRead(files[i]).Returns(new MemoryStream("content"u8.ToArray()));
        }

        _fileFilter.Apply(Arg.Any<IEnumerable<FileInfo>>(), null, null, null).Returns(fileInfos);
    }



    private static string[] CreateTempFiles(int count)
    {
        var files = new string[count];

        for (var i = 0; i < count; i++)
        {
            var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".log");
            File.WriteAllText(path, $"content {i}");
            files[i] = path;
        }

        return files;
    }
}
