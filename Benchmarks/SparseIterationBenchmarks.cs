using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using IntegrityTables;

namespace IntSet.Benchmarks;

[MarkdownExporterAttribute.GitHub]
[SimpleJob(RuntimeMoniker.Net80, launchCount: 1, warmupCount: 3, iterationCount: 100)]
[MemoryDiagnoser(false)]
public class SparseIterationBenchmarks
{
    public int N = 100000;

    private int[] aKeys, bKeys;

    private IntSet intSet;
    HashSet<int> hashSet;
    
    private IntSet sparseIntSet;
    HashSet<int> sparseHashSet;
    
    [IterationSetup]
    public void Setup()
    {
        aKeys = new int[N];

        var rng = new Random(123);
        
        for (var i = 0; i < N; i++)
        {
            aKeys[i] = rng.Next(0, N);
        }

        intSet = new IntSet(aKeys);
        hashSet = new HashSet<int>(aKeys);
        
        bKeys = new int[N];
        // generate a sparse array of values for testing iteration
        for (var i = 0; i < N; i++)
        {
            bKeys[i] = rng.Next(-N*10, +N*10);
        }
        sparseIntSet = new IntSet(bKeys);
        sparseHashSet = new HashSet<int>(bKeys);
    }
    
    
    
    [Benchmark]
    public void IntSet_SparseValues_Iterate()
    {
        var x = 0;
        foreach (var i in sparseIntSet)
            x += i;
    }
    
    [Benchmark(Baseline = true)]
    public void HashSet_SparseValues_Iterate()
    {
        var x = 0;
        foreach (var i in sparseHashSet)
            x += i;
    }
    
}
