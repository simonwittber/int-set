using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace IntSet.Benchmarks;

[MarkdownExporterAttribute.GitHub]
[SimpleJob(RuntimeMoniker.Net80, launchCount: 1, warmupCount: 3, iterationCount: 1)]
[MemoryDiagnoser(false)]
public class MemoryUsageBenchmarks
{
    [Params(10, 100, 1000,10000, 100000, 1000000, 10000000)]
    public int N;

    protected int[] aKeys;
    
    [IterationSetup]
    public void Setup()
    {
        aKeys = new int[N];
        for (var i = 0; i < N; i++)
        {
            aKeys[i] = N;
        }
    }

    [Benchmark]
    public void IntSet_MemoryUsage()
    {
        var intSet = new IntSet(aKeys);
    }
    
    [Benchmark(Baseline = true)]
    public void HashSet_MemoryUsage()
    {
        var hashSet = new HashSet<int>(aKeys);
    }
    
    [Benchmark]
    public void IntSet_MemoryUsage_2KeysExtremeValues()
    {
        var intSet = new IntSet();
        intSet.Add(0);
        intSet.Add(10000000);
    }
    
    [Benchmark()]
    public void HashSet_MemoryUsage_2KeysExtremeValues()
    {
        var hashSet = new HashSet<int>();
        hashSet.Add(0);
        hashSet.Add(10000000);
    }
}