namespace Wolfgang.LogCompressor.Tests.Fuzz;

/// <summary>
/// Creates a unique temporary directory for a fuzz case and deletes it
/// (best-effort) on dispose. Mirrors the unit/integration-test helpers of the
/// same name so cases that touch the real file system leave nothing behind.
/// </summary>
internal sealed class TempDirectory : IDisposable
{
    public string Path { get; }



    public TempDirectory()
    {
        Path = System.IO.Path.Combine
        (
            System.IO.Path.GetTempPath(),
            "logc-fuzz-" + Guid.NewGuid().ToString("N")
        );

        Directory.CreateDirectory(Path);
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
