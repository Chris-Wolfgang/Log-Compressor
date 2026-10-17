namespace Wolfgang.LogCompressor.Framework;

/// <summary>
/// The arguments produced by <see cref="ResponseFileSanitizer"/>, plus the temporary files behind them.
/// </summary>
internal sealed class SanitizedArguments : IDisposable
{
    private readonly IReadOnlyList<string> _temporaryFiles;



    internal SanitizedArguments(string[] args, IReadOnlyList<string> temporaryFiles)
    {
        Args = args;
        _temporaryFiles = temporaryFiles;
    }



    /// <summary>
    /// Gets the arguments to pass to the command-line parser.
    /// </summary>
    internal string[] Args { get; }



    /// <summary>
    /// Deletes any temporary response-file copies. Safe to call more than once.
    /// </summary>
    public void Dispose()
    {
        foreach (var file in _temporaryFiles)
        {
            File.Delete(file);
        }
    }
}
