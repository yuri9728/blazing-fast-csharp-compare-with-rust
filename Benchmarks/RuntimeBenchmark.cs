using BenchmarkDotNet.Attributes;
using ConsoleApp.Configurations;

namespace ConsoleApp.Benchmarks;

[Config(typeof(RuntimeBenchmarkConfig))]
public class RuntimeBenchmark
{
	public static readonly string InputDirPath = Path.Combine(Directory.GetCurrentDirectory(), "runtime-input");
	public static readonly string OutputDirPath = Path.Combine(Directory.GetCurrentDirectory(), "runtime-output");

	readonly record struct PathPair(string InputPath, string OutputPath);

	private readonly PathPair[] pathPairs;

	public RuntimeBenchmark()
	{
		string[] inputPaths = Directory.GetFiles(InputDirPath);

		pathPairs = inputPaths.Select(inputPath => new PathPair(inputPath, GetOutputPath(inputPath))).ToArray();

		_ = Directory.CreateDirectory(OutputDirPath);

		static string GetOutputPath(string inputPath)
		{
			string inputFileName = Path.GetFileName(inputPath);
			string outputPath = Path.Combine(OutputDirPath, $"output-for-{inputFileName}");

			return outputPath;
		}
	}

	[GlobalCleanup]
	public void Cleanup()
	{
		foreach (string outputPath in pathPairs.Select(pair => pair.OutputPath))
		{
			File.Delete(outputPath);
		}
	}

	[Benchmark]
	public async Task Runtime()
	{
		var tasks = pathPairs.Select(CopyFromInputToOutput);

		await Task.WhenAll(tasks);
	}

	[Benchmark]
	public async Task Runtime_ForEachAsync() =>
		await Parallel.ForEachAsync(pathPairs, async (pathPair, cancellationToken) =>
			await CopyFromInputToOutput(pathPair));

	private static async Task CopyFromInputToOutput(PathPair pathPair)
	{
		byte[] fileContent = await File.ReadAllBytesAsync(pathPair.InputPath);

		await File.WriteAllBytesAsync(pathPair.OutputPath, fileContent);
	}
}
