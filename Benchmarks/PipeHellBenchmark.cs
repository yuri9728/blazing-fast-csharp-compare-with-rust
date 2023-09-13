using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace ConsoleApp.Benchmarks;

[MediumRunJob(RuntimeMoniker.Net80)]
public class PipeHellBenchmark
{
	private int[] source = null!;

	[Params(1_000, 10_000, 1_000_000, 10_000_000)]
	public int N { get; set; }

	[GlobalSetup]
	public void Setup() => source = Enumerable.Range(0, N).ToArray();

	[Benchmark]
	public List<string> PipeHell() => source
		.Zip(source.Reverse())
		.Select(x => (float)x.Second / x.First)
		.Where(x => x > MathF.Sqrt(N))
		.Select(x => x / 1000)
		.Where(x => x < N / MathF.Sqrt(x))
		.Select(MathF.Truncate)
		.Select(x => x.ToString("2x")).ToList();

	[Benchmark]
	public List<string> PipeHell_With_AsParallel() => source
		.Zip(source.Reverse())
		.AsParallel()
		.Select(x => (float)x.Second / x.First)
		.Where(x => x > MathF.Sqrt(N))
		.Select(x => x / 1000)
		.Where(x => x < N / MathF.Sqrt(x))
		.Select(MathF.Truncate)
		.Select(x => x.ToString("2x")).ToList();
}
