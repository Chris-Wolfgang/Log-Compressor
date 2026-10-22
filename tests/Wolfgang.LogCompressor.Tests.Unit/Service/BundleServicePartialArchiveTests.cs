using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Wolfgang.LogCompressor.Abstraction;
using Wolfgang.LogCompressor.Model;
using Wolfgang.LogCompressor.Service;
using Wolfgang.LogCompressor.Service.Compression;

namespace Wolfgang.LogCompressor.Tests.Unit.Service;

/// <summary>
/// A bundle is written under <c>.partial</c> and moved to its final name only when complete and
/// verified, so a failed or interrupted bundle never leaves a final-named archive that blocks every
/// later bundle of the same files (#351).
/// </summary>
public sealed class BundleServicePartialArchiveTests : IDisposable
{
    private const string Dir = "/tmp/logs/MyApp";

    private readonly TempDirectory _tempDir = new();
    private readonly IFileSystem _fileSystem = Substitute.For<IFileSystem>();
    private readonly IFileFilter _fileFilter = Substitute.For<IFileFilter>();
    private readonly IFileNamer _fileNamer = Substitute.For<IFileNamer>();
    private readonly IArchiveVerifier _archiveVerifier = Substitute.For<IArchiveVerifier>();
    private readonly ICompressionStrategy _strategy = Substitute.For<ICompressionStrategy>();
    private readonly BundleService _sut;
    private readonly string _source;
    private readonly string _output;
    private readonly string _partial;
    private readonly string _failed;



    public BundleServicePartialArchiveTests()
    {
        var strategyFactory = Substitute.For<CompressionStrategyFactory>();
        strategyFactory.Create(Arg.Any<CompressionFormat>(), Arg.Any<System.IO.Compression.CompressionLevel>()).Returns(_strategy);
        _strategy.BundleFileExtension.Returns("zip");
        _archiveVerifier.VerifyAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<long?>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(true));
        _strategy
            .CompressFilesAsync(Arg.Any<IAsyncEnumerable<(Stream Stream, string EntryName)>>(), Arg.Any<Stream>(), Arg.Any<CancellationToken>())
            .Returns(async callInfo =>
            {
                await foreach (var _ in callInfo.Arg<IAsyncEnumerable<(Stream Stream, string EntryName)>>())
                {
                }
            });

        _source = _tempDir.WriteFile("app.log", "content");
        var fileInfo = new FileInfo(_source);
        _output = Path.Combine(new DirectoryInfo(Dir).Parent!.FullName, "bundle.zip");
        _partial = _output + ".partial";
        _failed = _output + ".failed";

        _fileSystem.FileExists(Dir).Returns(returnThis: false);
        _fileSystem.DirectoryExists(Dir).Returns(returnThis: true);
        _fileSystem.EnumerateFiles(Dir, "*", SearchOption.TopDirectoryOnly).Returns([_source]);
        _fileSystem.GetFileInfo(_source).Returns(fileInfo);
        _fileSystem.OpenRead(_source).Returns(_ => new MemoryStream("content"u8.ToArray()));
        _fileSystem.CreateWrite(Arg.Any<string>()).Returns(_ => new MemoryStream());
        _fileFilter.Apply
        (
            Arg.Any<IEnumerable<FileInfo>>(),
            Arg.Any<int?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<IReadOnlyList<string>>(),
            Arg.Any<IReadOnlyList<string>>()
        ).Returns([fileInfo]);
        _fileNamer.GetBundleFileName("MyApp", Arg.Any<IReadOnlyList<FileInfo>>(), "zip", Arg.Any<TimestampSource>(), Arg.Any<string?>()).Returns("bundle.zip");

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
    public async Task ExecuteAsync_when_bundleVerifies_expected_writtenAsPartialThenMovedToFinalName()
    {
        var result = await _sut.ExecuteAsync(new CompressionOptions { SourcePath = Dir, Verify = true });

        Assert.True(result.Success);
        Assert.Equal(_output, result.OutputPath);
        _fileSystem.Received(1).CreateWrite(_partial);
        _fileSystem.DidNotReceive().CreateWrite(_output);
        await _archiveVerifier.Received(1).VerifyAsync(_partial, "zip", Arg.Any<long?>(), Arg.Any<CancellationToken>());
        Received.InOrder(() =>
        {
            _fileSystem.MoveFile(_partial, _output, false);
            _fileSystem.DeleteFile(_source);
        });
    }



    [Fact]
    public async Task ExecuteAsync_when_verificationFails_expected_bundleKeptAsFailedNotAtFinalName()
    {
        _archiveVerifier.VerifyAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<long?>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(false));

        var result = await _sut.ExecuteAsync(new CompressionOptions { SourcePath = Dir, Verify = true });

