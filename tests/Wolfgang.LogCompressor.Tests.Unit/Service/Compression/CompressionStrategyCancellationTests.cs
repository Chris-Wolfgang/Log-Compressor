using NSubstitute;
using Wolfgang.LogCompressor.Model;
using Wolfgang.LogCompressor.Service.Compression;

namespace Wolfgang.LogCompressor.Tests.Unit.Service.Compression;

/// <summary>
/// Regression tests for cancellation: a canceled run must stop before the next entry is read or
/// written. The source sequence here deliberately ignores the token it is handed, so any stop has
/// to come from the strategy itself. These tests do not isolate the strategy's explicit per-entry
/// <c>ThrowIfCancellationRequested()</c>: the downstream archive APIs also observe the token, so
/// removing that call still leaves them passing.
/// </summary>
public sealed class CompressionStrategyCancellationTests
{
    private readonly CompressionStrategyFactory _factory = new();



    [Theory]
    [InlineData("Zip")]
    [InlineData("Gz")]
    [InlineData("Brotli")]
    [InlineData("Zstd")]
    [InlineData("Lz4")]
    public async Task CompressFilesAsync_when_tokenAlreadyCanceled_expected_canceledWithoutReadingTheEntry(string formatName)
    {
        var strategy = _factory.Create(Enum.Parse<CompressionFormat>(formatName));
        using var source = Substitute.ForPartsOf<MemoryStream>("payload"u8.ToArray());
        using var output = new MemoryStream();
        using var cancellation = new CancellationTokenSource();
        await cancellation.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>
        (
            () => strategy.CompressFilesAsync(SingleEntry(source), output, cancellation.Token)
        );

        var readCalls = source
            .ReceivedCalls()
            .Select(call => call.GetMethodInfo().Name)
            .Where(name => name.StartsWith("Read", StringComparison.Ordinal) || name.StartsWith("CopyTo", StringComparison.Ordinal))
            .ToList();
        Assert.Empty(readCalls);
    }



    [Theory]
    [InlineData("Zip")]
    [InlineData("Gz")]
    [InlineData("Brotli")]
    [InlineData("Zstd")]
    [InlineData("Lz4")]
    public async Task CompressFilesAsync_when_notCanceled_expected_entryIsReadThroughTheSubstitute(string formatName)
    {
        // Positive control for the test above: proves the substitute really records reads, so
        // "no reads" there means the strategy never touched the entry rather than that
        // the substitute could not see it.
        var strategy = _factory.Create(Enum.Parse<CompressionFormat>(formatName));
        using var source = Substitute.ForPartsOf<MemoryStream>("payload"u8.ToArray());
        using var output = new MemoryStream();

        await strategy.CompressFilesAsync(SingleEntry(source), output);

        var readCalls = source
            .ReceivedCalls()
            .Select(call => call.GetMethodInfo().Name)
            .Where(name => name.StartsWith("Read", StringComparison.Ordinal) || name.StartsWith("CopyTo", StringComparison.Ordinal))
            .ToList();
        Assert.NotEmpty(readCalls);
    }



    // Ignores the token on purpose (no [EnumeratorCancellation]): only the strategy can notice the cancel.
    private static async IAsyncEnumerable<(Stream Stream, string EntryName)> SingleEntry(Stream stream)
    {
        await Task.Yield();
        yield return (stream, "a.log");
    }
}
