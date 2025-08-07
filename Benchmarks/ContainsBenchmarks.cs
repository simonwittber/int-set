using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace IntSet.Benchmarks;

[MarkdownExporterAttribute.GitHub]
[SimpleJob(RuntimeMoniker.Net80, launchCount: 1, warmupCount: 3, iterationCount: 100)]
[MemoryDiagnoser(false)]
public class ContainsBenchmarks : IntSetBenchmarkBase
{
    [Benchmark]
    public void IntSet_Contains()
    {
        for (var i = 0; i < N; i++)
            intSet.Contains(lookupKeys[i]);
    }
    
    [Benchmark]
    public void DenseIdMapContains()
    {
        for (var i = 0; i < N; i++)
            DenseIdMap.Contains(lookupKeys[i]);
    }

    [Benchmark(Baseline = true)]
    public void HashSet_Contains()
    {
        for (var i = 0; i < N; i++)
            hashSet.Contains(lookupKeys[i]);
    }
}