        Assert.False(result.Success);
        Assert.Equal("Archive verification failed.", result.ErrorMessage);
        _fileSystem.Received(1).MoveFile(_partial, _failed, true);
        _fileSystem.DidNotReceive().MoveFile(Arg.Any<string>(), _output, Arg.Any<bool>());
        _fileSystem.DidNotReceive().DeleteFile(_source);
    }



    [Fact]
    public async Task ExecuteAsync_when_writeFails_expected_partialDeletedAndNothingAtFinalName()
    {
        _strategy
            .CompressFilesAsync(Arg.Any<IAsyncEnumerable<(Stream Stream, string EntryName)>>(), Arg.Any<Stream>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new IOException("disk full"));
        _fileSystem.When(f => f.CreateWrite(_partial)).Do(_ => _fileSystem.FileExists(_partial).Returns(returnThis: true));

        var result = await _sut.ExecuteAsync(new CompressionOptions { SourcePath = Dir });

        Assert.False(result.Success);
        Assert.Equal("disk full", result.ErrorMessage);
        _fileSystem.Received(1).DeleteFile(_partial);
        _fileSystem.DidNotReceive().MoveFile(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool>());
        _fileSystem.DidNotReceive().DeleteFile(_source);
    }



    [Fact]
    public async Task ExecuteAsync_when_canceledDuringWrite_expected_partialDeletedAndCancellationPropagates()
    {
        _strategy
            .CompressFilesAsync(Arg.Any<IAsyncEnumerable<(Stream Stream, string EntryName)>>(), Arg.Any<Stream>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new OperationCanceledException());
        _fileSystem.When(f => f.CreateWrite(_partial)).Do(_ => _fileSystem.FileExists(_partial).Returns(returnThis: true));

        await Assert.ThrowsAsync<OperationCanceledException>
        (
            () => _sut.ExecuteAsync(new CompressionOptions { SourcePath = Dir })
        );

        _fileSystem.Received(1).DeleteFile(_partial);
        _fileSystem.DidNotReceive().DeleteFile(_source);
    }



    [Fact]
    public async Task ExecuteAsync_when_stalePartialFromEarlierRun_expected_deletedBeforeWriting()
    {
        _fileSystem.FileExists(_partial).Returns(returnThis: true);

        var result = await _sut.ExecuteAsync(new CompressionOptions { SourcePath = Dir });

        Assert.True(result.Success);
        Received.InOrder(() =>
        {
            _fileSystem.DeleteFile(_partial);
            _fileSystem.CreateWrite(_partial);
        });
    }



    [Fact]
    public async Task ExecuteAsync_when_allFilesUnreadable_expected_partialDeleted()
    {
        _fileSystem.OpenRead(_source).Returns(_ => throw new IOException("locked"));

        var result = await _sut.ExecuteAsync(new CompressionOptions { SourcePath = Dir });

        Assert.False(result.Success);
        _fileSystem.Received(1).DeleteFile(_partial);
        _fileSystem.DidNotReceive().MoveFile(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool>());
    }



    [Theory]
    [InlineData("MyApp-old.zip.partial")]
    [InlineData("MyApp-old.zip.failed")]
    public async Task ExecuteAsync_when_directoryHoldsWorkFile_expected_workFileNotBundled(string workFileName)
    {
        var workFile = Dir + "/" + workFileName;
        _fileSystem.EnumerateFiles(Dir, "*", SearchOption.TopDirectoryOnly).Returns([_source, workFile]);

        await _sut.ExecuteAsync(new CompressionOptions { SourcePath = Dir });

        _fileSystem.DidNotReceive().GetFileInfo(workFile);
        _fileSystem.Received(1).GetFileInfo(_source);
    }



    [Fact]
    public async Task ExecuteAsync_when_canceledDuringVerification_expected_tokenPassedPartialDeletedAndCancellationPropagates()
    {
        using var cts = new CancellationTokenSource();
        _archiveVerifier.VerifyAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<long?>(), Arg.Any<CancellationToken>())
            .Returns<Task<bool>>(_ => throw new OperationCanceledException(cts.Token));
        _fileSystem.When(f => f.CreateWrite(_partial)).Do(_ => _fileSystem.FileExists(_partial).Returns(returnThis: true));

        await Assert.ThrowsAsync<OperationCanceledException>
        (
            () => _sut.ExecuteAsync(new CompressionOptions { SourcePath = Dir, Verify = true }, cts.Token)
        );

        await _archiveVerifier.Received(1).VerifyAsync(_partial, "zip", null, cts.Token);
        _fileSystem.Received(1).DeleteFile(_partial);
        _fileSystem.DidNotReceive().MoveFile(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool>());
        _fileSystem.DidNotReceive().DeleteFile(_source);
    }



    public void Dispose()
    {
        _tempDir.Dispose();
    }
}
