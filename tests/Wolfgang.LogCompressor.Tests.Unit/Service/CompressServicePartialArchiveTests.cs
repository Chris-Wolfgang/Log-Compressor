using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Wolfgang.LogCompressor.Abstraction;
using Wolfgang.LogCompressor.Model;
using Wolfgang.LogCompressor.Service;
using Wolfgang.LogCompressor.Service.Compression;

namespace Wolfgang.LogCompressor.Tests.Unit.Service;

/// <summary>
/// An archive is written under <c>.partial</c> and moved to its final name only when complete and
/// verified, so a failed or interrupted attempt never leaves a final-named archive that blocks the
/// next run (#351).
/// </summary>
public sealed class CompressServicePartialArchiveTests : IDisposable
{
    private readonly TempDirectory _tempDir = new();
    private readonly IFileSystem _fileSystem = Substitute.For<IFileSystem>();
    private readonly IFileFilter _fileFilter = Substitute.For<IFileFilter>();
    private readonly IFileNamer _fileNamer = Substitute.For<IFileNamer>();
    private readonly IArchiveVerifier _archiveVerifier = Substitute.For<IArchiveVerifier>();
    private readonly ICompressionStrategy _strategy = Substitute.For<ICompressionStrategy>();
    private readonly CompressService _sut;
    private readonly string _source;
    private readonly string _output;
    private readonly string _partial;
    private readonly string _failed;



    public CompressServicePartialArchiveTests()
    {
        var strategyFactory = Substitute.For<CompressionStrategyFactory>();
        strategyFactory.Create(Arg.Any<CompressionFormat>(), Arg.Any<System.IO.Compression.CompressionLevel>()).Returns(_strategy);
        _strategy.FileExtension.Returns("zip");
        _archiveVerifier.VerifyAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<long?>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(true));

        _source = _tempDir.WriteFile("app.log", "content");
        var fileInfo = new FileInfo(_source);
        _output = Path.Combine(_tempDir.Path, "out.zip");
        _partial = _output + ".partial";
        _failed = _output + ".failed";

        _fileSystem.FileExists(_source).Returns(returnThis: true);
        _fileSystem.GetFileInfo(_source).Returns(fileInfo);
        StubFilterPassThrough([fileInfo]);
        _fileNamer.GetCompressedFileName(Arg.Any<FileInfo>(), "zip", Arg.Any<TimestampSource>(), Arg.Any<string?>()).Returns("out.zip");
        _fileSystem.OpenRead(_source).Returns(_ => new MemoryStream("content"u8.ToArray()));
        _fileSystem.CreateWrite(Arg.Any<string>()).Returns(_ => new MemoryStream());

