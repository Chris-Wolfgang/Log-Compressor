using Microsoft.Extensions.Logging;
using NSubstitute;
using Wolfgang.LogCompressor.Abstraction;
using Wolfgang.LogCompressor.Model;
using Wolfgang.LogCompressor.Service;
using Wolfgang.LogCompressor.Service.Compression;

namespace Wolfgang.LogCompressor.Tests.Unit.Service;

public sealed class BundleServiceTests : IDisposable
{
    private readonly TempDirectory _tempDir = new();
    private readonly IFileSystem _fileSystem = Substitute.For<IFileSystem>();
    private readonly IFileFilter _fileFilter = Substitute.For<IFileFilter>();
    private readonly IFileNamer _fileNamer = Substitute.For<IFileNamer>();
    private readonly IArchiveVerifier _archiveVerifier = Substitute.For<IArchiveVerifier>();
    private readonly ICompressionStrategy _strategy = Substitute.For<ICompressionStrategy>();
    private readonly BundleService _sut;



    public BundleServiceTests()
    {
        var strategyFactory = Substitute.For<CompressionStrategyFactory>();
        strategyFactory.Create(Arg.Any<CompressionFormat>(), Arg.Any<System.IO.Compression.CompressionLevel>()).Returns(_strategy);
        _strategy.BundleFileExtension.Returns("zip");
        _archiveVerifier.VerifyAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<long?>()).Returns(Task.FromResult(true));

