using BenchmarkDotNet.Running;
using Wolfgang.LogCompressor.Benchmarks;

if (args.Length > 0 && string.Equals(args[0], "--ratio", StringComparison.OrdinalIgnoreCase))
{
    await CompressionRatioBenchmarks.RunAsync();
}
else
{
    BenchmarkSwitcher.FromAssembly(typeof(CompressionBenchmarks).Assembly).Run(args);
}
