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
            aKeys[i] = i;
        }
    }

    [Benchmark]
    public IntSetPaged IntSet_MemoryUsage()
    {
        var intSet = new IntSetPaged(aKeys);
        return intSet;
    }
    
    [Benchmark]
    public IntSetClustered ClusSet_MemoryUsage()
    {
        var vm = new IntSetClustered(aKeys);
        return vm;
    }
    
    [Benchmark(Baseline = true)]
    public HashSet<int> HashSet_MemoryUsage()
    {
        var hashSet = new HashSet<int>(aKeys);
        return hashSet;
    }
    
    [Benchmark]
    public void IntSet_MemoryUsage_2KeysExtremeValues()
    {
        var intSet = new IntSetPaged();
        intSet.Add(0);
        intSet.Add(int.MaxValue);
    }
    
    [Benchmark]
    public void ClusIntSet_MemoryUsage_2KeysExtremeValues()
    {
        var intSet = new IntSetClustered();
        intSet.Add(0);
        intSet.Add(int.MaxValue);
    }
    
    [Benchmark()]
    public void HashSet_MemoryUsage_2KeysExtremeValues()
    {
        var hashSet = new HashSet<int>();
        hashSet.Add(0);
        hashSet.Add(int.MaxValue);
    }
}