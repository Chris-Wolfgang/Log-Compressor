using BenchmarkDotNet.Running;
using Wolfgang.LogCompressor.Benchmarks;

BenchmarkSwitcher.FromAssembly(typeof(CompressionBenchmarks).Assembly).Run(args);
