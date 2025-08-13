using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using IntegrityTables;

namespace IntSet.Benchmarks;

[MarkdownExporterAttribute.GitHub]
[SimpleJob(RuntimeMoniker.Net80, launchCount: 1, warmupCount: 3, iterationCount: 100)]
[MemoryDiagnoser(false)]
public class DenseIterationBenchmarks
{
    public int N = 100000;

    private int[] aKeys, bKeys;

    private IntSetPaged _intSetPaged;
    HashSet<int> hashSet;
    
    private IntSetPaged _sparseIntSetPaged;
    HashSet<int> sparseHashSet;
    DenseIdMap _denseIdMap;
    
    [IterationSetup]
    public void Setup()
    {
        aKeys = new int[N];

        var rng = new Random(123);
        
        for (var i = 0; i < N; i++)
        {
            aKeys[i] = rng.Next(0, N);
        }

        _intSetPaged = new IntSetPaged(aKeys);
        hashSet = new HashSet<int>(aKeys);
        _denseIdMap = new DenseIdMap(aKeys);
        bKeys = new int[N];
        // generate a sparse array of values for testing iteration
        for (var i = 0; i < N; i++)
        {
            bKeys[i] = rng.Next(-N*10, +N*10);
        }
        _sparseIntSetPaged = new IntSetPaged(bKeys);
        sparseHashSet = new HashSet<int>(bKeys);
    }
    
    [Benchmark]
    public void IntSet_DenseValues_Iterate()
    {
        var x = 0;
        foreach (var i in _intSetPaged)
            x += i;
    }
    
    [Benchmark]
    public void DenseIdMapDenseValues_Iterate()
    {
        var x = 0;
        foreach (var i in _denseIdMap)
            x += i;
    }
    
    [Benchmark(Baseline = true)]
    public void HashSet_DenseValues_Iterate()
    {
        var x = 0;
        foreach (var i in hashSet)
            x += i;
    }
    
    
    
}
