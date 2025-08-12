using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace IntSet.Benchmarks;

[MarkdownExporterAttribute.GitHub]
[SimpleJob(RuntimeMoniker.Net80, launchCount: 1, warmupCount: 3, iterationCount: 100)]
[MemoryDiagnoser(false)]
public class UnionWithBenchmarks : IntSetBenchmarkBase
{

    [Benchmark]
    public void IntSet_UnionWith()
    {
        intSet.UnionWith(bKeys);
    }
    
    [Benchmark]
    public void IntSet_UnionWith_IntSet()
    {
        intSet.UnionWith(intSetB);
    }

    [Benchmark(Baseline = true)]
    public void HashSet_UnionWith()
    {
        hashSet.UnionWith(bKeys);
    }
}