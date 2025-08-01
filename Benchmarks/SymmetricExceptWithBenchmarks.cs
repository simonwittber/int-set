using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using IntegrityTables;

namespace IntSet.Benchmarks;

[MarkdownExporterAttribute.GitHub]
[SimpleJob(RuntimeMoniker.Net80, launchCount: 1, warmupCount: 3, iterationCount: 100)]
[MemoryDiagnoser(false)]
public class SymmetricExceptWithBenchmarks : IntSetBenchmarkBase
{
    
    [Benchmark]
    public void IntSet_SymmetricExceptWith()
    {
        intSet.SymmetricExceptWith(bKeys);
    }

    [Benchmark(Baseline = true)]
    public void HashSet_SymmetricExceptWith()
    {
        hashSet.SymmetricExceptWith(bKeys);
    }
}
