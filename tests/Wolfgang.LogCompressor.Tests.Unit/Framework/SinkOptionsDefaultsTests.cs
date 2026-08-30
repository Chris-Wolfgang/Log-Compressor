using Wolfgang.LogCompressor.Framework;

namespace Wolfgang.LogCompressor.Tests.Unit.Framework;

/// <summary>
/// Pins the logging sink option defaults (#176 mutant hunt): these values are
/// the out-of-the-box contract for unattended jobs, and boolean/arithmetic
/// mutations on them previously survived because nothing asserted them.
/// </summary>
public sealed class SinkOptionsDefaultsTests
{
    [Fact]
    public void ConsoleSinkOptions_defaults_expected_enabled()
    {
        var sut = new ConsoleSinkOptions();

        Assert.True(sut.Enabled);
    }



    [Fact]
    public void FileSinkOptions_defaults_expected_enabledRollingTenMiB()
    {
        var sut = new FileSinkOptions();

        Assert.True(sut.Enabled);
        Assert.True(sut.RollOnFileSizeLimit);
        Assert.Equal(10 * 1024 * 1024, sut.FileSizeLimitBytes);
    }
}
