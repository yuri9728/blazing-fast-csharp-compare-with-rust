using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace ConsoleApp.Benchmarks;

[MediumRunJob(RuntimeMoniker.Net80)]
public class NumberCrusherBenchmark
{
	private int[] source = null!;

	[Params(1_000, 10_000, 1_000_000, 10_000_000)]
	public int N { get; set; }

	[GlobalSetup]
	public void Setup() => source = Enumerable.Range(0, N).ToArray();

	[Benchmark(Baseline = true)]
	public float NumberCrusher()
	{
		return (from x in source select float.Abs(float.Cos(float.Pow(float.Sin(float.Sqrt(x)), 2)))).Sum();
	}

	[Benchmark(Description = "With AsParallel")]
	public float NumberCrusherAsParallel()
	{
		return (from x in source.AsParallel() select float.Abs(float.Cos(float.Pow(float.Sin(float.Sqrt(x)), 2)))).Sum();
	}
}
