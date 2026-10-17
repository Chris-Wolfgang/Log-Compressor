namespace Wolfgang.LogCompressor.Framework;

/// <summary>
/// Rewrites <c>@file</c> response-file arguments whose files hold lines that McMaster's
/// line-separated mode would turn into bogus arguments.
/// </summary>
/// <remarks>
/// <para>
/// In <c>ResponseFileHandling.ParseArgsAsLineSeparated</c> only a line that starts with <c>#</c>
/// in the first column is a comment. A blank line, a whitespace-only line or an indented comment
/// becomes an <b>empty argument</b>, which claims the first positional slot, so the real path is
/// reported as unrecognized. The file <c>logc init</c> generates is full of blank lines, so
/// editing it as its own header instructs made it unusable.
/// </para>
/// <para>
/// A file that needs no change is passed through untouched. Otherwise the offending lines are
/// dropped into a temporary copy that McMaster reads instead; everything else (<c>--opt value</c>
/// splitting, comments, quoting) stays McMaster's own behaviour. The user's file is never modified.
/// </para>
/// </remarks>
internal static class ResponseFileSanitizer
{
    // `init` turns response files off (ResponseFileHandling.Disabled), so '@' is a literal there.
    private const string InitCommand = "init";



    /// <summary>
    /// Returns arguments whose response files contain no blank, whitespace-only or indented-comment lines.
    /// </summary>
    /// <param name="args">The command-line arguments.</param>
    /// <param name="temporaryDirectory">Where to put cleaned copies; the system temp directory when <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token to observe while reading the files.</param>
    /// <returns>The arguments to hand to the parser; dispose the result to delete any temporary copies.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="args"/> is <see langword="null"/>.</exception>
    internal static async Task<SanitizedArguments> SanitizeAsync
    (
        string[] args,
        string? temporaryDirectory = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(args);
        var directory = temporaryDirectory ?? Path.GetTempPath();

        if (args.Length > 0 && string.Equals(args[0], InitCommand, StringComparison.OrdinalIgnoreCase))
        {
            return new SanitizedArguments(args, []);
        }

        var rewritten = new string[args.Length];
        var temporaryFiles = new List<string>();
        var sanitized = new SanitizedArguments(rewritten, temporaryFiles);

        try
        {
            for (var i = 0; i < args.Length; i++)
            {
                rewritten[i] = await SanitizeArgumentAsync(args[i], directory, temporaryFiles, cancellationToken).ConfigureAwait(false);
            }
        }
        catch
        {
            sanitized.Dispose();
            throw;
        }

        return sanitized;
    }



    private static async Task<string> SanitizeArgumentAsync
    (
        string arg,
        string directory,
        List<string> temporaryFiles,
        CancellationToken cancellationToken
    )
    {
        if (arg.Length < 2 || arg[0] != '@')
        {
            return arg;
        }

        var path = arg[1..];
        if (!File.Exists(path))
        {
            return arg;
        }

        var lines = await File.ReadAllLinesAsync(path, cancellationToken).ConfigureAwait(false);
        var kept = lines.Where(static line => !IsMishandled(line)).ToArray();
        if (kept.Length == lines.Length)
        {
            return arg;
        }

        var temporaryPath = Path.Combine(directory, $"logc-{Guid.NewGuid():N}.rsp");

        // Tracked before the write so a failed or canceled write still gets its partial file deleted.
        temporaryFiles.Add(temporaryPath);
        await File.WriteAllLinesAsync(temporaryPath, kept, cancellationToken).ConfigureAwait(false);

        return "@" + temporaryPath;
    }



    // The lines McMaster turns into arguments although they carry none: a blank or whitespace-only
    // line, or a comment that does not start in the first column. A comment that does start there
    // is skipped by McMaster itself, so it is not touched.
    private static bool IsMishandled(string line)
    {
        var trimmed = line.AsSpan().TrimStart();
        return trimmed.IsEmpty || (trimmed[0] == '#' && line[0] != '#');
    }
}
