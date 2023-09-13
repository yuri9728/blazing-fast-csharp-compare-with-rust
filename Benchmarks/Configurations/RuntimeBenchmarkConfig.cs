using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.InProcess.NoEmit;

namespace ConsoleApp.Configurations;

internal class RuntimeBenchmarkConfig : ManualConfig
{
	public RuntimeBenchmarkConfig()
	{
		var noEmitJob = Job.MediumRun.WithToolchain(InProcessNoEmitToolchain.Instance);

		AddJob(noEmitJob);
	}
}
