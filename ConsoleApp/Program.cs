using BenchmarkDotNet.Running;
using ConsoleApp.Benchmarks;

BenchmarkSwitcher.FromAssembly(typeof(NumberCrusherBenchmark).Assembly).Run(args);