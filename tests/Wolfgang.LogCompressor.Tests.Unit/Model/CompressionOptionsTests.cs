using Wolfgang.LogCompressor.Model;

namespace Wolfgang.LogCompressor.Tests.Unit.Model;

/// <summary>
/// Pins the <see cref="CompressionOptions"/> defaults that guard data: an
/// options instance built without an explicit <c>Verify</c> must still verify
/// every archive before its original is deleted.
/// </summary>
public sealed class CompressionOptionsTests
{
    [Fact]
    public void Verify_when_not_set_expected_true()
    {
        var sut = new CompressionOptions { SourcePath = "app.log" };

        Assert.True(sut.Verify);
    }
}
