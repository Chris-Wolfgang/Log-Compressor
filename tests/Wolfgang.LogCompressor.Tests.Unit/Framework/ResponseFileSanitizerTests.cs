using McMaster.Extensions.CommandLineUtils;
using Wolfgang.LogCompressor.Framework;

namespace Wolfgang.LogCompressor.Tests.Unit.Framework;

public sealed class ResponseFileSanitizerTests : IDisposable
{
    private readonly TempDirectory _directory = new();
    private readonly string _copies;



    public ResponseFileSanitizerTests()
    {
        _copies = _directory.CreateSubdirectory();
    }



    public void Dispose()
    {
        _directory.Dispose();
    }



    [Fact]
    public async Task SanitizeAsync_when_argsNull_expected_ArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => ResponseFileSanitizer.SanitizeAsync(null!));
    }



    [Theory]
    [InlineData("compress", "/logs", "--recurse")]
    [InlineData("compress", "@", "--recurse")]
    [InlineData("compress", "x@y", "-@z")]
    public async Task SanitizeAsync_when_noResponseFileArgument_expected_argsUnchanged(params string[] args)
    {
        using var sanitized = await ResponseFileSanitizer.SanitizeAsync(args, _copies);

        Assert.Equal(args, sanitized.Args);
        Assert.Empty(Directory.GetFiles(_copies));
    }



    [Fact]
    public async Task SanitizeAsync_when_noArgs_expected_empty()
    {
        using var sanitized = await ResponseFileSanitizer.SanitizeAsync([], _copies);

        Assert.Empty(sanitized.Args);
    }



    [Fact]
    public async Task SanitizeAsync_when_responseFileIsClean_expected_argUntouchedAndNoCopy()
    {
        // Column-1 comments and "--opt value" lines are McMaster's own business.
        var path = _directory.WriteFile("clean.rsp", "# header\ncompress\n/logs\n--recurse\n--older-than 7\n");

        using var sanitized = await ResponseFileSanitizer.SanitizeAsync(["@" + path], _copies);

        Assert.Equal(["@" + path], sanitized.Args);
        Assert.Empty(Directory.GetFiles(_copies));
    }



    [Theory]
    [InlineData("compress\n/logs\n\n--recurse\n", "compress|/logs|--recurse")]
    [InlineData("\ncompress\n/logs\n--recurse\n", "compress|/logs|--recurse")]
    [InlineData("compress\n/logs\n--recurse\n\n\n", "compress|/logs|--recurse")]
    [InlineData("compress\n/logs\n   \n--recurse\n", "compress|/logs|--recurse")]
    [InlineData("compress\n/logs\n\t\n--recurse\n", "compress|/logs|--recurse")]
    [InlineData("compress\n/logs\n   # indented\n--recurse\n", "compress|/logs|--recurse")]
    [InlineData("compress\n/logs\n\t# tab-indented\n--recurse\n", "compress|/logs|--recurse")]
    [InlineData("compress\r\n/logs\r\n\r\n--recurse\r\n", "compress|/logs|--recurse")]
    [InlineData("# keep me\ncompress\n\n/logs\n--older-than 7\n", "# keep me|compress|/logs|--older-than 7")]
    public async Task SanitizeAsync_when_responseFileHasMishandledLines_expected_copyWithoutThem(string content, string expected)
    {
        var path = _directory.WriteFile("edited.rsp", content);

        using var sanitized = await ResponseFileSanitizer.SanitizeAsync(["@" + path], _copies);

        var argument = Assert.Single(sanitized.Args);
        Assert.StartsWith("@", argument, StringComparison.Ordinal);
        Assert.NotEqual("@" + path, argument);

        var copyPath = argument[1..];
        Assert.Equal(_copies, Path.GetDirectoryName(copyPath));
        Assert.Equal(expected, string.Join('|', await File.ReadAllLinesAsync(copyPath)));
    }



    [Fact]
    public async Task SanitizeAsync_when_responseFileRewritten_expected_originalFileNotModified()
    {
        const string content = "compress\n\n/logs\n   # note\n--recurse\n";
        var path = _directory.WriteFile("edited.rsp", content);

        using var sanitized = await ResponseFileSanitizer.SanitizeAsync(["@" + path], _copies);

        Assert.NotEqual("@" + path, Assert.Single(sanitized.Args));
        Assert.Equal(content, await File.ReadAllTextAsync(path));
    }



    [Fact]
    public async Task SanitizeAsync_when_mixedArguments_expected_orderKeptAndOnlyDirtyFilesRewritten()
    {
        var clean = _directory.WriteFile("clean.rsp", "--recurse\n--older-than 7\n");
        var dirty = _directory.WriteFile("dirty.rsp", "--output\n\n/archive\n");

        using var sanitized = await ResponseFileSanitizer.SanitizeAsync(["compress", "@" + clean, "/logs", "@" + dirty, "--no-verify"], _copies);

        Assert.Equal(5, sanitized.Args.Length);
        Assert.Equal("compress", sanitized.Args[0]);
        Assert.Equal("@" + clean, sanitized.Args[1]);
        Assert.Equal("/logs", sanitized.Args[2]);
        Assert.NotEqual("@" + dirty, sanitized.Args[3]);
        Assert.StartsWith("@", sanitized.Args[3], StringComparison.Ordinal);
        Assert.Equal("--no-verify", sanitized.Args[4]);
        Assert.Single(Directory.GetFiles(_copies));
    }



    [Fact]
    public async Task SanitizeAsync_when_responseFileMissing_expected_argUntouched()
    {
        var missing = "@" + Path.Combine(_directory.Path, "does-not-exist.rsp");

        using var sanitized = await ResponseFileSanitizer.SanitizeAsync(["compress", missing], _copies);

        Assert.Equal(["compress", missing], sanitized.Args);
    }



    [Theory]
    [InlineData("init")]
    [InlineData("INIT")]
    public async Task SanitizeAsync_when_firstArgumentIsInit_expected_nothingRewritten(string command)
    {
        // `init` disables response files, so '@' is a literal there and must be left alone.
        var path = _directory.WriteFile("literal.rsp", "compress\n\n/logs\n");
        string[] args = [command, "compress", "@" + path];

        using var sanitized = await ResponseFileSanitizer.SanitizeAsync(args, _copies);

        Assert.Equal(args, sanitized.Args);
        Assert.Empty(Directory.GetFiles(_copies));
    }



    [Fact]
    public async Task SanitizeAsync_when_initIsNotTheFirstArgument_expected_stillSanitized()
    {
        var path = _directory.WriteFile("edited.rsp", "compress\n\n/logs\n");

        using var sanitized = await ResponseFileSanitizer.SanitizeAsync(["compress", "init", "@" + path], _copies);

        Assert.NotEqual("@" + path, sanitized.Args[2]);
    }



    [Fact]
    public async Task Dispose_when_called_expected_copiesDeletedAndSecondCallHarmless()
    {
        var path = _directory.WriteFile("edited.rsp", "compress\n\n/logs\n");
        var sanitized = await ResponseFileSanitizer.SanitizeAsync(["@" + path], _copies);
        Assert.Single(Directory.GetFiles(_copies));

        sanitized.Dispose();
        sanitized.Dispose();

        Assert.Empty(Directory.GetFiles(_copies));
        Assert.True(File.Exists(path));
    }



    [Fact]
    public async Task SanitizeAsync_when_laterFileUnreadable_expected_earlierCopiesDeletedAndExceptionPropagates()
    {
        var dirty = _directory.WriteFile("dirty.rsp", "compress\n\n/logs\n");
        var locked = _directory.WriteFile("locked.rsp", "compress\n");
        await using var holder = new FileStream(locked, FileMode.Open, FileAccess.Read, FileShare.None);

        await Assert.ThrowsAsync<IOException>(() => ResponseFileSanitizer.SanitizeAsync(["@" + dirty, "@" + locked], _copies));

        Assert.Empty(Directory.GetFiles(_copies));
    }



    [Fact]
    public async Task SanitizeAsync_when_canceledWhileReading_expected_OperationCanceledException()
    {
        var path = _directory.WriteFile("edited.rsp", "compress\n\n/logs\n");
        using var cancellation = new CancellationTokenSource();
        await cancellation.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>
        (
            () => ResponseFileSanitizer.SanitizeAsync(["@" + path], _copies, cancellation.Token)
        );

        Assert.Empty(Directory.GetFiles(_copies));
    }



    [Fact]
    public async Task SanitizeAsync_when_fileHasBlankLines_expected_McMasterRejectsRawFileButParsesTheCopy()
    {
        // The regression itself, against the real parser in the same response-file mode the app uses.
        // The raw file is the negative control: if McMaster ever starts skipping these lines
        // itself, that assertion fails and this sanitizer can be retired.
        var path = _directory.WriteFile
        (
            "template.rsp",
            "# logc compress configuration\n\n/logs\n\n# Recurse\n--recurse\n   # indented note\n--older-than 7\n"
        );

        var rejected = Assert.ThrowsAny<CommandParsingException>(() => Parse(["@" + path]));
        Assert.Contains("/logs", rejected.Message, StringComparison.Ordinal);

        using var sanitized = await ResponseFileSanitizer.SanitizeAsync(["@" + path], _copies);
        var model = Parse(sanitized.Args);

        Assert.Equal("/logs", model.Path);
        Assert.True(model.Recurse);
        Assert.Equal(7, model.OlderThan);
    }



    private static ProbeCommand Parse(string[] args)
    {
        var application = new CommandLineApplication<ProbeCommand>();
        application.Conventions.UseDefaultConventions();
        application.Parse(args);
        return application.Model;
    }
}

