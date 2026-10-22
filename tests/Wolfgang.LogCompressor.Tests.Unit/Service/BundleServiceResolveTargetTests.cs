using Wolfgang.LogCompressor.Service;

namespace Wolfgang.LogCompressor.Tests.Unit.Service;

public sealed class BundleServiceResolveTargetTests
{
    [Fact]
    public void ResolveTarget_when_fileSource_expected_fileDirectoryAndNameWithoutExtension()
    {
        var file = Path.Combine(Path.GetTempPath(), "logs", "app.log");

        var (outputDirectory, folderName) = BundleService.ResolveTarget(file, sourceIsFile: true, outputPath: null);

        Assert.Equal(Path.Combine(Path.GetTempPath(), "logs"), outputDirectory);
        Assert.Equal("app", folderName);
    }



    [Fact]
    public void ResolveTarget_when_directorySource_expected_parentDirectoryAndDirectoryName()
    {
        var dir = Path.Combine(Path.GetTempPath(), "logs", "MyApp");

        var (outputDirectory, folderName) = BundleService.ResolveTarget(dir, sourceIsFile: false, outputPath: null);

        Assert.Equal(Path.Combine(Path.GetTempPath(), "logs"), outputDirectory);
        Assert.Equal("MyApp", folderName);
    }



    [Fact]
    public void ResolveTarget_when_rootDirectory_expected_rootItselfAndArchiveName()
    {
        // A root has no parent and its Name is the root ("C:\\", "/"), not a
        // usable archive name (#303).
        var root = Path.GetPathRoot(Path.GetTempPath())!;

        var (outputDirectory, folderName) = BundleService.ResolveTarget(root, sourceIsFile: false, outputPath: null);

        Assert.Equal(root, outputDirectory);
        Assert.Equal("archive", folderName);
    }



    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ResolveTarget_when_outputGiven_expected_outputWins(bool sourceIsFile)
    {
        var output = Path.Combine(Path.GetTempPath(), "out");

        var (outputDirectory, _) = BundleService.ResolveTarget(Path.Combine(Path.GetTempPath(), "logs", "x"), sourceIsFile, output);

        Assert.Equal(output, outputDirectory);
    }



    [Fact]
    public void ResolveTarget_when_rootDirectoryWithOutput_expected_outputWins()
    {
        var root = Path.GetPathRoot(Path.GetTempPath())!;
        var output = Path.Combine(Path.GetTempPath(), "out");

        var (outputDirectory, folderName) = BundleService.ResolveTarget(root, sourceIsFile: false, output);

        Assert.Equal(output, outputDirectory);
        Assert.Equal("archive", folderName);
    }
}
