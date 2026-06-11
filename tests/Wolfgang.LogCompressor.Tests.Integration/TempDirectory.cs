namespace Wolfgang.LogCompressor.Tests.Integration;

/// <summary>
/// Creates a unique temporary directory for an integration test and deletes it
/// (best-effort) on dispose. Integration tests run against the real file system.
/// </summary>
internal sealed class TempDirectory : IDisposable
{
    public string Path { get; }



    public TempDirectory()
    {
        Path = System.IO.Path.Combine
        (
            System.IO.Path.GetTempPath(),
            "logc-it-" + Guid.NewGuid().ToString("N")
        );

        Directory.CreateDirectory(Path);
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
