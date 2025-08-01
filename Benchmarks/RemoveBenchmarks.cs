using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace IntSet.Benchmarks;

[MarkdownExporterAttribute.GitHub]
[SimpleJob(RuntimeMoniker.Net80, launchCount: 1, warmupCount: 3, iterationCount: 100)]
[MemoryDiagnoser(false)]
public class RemoveBenchmarks : IntSetBenchmarkBase
{
    [Benchmark]
    public void IntSet_Remove()
    {
        for (var i = 0; i < N; i++)
            intSet.Remove(lookupKeys[i]);
    }

    [Benchmark(Baseline = true)]
    public void HashSet_Remove()
    {
        for (var i = 0; i < N; i++)
            hashSet.Remove(lookupKeys[i]);
    }
}