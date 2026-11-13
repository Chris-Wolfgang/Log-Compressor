using Wolfgang.LogCompressor.Command;

namespace Wolfgang.LogCompressor.Tests.Unit.Command;

public sealed class RunOptionValidationTests
{
    [Fact]
    public void Validate_when_errorPolicyInvalid_expected_messageNamesValueAndChoices()
    {
        Assert.Equal
        (
            "Error: Unsupported error policy: 'sometimes'. Supported: skip, fail, retry:N (1-100)",
            RunOptionValidation.Validate("sometimes", report: null, reportPath: null)
        );
    }



    [Fact]
    public void Validate_when_reportFormatInvalid_expected_messageNamesValueAndChoices()
    {
        Assert.Equal
        (
            "Error: Unsupported report format: 'xml'. Supported: json, csv",
            RunOptionValidation.Validate("skip", "xml", reportPath: null)
        );
    }



    [Fact]
    public void Validate_when_reportPathWithoutReport_expected_messageNamesBothOptions()
    {
        Assert.Equal
        (
            "Error: --report-path requires --report to be specified.",
            RunOptionValidation.Validate("skip", report: null, "r.json")
        );
    }



    [Theory]
    [InlineData("skip", null, null)]
    [InlineData("retry:3", "JSON", "r.json")]
    [InlineData("fail", "csv", null)]
    public void Validate_when_allValid_expected_null(string onError, string? report, string? reportPath)
    {
        Assert.Null(RunOptionValidation.Validate(onError, report, reportPath));
    }
}
