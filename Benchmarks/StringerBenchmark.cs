using BenchmarkDotNet.Attributes;
using ConsoleApp.Configurations;
using System.Globalization;

namespace ConsoleApp.Benchmarks;

[Config(typeof(StringerBenchmarkConfig))]
public class StringerBenchmark
{
	private const int sliceStart = 3;
	private const int sliceEnd = 5;

	public static readonly string InputDirPath = Path.Combine(Directory.GetCurrentDirectory(), "stringer-input");

	public string[] Strings { get; set; }

	public StringerBenchmark()
	{
		string filePath = Path.Combine(InputDirPath, "input.txt");

		Strings = File.ReadAllLines(filePath);
	}

	[Benchmark]
	public List<string> Stringer() => Strings
		.Select(Reverse)
		.Select(str => Substring(str, sliceStart, sliceEnd - sliceStart))
		.Select(Reverse)
		.ToList();

	[Benchmark]
	public List<string> Stringer_AsParallel() => Strings.AsParallel()
		.Select(Reverse)
		.Select(str => Substring(str, sliceStart, sliceEnd - sliceStart))
		.Select(Reverse)
		.ToList();

	private static string Substring(string stringToSubstring, int startIndex, int substringLength) =>
		string.Create(stringToSubstring.Length, stringToSubstring, (chars, span) =>
		{
			ReadOnlySpan<char> stateSpan = span.AsSpan();

			var enumerator = StringInfo.GetTextElementEnumerator(span);

			enumerator.MoveNext();

			int i = 0;
			while (i < startIndex)
			{
				enumerator.MoveNext();
				++i;
			}

			int start = enumerator.ElementIndex;
			int position = 0;

			int j = 0;
			while (j < substringLength && enumerator.MoveNext())
			{
				int next = enumerator.ElementIndex;
				int length = next - start;
				stateSpan[start..next].CopyTo(chars[position..(position + length)]);
				position += length;
				start = next;

				++j;
			}
		});

	private static string Reverse(string stringToReverse) =>
		string.Create(stringToReverse.Length, stringToReverse, (chars, state) =>
		{
			ReadOnlySpan<char> stateSpan = state.AsSpan();
			var enumerator = StringInfo.GetTextElementEnumerator(state);

			enumerator.MoveNext();

			int start = enumerator.ElementIndex;
			int position = chars.Length;

			while (enumerator.MoveNext())
			{
				int next = enumerator.ElementIndex;
				int length = next - start;
				stateSpan[start..next].CopyTo(chars[(position - length)..position]);
				position -= length;
				start = next;
			}

			if (start != 0)
			{
				stateSpan[start..].CopyTo(chars[0..position]);
			}
		});
}
