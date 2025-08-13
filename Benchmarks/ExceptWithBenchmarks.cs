using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace IntSet.Benchmarks;

[MarkdownExporterAttribute.GitHub]
[SimpleJob(RuntimeMoniker.Net80, launchCount: 1, warmupCount: 3, iterationCount: 100)]
[MemoryDiagnoser(false)]
public class ExceptWithBenchmarks : IntSetBenchmarkBase
{
    [Benchmark]
    public void IntSet_ExceptWith()
    {
        IntSetPaged.ExceptWith(bKeys);
    }

    [Benchmark(Baseline = true)]
    public void HashSet_ExceptWith()
    {
        hashSet.ExceptWith(bKeys);
    }
}