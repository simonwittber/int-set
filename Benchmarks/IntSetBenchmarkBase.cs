using BenchmarkDotNet.Attributes;

namespace IntSet.Benchmarks;

public abstract class IntSetBenchmarkBase
{
    public int N = 10000;

    protected int[] aKeys, bKeys, lookupKeys;
    protected IntSet intSet, intSetB;
    protected HashSet<int> hashSet;
    protected DenseIdMap DenseIdMap;
    
    [IterationSetup]
    public void Setup()
    {
        aKeys = new int[N];
        bKeys = new int[N];
        lookupKeys = new int[N];
        var rng = new Random(123);
        for (var i = 0; i < N; i++)
        {
            aKeys[i] = rng.Next(0, N);
            bKeys[i] = rng.Next(0, N);
            lookupKeys[i] = rng.Next(0, N);
        }
        intSet = new IntSet(aKeys);
        intSetB = new IntSet(bKeys);
        hashSet = new HashSet<int>(aKeys);
        DenseIdMap = new DenseIdMap(aKeys);
    }
}