        _sut = new CompressService
        (
            _fileSystem,
            _fileFilter,
            _fileNamer,
            _archiveVerifier,
            strategyFactory,
            Substitute.For<ILogger<CompressService>>()
        );
    }



    [Fact]
    public async Task ExecuteAsync_when_archiveVerifies_expected_writtenAsPartialThenMovedToFinalName()
    {
        var results = await _sut.ExecuteAsync(new CompressionOptions { SourcePath = _source, Verify = true });

        Assert.True(Assert.Single(results).Success);
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
    public async Task ExecuteAsync_when_verificationFails_expected_archiveKeptAsFailedNotAtFinalName()
    {
        _archiveVerifier.VerifyAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<long?>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(false));

        var results = await _sut.ExecuteAsync(new CompressionOptions { SourcePath = _source, Verify = true });

        var result = Assert.Single(results);
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
            .CompressFileAsync(Arg.Any<Stream>(), Arg.Any<Stream>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new IOException("disk full"));
        _fileSystem.When(f => f.CreateWrite(_partial)).Do(_ => _fileSystem.FileExists(_partial).Returns(returnThis: true));

        var results = await _sut.ExecuteAsync(new CompressionOptions { SourcePath = _source });

        var result = Assert.Single(results);
        Assert.False(result.Success);
        Assert.Equal("disk full", result.ErrorMessage);
        _fileSystem.Received(1).DeleteFile(_partial);
        _fileSystem.DidNotReceive().MoveFile(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool>());
        _fileSystem.DidNotReceive().DeleteFile(_source);
    }



    [Fact]
    public async Task ExecuteAsync_when_partialCannotBeDeletedAfterFailure_expected_originalErrorReported()
    {
        _strategy
            .CompressFileAsync(Arg.Any<Stream>(), Arg.Any<Stream>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new IOException("disk full"));
        _fileSystem.When(f => f.CreateWrite(_partial)).Do(_ => _fileSystem.FileExists(_partial).Returns(returnThis: true));
        _fileSystem.When(f => f.DeleteFile(_partial)).Do(_ => throw new UnauthorizedAccessException("denied"));

        var results = await _sut.ExecuteAsync(new CompressionOptions { SourcePath = _source });

        Assert.Equal("disk full", Assert.Single(results).ErrorMessage);
    }



    [Fact]
    public async Task ExecuteAsync_when_canceledDuringWrite_expected_partialDeletedAndCancellationPropagates()
    {
        _strategy
            .CompressFileAsync(Arg.Any<Stream>(), Arg.Any<Stream>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new OperationCanceledException());
        _fileSystem.When(f => f.CreateWrite(_partial)).Do(_ => _fileSystem.FileExists(_partial).Returns(returnThis: true));

        await Assert.ThrowsAsync<OperationCanceledException>
        (
            () => _sut.ExecuteAsync(new CompressionOptions { SourcePath = _source })
        );

        _fileSystem.Received(1).DeleteFile(_partial);
        _fileSystem.DidNotReceive().DeleteFile(_source);
    }



    [Fact]
    public async Task ExecuteAsync_when_stalePartialFromEarlierRun_expected_deletedBeforeWriting()
    {
        _fileSystem.FileExists(_partial).Returns(returnThis: true);

        var results = await _sut.ExecuteAsync(new CompressionOptions { SourcePath = _source });

        Assert.True(Assert.Single(results).Success);
        Received.InOrder(() =>
        {
            _fileSystem.DeleteFile(_partial);
            _fileSystem.CreateWrite(_partial);
        });
    }



    [Theory]
    [InlineData("old.log.zip.partial")]
    [InlineData("old.log.zip.failed")]
    [InlineData("OLD.LOG.ZIP.PARTIAL")]
    public async Task ExecuteAsync_when_directoryHoldsWorkFile_expected_workFileNotCompressed(string workFileName)
    {
        var dir = _tempDir.CreateSubdirectory();
        var workFile = Path.Combine(dir, workFileName);
        _fileSystem.FileExists(dir).Returns(returnThis: false);
        _fileSystem.DirectoryExists(dir).Returns(returnThis: true);
        _fileSystem.EnumerateFiles(dir, "*", SearchOption.TopDirectoryOnly).Returns([_source, workFile]);

        await _sut.ExecuteAsync(new CompressionOptions { SourcePath = dir });

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
            () => _sut.ExecuteAsync(new CompressionOptions { SourcePath = _source, Verify = true }, cts.Token)
        );

        await _archiveVerifier.Received(1).VerifyAsync(_partial, "zip", Arg.Any<long?>(), cts.Token);
        _fileSystem.Received(1).DeleteFile(_partial);
        _fileSystem.DidNotReceive().MoveFile(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool>());
        _fileSystem.DidNotReceive().DeleteFile(_source);
    }



    private void StubFilterPassThrough(IReadOnlyList<FileInfo> files)
    {
        // Any filter arguments: these tests are about what the service does with
        // the files the filter returns, not about the filter itself.
        _fileFilter.Apply
        (
            Arg.Any<IEnumerable<FileInfo>>(),
            Arg.Any<int?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<IReadOnlyList<string>>(),
            Arg.Any<IReadOnlyList<string>>()
        ).Returns(files);
    }



    public void Dispose()
    {
        _tempDir.Dispose();
    }
}
