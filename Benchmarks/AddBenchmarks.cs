using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace IntSet.Benchmarks;

[MarkdownExporterAttribute.GitHub]
[SimpleJob(RuntimeMoniker.Net80, launchCount: 1, warmupCount: 3, iterationCount: 100)]
[MemoryDiagnoser(false)]
public class AddBenchmarks : IntSetBenchmarkBase
{
    [Benchmark]
    public void IntSet_Add()
    {
        for (var i = 0; i < N; i++)
            intSet.Add(lookupKeys[i]);
    }
    
    [Benchmark]
    public void DenseIdMapAdd()
    {
        for (var i = 0; i < N; i++)
            DenseIdMap.GetOrAdd(lookupKeys[i]);
    }

    [Benchmark(Baseline = true)]
    public void HashSet_Add()
    {
        for (var i = 0; i < N; i++)
            hashSet.Add(lookupKeys[i]);
    }
}