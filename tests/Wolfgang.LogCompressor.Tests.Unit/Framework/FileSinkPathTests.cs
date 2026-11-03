using Wolfgang.LogCompressor.Framework;

namespace Wolfgang.LogCompressor.Tests.Unit.Framework;

/// <summary>
/// The file sink path is anchored to the application directory, not the working directory a
/// scheduler picks (#355).
/// </summary>
public sealed class FileSinkPathTests
{
    [Fact]
    public void ResolvePath_when_relative_expected_underBaseDirectory()
    {
        var baseDirectory = Path.Combine(Path.GetTempPath(), "logc-app");

        var resolved = FileSinkOptions.ResolvePath("logs/log-.txt", baseDirectory);

        Assert.Equal(Path.Combine(baseDirectory, "logs", "log-.txt"), resolved);
    }



    [Fact]
    public void ResolvePath_when_absolute_expected_unchanged()
    {
        var absolute = Path.Combine(Path.GetTempPath(), "elsewhere", "log-.txt");

        var resolved = FileSinkOptions.ResolvePath(absolute, Path.Combine(Path.GetTempPath(), "logc-app"));

        Assert.Equal(absolute, resolved);
    }
}
