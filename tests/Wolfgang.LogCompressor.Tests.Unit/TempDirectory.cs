namespace Wolfgang.LogCompressor.Tests.Unit;

/// <summary>
/// Creates a unique temporary directory for a unit test and deletes it
/// (best-effort) on dispose. Mirrors the integration-test helper of the same
/// name so fixtures that touch the real file system leave nothing behind.
/// </summary>
internal sealed class TempDirectory : IDisposable
{
    public string Path { get; }



    public TempDirectory()
    {
        Path = System.IO.Path.Combine
        (
            System.IO.Path.GetTempPath(),
            "logc-ut-" + Guid.NewGuid().ToString("N")
        );

        Directory.CreateDirectory(Path);
    }



    /// <summary>
    /// Creates a uniquely named subdirectory under the temp root and returns
    /// its full path. Lets one fixture create several isolated directories
    /// that are all removed by a single dispose.
    /// </summary>
    public string CreateSubdirectory()
    {
        var fullPath = System.IO.Path.Combine
        (
            Path,
            Guid.NewGuid().ToString("N")
        );

        Directory.CreateDirectory(fullPath);
        return fullPath;
    }



    /// <summary>
    /// Writes a file with the given name and content into the temp directory and
    /// returns its full path.
    /// </summary>
    public string WriteFile(string name, string content)
    {
        var fullPath = System.IO.Path.Combine(Path, name);
        File.WriteAllText(fullPath, content);
        return fullPath;
    }



    public void Dispose()
    {
        try
        {
            if (Directory.Exists(Path))
            {
                Directory.Delete(Path, recursive: true);
            }
        }
        catch (IOException)
        {
            // Best-effort cleanup — a lingering temp dir must not fail the test.
        }
        catch (UnauthorizedAccessException)
        {
            // Best-effort cleanup.
        }
    }
}
