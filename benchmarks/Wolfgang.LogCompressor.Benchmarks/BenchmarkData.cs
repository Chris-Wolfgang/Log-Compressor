using System.Text;

namespace Wolfgang.LogCompressor.Benchmarks;

/// <summary>
/// Test data shared by the throughput benchmarks.
/// </summary>
internal static class BenchmarkData
{
    private const string LogLine = "2026-03-15 23:00:15.123 [INF] Processing request id=abc123 method=GET path=/api/data duration=42ms\n";



    /// <summary>
    /// Returns <paramref name="size"/> bytes of one repeated, realistic log line (UTF-8).
    /// </summary>
    /// <param name="size">The number of bytes to return.</param>
    /// <returns>The test data.</returns>
    internal static byte[] RepeatedLogLines(int size)
    {
        var sb = new StringBuilder(size + LogLine.Length);
        while (sb.Length < size)
        {
            sb.Append(LogLine);
        }

        return Encoding.UTF8.GetBytes(sb.ToString(0, size));
    }
}
