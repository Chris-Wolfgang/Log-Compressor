using Wolfgang.LogCompressor.Model;

namespace Wolfgang.LogCompressor.Tests.Unit.Model;

public sealed class ErrorPolicyTests
{
    // Mode passed by name: OnErrorMode is internal, and xunit Theory methods
    // must be public (CS0051 otherwise).
    [Theory]
    [InlineData("skip", "Skip", 0)]
    [InlineData("SKIP", "Skip", 0)]
    [InlineData("fail", "Fail", 0)]
    [InlineData("retry:1", "Skip", 1)]
    [InlineData("retry:100", "Skip", 100)]
    public void TryParse_when_validPolicy_expected_parsed(string value, string modeName, int retries)
    {
        Assert.True(ErrorPolicy.TryParse(value, out var policy));
        Assert.Equal(Enum.Parse<OnErrorMode>(modeName), policy.Mode);
        Assert.Equal(retries, policy.RetryCount);
    }



    [Theory]
    [InlineData(1, 200)]
    [InlineData(5, 1000)]
    [InlineData(10, 2000)]
    [InlineData(100, 2000)]
    public void RetryDelay_when_attemptGiven_expected_linearBackoffCappedAtTwoSeconds(int attempt, int expectedMs)
    {
        Assert.Equal(TimeSpan.FromMilliseconds(expectedMs), ErrorPolicy.RetryDelay(attempt));
    }



    [Theory]
    [InlineData("retry:0")]
    [InlineData("retry:101")]
    [InlineData("retry:")]
    [InlineData("retry:abc")]
    [InlineData("retry:-1")]
    [InlineData("continue")]
    [InlineData("")]
    public void TryParse_when_invalidPolicy_expected_false(string value)
    {
        Assert.False(ErrorPolicy.TryParse(value, out var policy));
        Assert.Equal(ErrorPolicy.Default, policy);
    }
}