        // The real strategies enumerate (and dispose) the lazily-opened input
        // sequence; the substitute must enumerate it too so BundleService records
        // which files were actually bundled.
        _strategy
            .CompressFilesAsync(Arg.Any<IEnumerable<(Stream Stream, string EntryName)>>(), Arg.Any<Stream>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                foreach (var _ in callInfo.Arg<IEnumerable<(Stream Stream, string EntryName)>>())
                {
                }

                return Task.CompletedTask;
            });

        _sut = new BundleService
        (
            _fileSystem,
            _fileFilter,
            _fileNamer,
            _archiveVerifier,
            strategyFactory,
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

        _fileSystem.FileExists(dir).Returns(returnThis: false);
        _fileSystem.DirectoryExists(dir).Returns(returnThis: true);
        _fileSystem.EnumerateFiles(dir, "*", SearchOption.TopDirectoryOnly).Returns(allFiles);
        foreach (var file in allFiles)
        {
            _fileSystem.GetFileInfo(file).Returns(allInfos[allFiles.ToList().IndexOf(file)]);
        }

        _fileFilter.Apply
        (
            Arg.Any<IEnumerable<FileInfo>>(),
            Arg.Is<int?>(7),
            Arg.Any<DateTime?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<IReadOnlyList<string>>(),
            Arg.Any<IReadOnlyList<string>>()
        ).Returns(filteredInfos);
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
        _fileSystem.DirectoryExists(outputDir).Returns(returnThis: false);
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

        _fileSystem.FileExists(dir).Returns(returnThis: false);
        _fileSystem.DirectoryExists(dir).Returns(returnThis: true);
        _fileSystem.EnumerateFiles(dir, "*", SearchOption.TopDirectoryOnly).Returns([]);
        _fileFilter.Apply
        (
            Arg.Any<IEnumerable<FileInfo>>(),
            Arg.Any<int?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<IReadOnlyList<string>>(),
            Arg.Any<IReadOnlyList<string>>()
        ).Returns([]);

        var options = new CompressionOptions { SourcePath = dir };
        var result = await _sut.ExecuteAsync(options);

        Assert.False(result.Success);
        Assert.Equal("No files matched the specified criteria.", result.ErrorMessage);
    }



    [Fact]
    public async Task ExecuteAsync_when_sourceNotFound_expected_throwsFileNotFoundException()
    {
        _fileSystem.FileExists("nonexistent").Returns(returnThis: false);
        _fileSystem.DirectoryExists("nonexistent").Returns(returnThis: false);

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

        _fileSystem.FileExists(file).Returns(returnThis: true);
        _fileSystem.GetFileInfo(file).Returns(fileInfo);
        _fileFilter.Apply
        (
            Arg.Any<IEnumerable<FileInfo>>(),
            Arg.Any<int?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<IReadOnlyList<string>>(),
            Arg.Any<IReadOnlyList<string>>()
        ).Returns(new List<FileInfo> { fileInfo });
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

        _fileSystem.FileExists(dir).Returns(returnThis: false);
        _fileSystem.DirectoryExists(dir).Returns(returnThis: true);
        _fileSystem.EnumerateFiles(dir, "*", SearchOption.AllDirectories).Returns(files);
        _fileSystem.GetFileInfo(files[0]).Returns(fileInfos[0]);
        _fileSystem.OpenRead(files[0]).Returns(new MemoryStream("content"u8.ToArray()));
        _fileFilter.Apply
        (
            Arg.Any<IEnumerable<FileInfo>>(),
            Arg.Any<int?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<IReadOnlyList<string>>(),
            Arg.Any<IReadOnlyList<string>>()
        ).Returns(fileInfos);
        _fileNamer.GetBundleFileName("MyApp", Arg.Any<IReadOnlyList<FileInfo>>(), "zip").Returns("bundle.zip");
        _fileSystem.CreateWrite(Arg.Any<string>()).Returns(new MemoryStream());

        var options = new CompressionOptions { SourcePath = dir, Recurse = true };
        var result = await _sut.ExecuteAsync(options);

        _fileSystem.Received(1).EnumerateFiles(dir, "*", SearchOption.AllDirectories);
        Assert.True(result.Success);
    }



    [Fact]
    public async Task ExecuteAsync_when_verificationFails_expected_originalsPreservedAndFailure()
    {
        var dir = "/tmp/logs/MyApp";
        var files = CreateTempFiles(2);
        var fileInfos = files.Select(f => new FileInfo(f)).ToList();

        SetupDirectory(dir, files, fileInfos);
        _fileNamer.GetBundleFileName("MyApp", Arg.Any<IReadOnlyList<FileInfo>>(), "zip").Returns("bundle.zip");
        _fileSystem.CreateWrite(Arg.Any<string>()).Returns(new MemoryStream());
        _archiveVerifier.VerifyAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<long?>()).Returns(Task.FromResult(false));

        var options = new CompressionOptions { SourcePath = dir, Verify = true };
        var result = await _sut.ExecuteAsync(options);

        Assert.False(result.Success);
        Assert.Equal("Archive verification failed.", result.ErrorMessage);
        foreach (var file in files)
        {
            _fileSystem.DidNotReceive().DeleteFile(file);
        }
    }



    [Fact]
    public async Task ExecuteAsync_when_someFilesUnreadable_expected_skippedNotDeletedAndPartialFailure()
    {
        var dir = "/tmp/logs/MyApp";
        var files = CreateTempFiles(2);
        var fileInfos = files.Select(f => new FileInfo(f)).ToList();

        SetupDirectory(dir, files, fileInfos);
        _fileSystem.OpenRead(files[0]).Returns(_ => throw new IOException("locked"));
        _fileNamer.GetBundleFileName("MyApp", Arg.Any<IReadOnlyList<FileInfo>>(), "zip").Returns("bundle.zip");
        _fileSystem.CreateWrite(Arg.Any<string>()).Returns(new MemoryStream());

        var result = await _sut.ExecuteAsync(new CompressionOptions { SourcePath = dir });

        Assert.False(result.Success);
        Assert.Contains("skipped", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
        _fileSystem.DidNotReceive().DeleteFile(files[0]);   // unreadable file preserved
        _fileSystem.Received(1).DeleteFile(files[1]);        // readable file bundled + deleted
    }



    [Fact]
    public async Task ExecuteAsync_when_allFilesUnreadable_expected_failureAndEmptyArchiveRemoved()
    {
        var dir = "/tmp/logs/MyApp";
        var files = CreateTempFiles(2);
        var fileInfos = files.Select(f => new FileInfo(f)).ToList();

        SetupDirectory(dir, files, fileInfos);
        foreach (var file in files)
        {
            _fileSystem.OpenRead(file).Returns(_ => throw new IOException("locked"));
        }
        _fileNamer.GetBundleFileName("MyApp", Arg.Any<IReadOnlyList<FileInfo>>(), "zip").Returns("bundle.zip");
        _fileSystem.CreateWrite(Arg.Any<string>()).Returns(new MemoryStream());

        var result = await _sut.ExecuteAsync(new CompressionOptions { SourcePath = dir });

        Assert.False(result.Success);
        Assert.Contains("No readable files", result.ErrorMessage);
        _fileSystem.Received(1).DeleteFile(Arg.Is<string>(p => p != null && p.Contains("bundle.zip", StringComparison.Ordinal)));
        foreach (var file in files)
        {
            _fileSystem.DidNotReceive().DeleteFile(file);   // no source deleted
        }
    }



    [Fact]
    public async Task ExecuteAsync_when_outputArchiveExists_expected_refuseAndNoDelete()
    {
        var dir = "/tmp/logs/MyApp";
        var files = CreateTempFiles(1);
        var fileInfos = files.Select(f => new FileInfo(f)).ToList();

        SetupDirectory(dir, files, fileInfos);
        _fileNamer.GetBundleFileName("MyApp", Arg.Any<IReadOnlyList<FileInfo>>(), "zip").Returns("bundle.zip");
        _fileSystem.FileExists(Arg.Is<string>(p => p != null && p.Contains("bundle.zip", StringComparison.Ordinal))).Returns(returnThis: true);

        var result = await _sut.ExecuteAsync(new CompressionOptions { SourcePath = dir });

        Assert.False(result.Success);
        Assert.Contains("already exists", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
        _fileSystem.DidNotReceive().DeleteFile(files[0]);
    }



    private void SetupDirectory(string dir, string[] files, List<FileInfo> fileInfos)
    {
        _fileSystem.FileExists(dir).Returns(returnThis: false);
        _fileSystem.DirectoryExists(dir).Returns(returnThis: true);
        _fileSystem.EnumerateFiles(dir, "*", SearchOption.TopDirectoryOnly).Returns(files);

        for (var i = 0; i < files.Length; i++)
        {
            _fileSystem.GetFileInfo(files[i]).Returns(fileInfos[i]);
            _fileSystem.OpenRead(files[i]).Returns(new MemoryStream("content"u8.ToArray()));
        }

        _fileFilter.Apply
        (
            Arg.Any<IEnumerable<FileInfo>>(),
            Arg.Any<int?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<IReadOnlyList<string>>(),
            Arg.Any<IReadOnlyList<string>>()
        ).Returns(fileInfos);
    }



    [Fact]
    public async Task ExecuteAsync_when_directoryContainsLockFile_expected_lockFileSkipped()
    {
        // The run's own live .logc.lock sits in the source directory; it must
        // never be bundled (and then deleted) — #172.
        var dir = "/tmp/logs/MyApp";
        var file = CreateTempFiles(1)[0];
        var fileInfo = new FileInfo(file);
        var lockPath = "/tmp/logs/MyApp/.logc.lock";

        _fileSystem.FileExists(dir).Returns(returnThis: false);
        _fileSystem.DirectoryExists(dir).Returns(returnThis: true);
        _fileSystem.EnumerateFiles(dir, "*", SearchOption.TopDirectoryOnly).Returns([file, lockPath]);
        _fileSystem.GetFileInfo(file).Returns(fileInfo);
        _fileFilter.Apply
        (
            Arg.Any<IEnumerable<FileInfo>>(),
            Arg.Any<int?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<IReadOnlyList<string>>(),
            Arg.Any<IReadOnlyList<string>>()
        ).Returns([fileInfo]);
        _fileNamer.GetBundleFileName("MyApp", Arg.Any<IReadOnlyList<FileInfo>>(), "zip").Returns("bundle.zip");
        _fileSystem.OpenRead(Arg.Any<string>()).Returns(new MemoryStream("content"u8.ToArray()));
        _fileSystem.CreateWrite(Arg.Any<string>()).Returns(new MemoryStream());

        var options = new CompressionOptions { SourcePath = dir };
        var result = await _sut.ExecuteAsync(options);

        Assert.True(result.Success);
        _fileSystem.DidNotReceive().GetFileInfo(lockPath);
    }



    [Fact]
    public async Task ExecuteAsync_when_verifyDisabled_expected_verifierNotCalled()
    {
        var dir = "/tmp/logs/MyApp";
        var file = CreateTempFiles(1)[0];
        var fileInfo = new FileInfo(file);

        _fileSystem.FileExists(dir).Returns(returnThis: false);
        _fileSystem.DirectoryExists(dir).Returns(returnThis: true);
        _fileSystem.EnumerateFiles(dir, "*", SearchOption.TopDirectoryOnly).Returns([file]);
        _fileSystem.GetFileInfo(file).Returns(fileInfo);
        _fileFilter.Apply
        (
            Arg.Any<IEnumerable<FileInfo>>(),
            Arg.Any<int?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<IReadOnlyList<string>>(),
            Arg.Any<IReadOnlyList<string>>()
        ).Returns([fileInfo]);
        _fileNamer.GetBundleFileName("MyApp", Arg.Any<IReadOnlyList<FileInfo>>(), "zip").Returns("bundle.zip");
        _fileSystem.OpenRead(Arg.Any<string>()).Returns(new MemoryStream("content"u8.ToArray()));
        _fileSystem.CreateWrite(Arg.Any<string>()).Returns(new MemoryStream());

        var options = new CompressionOptions { SourcePath = dir, Verify = false };
        var result = await _sut.ExecuteAsync(options);

        Assert.True(result.Success);
        await _archiveVerifier.DidNotReceive().VerifyAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<long?>());
    }



    [Fact]
    public async Task ExecuteAsync_when_recurseDisabled_expected_topDirectoryOnlyEnumeration()
    {
        var dir = "/tmp/logs/MyApp";

        _fileSystem.FileExists(dir).Returns(returnThis: false);
        _fileSystem.DirectoryExists(dir).Returns(returnThis: true);
        _fileSystem.EnumerateFiles(dir, "*", Arg.Any<SearchOption>()).Returns([]);
        _fileFilter.Apply
        (
            Arg.Any<IEnumerable<FileInfo>>(),
            Arg.Any<int?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<IReadOnlyList<string>>(),
            Arg.Any<IReadOnlyList<string>>()
        ).Returns([]);

        await _sut.ExecuteAsync(new CompressionOptions { SourcePath = dir, Recurse = false });

        _fileSystem.Received(1).EnumerateFiles(dir, "*", SearchOption.TopDirectoryOnly);
        _fileSystem.DidNotReceive().EnumerateFiles(dir, "*", SearchOption.AllDirectories);
    }



    [Fact]
    public async Task ExecuteAsync_when_onErrorFail_withUnreadableInput_expected_bundleFails()
    {
        var dir = "/tmp/logs/MyApp";
        var files = CreateTempFiles(2);
        var infos = files.Select(f => new FileInfo(f)).ToList();

        _fileSystem.FileExists(dir).Returns(returnThis: false);
        _fileSystem.DirectoryExists(dir).Returns(returnThis: true);
        _fileSystem.EnumerateFiles(dir, "*", SearchOption.TopDirectoryOnly).Returns(files);
        _fileSystem.GetFileInfo(files[0]).Returns(infos[0]);
        _fileSystem.GetFileInfo(files[1]).Returns(infos[1]);
        _fileFilter.Apply
        (
            Arg.Any<IEnumerable<FileInfo>>(),
            Arg.Any<int?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<IReadOnlyList<string>>(),
            Arg.Any<IReadOnlyList<string>>()
        ).Returns(infos);
        _fileNamer.GetBundleFileName("MyApp", Arg.Any<IReadOnlyList<FileInfo>>(), "zip", Arg.Any<TimestampSource>(), Arg.Any<string?>()).Returns("bundle.zip");
        _fileSystem.OpenRead(files[0]).Returns(_ => throw new IOException("locked"));
        _fileSystem.OpenRead(files[1]).Returns(new MemoryStream("content"u8.ToArray()));
        _fileSystem.CreateWrite(Arg.Any<string>()).Returns(new MemoryStream());

        var options = new CompressionOptions
        {
            SourcePath = dir,
            OnError = new ErrorPolicy { Mode = OnErrorMode.Fail }
        };
        var result = await _sut.ExecuteAsync(options);

        Assert.False(result.Success);
        Assert.Contains("--on-error fail", result.ErrorMessage, StringComparison.Ordinal);
        _fileSystem.DidNotReceive().DeleteFile(Arg.Any<string>());
    }



    [Fact]
    public async Task ExecuteAsync_when_onErrorRetry_withTransientUnreadable_expected_inputBundled()
    {
        var dir = "/tmp/logs/MyApp";
        var file = CreateTempFiles(1)[0];
        var info = new FileInfo(file);

        _fileSystem.FileExists(dir).Returns(returnThis: false);
        _fileSystem.DirectoryExists(dir).Returns(returnThis: true);
        _fileSystem.EnumerateFiles(dir, "*", SearchOption.TopDirectoryOnly).Returns([file]);
        _fileSystem.GetFileInfo(file).Returns(info);
        _fileFilter.Apply
        (
            Arg.Any<IEnumerable<FileInfo>>(),
            Arg.Any<int?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<IReadOnlyList<string>>(),
            Arg.Any<IReadOnlyList<string>>()
        ).Returns([info]);
        _fileNamer.GetBundleFileName("MyApp", Arg.Any<IReadOnlyList<FileInfo>>(), "zip", Arg.Any<TimestampSource>(), Arg.Any<string?>()).Returns("bundle.zip");
        // Throws once, then readable — the transient sharing-violation shape.
        var attempts = 0;
        _fileSystem.OpenRead(file).Returns(_ =>
            ++attempts == 1 ? throw new IOException("locked") : new MemoryStream("content"u8.ToArray()));
        _fileSystem.CreateWrite(Arg.Any<string>()).Returns(new MemoryStream());

        var options = new CompressionOptions
        {
            SourcePath = dir,
            OnError = new ErrorPolicy { RetryCount = 2 }
        };
        var result = await _sut.ExecuteAsync(options);

        Assert.True(result.Success);
        _fileSystem.Received(1).DeleteFile(file);
    }



    private string[] CreateTempFiles(int count)
    {
        var files = new string[count];

        for (var i = 0; i < count; i++)
        {
            files[i] = _tempDir.WriteFile(Guid.NewGuid() + ".log", $"content {i}");
        }

        return files;
    }



    public void Dispose()
    {
        _tempDir.Dispose();
    }
}
