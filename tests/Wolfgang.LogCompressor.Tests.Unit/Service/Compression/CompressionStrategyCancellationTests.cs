using NSubstitute;
using Wolfgang.LogCompressor.Model;
using Wolfgang.LogCompressor.Service.Compression;

namespace Wolfgang.LogCompressor.Tests.Unit.Service.Compression;

/// <summary>
/// Every bundle strategy checks the cancellation token between entries. The source sequence here
/// deliberately ignores the token it is handed, so the strategy's own per-entry check is the only
/// thing standing between a canceled run and one more entry being read and written.
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



    // Ignores the token on purpose (no [EnumeratorCancellation]): only the strategy can notice the cancel.
    private static async IAsyncEnumerable<(Stream Stream, string EntryName)> SingleEntry(Stream stream)
    {
        await Task.Yield();
        yield return (stream, "a.log");
    }
}
