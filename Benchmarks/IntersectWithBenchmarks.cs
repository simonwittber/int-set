using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace IntSet.Benchmarks;

[MarkdownExporterAttribute.GitHub]
[SimpleJob(RuntimeMoniker.Net80, launchCount: 1, warmupCount: 3, iterationCount: 100)]
[MemoryDiagnoser(false)]
public class IntersectWithBenchmarks : IntSetBenchmarkBase
{
    [Benchmark]
    public void IntSet_IntersectWith()
    {
        IntSetPaged.IntersectWith(bKeys);
    }
    
    [Benchmark]
    public void IntSet_IntersectWith_IntSet()
    {
        IntSetPaged.IntersectWith(IntSetPagedB);
    }

    [Benchmark(Baseline = true)]
    public void HashSet_IntersectWith()
    {
        hashSet.IntersectWith(bKeys);
    }
}