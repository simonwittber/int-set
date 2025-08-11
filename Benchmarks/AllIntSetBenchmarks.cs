using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace IntSet.Benchmarks;

[MarkdownExporterAttribute.GitHub]
[SimpleJob(RuntimeMoniker.Net80, launchCount: 1, warmupCount: 3, iterationCount: 100)]
[MemoryDiagnoser(false)]
public class AllIntSetBenchmarks : IntSetBenchmarkBase
{
    [Benchmark]
    public void IntSet_Contains()
    {
        for (var i = 0; i < N; i++)
            intSet.Contains(lookupKeys[i]);
    }
    
    [Benchmark]
    public void DenseIdMapContains()
    {
        for (var i = 0; i < N; i++)
            DenseIdMap.Contains(lookupKeys[i]);
    }

    [Benchmark()]
    public void HashSet_Contains()
    {
        for (var i = 0; i < N; i++)
            hashSet.Contains(lookupKeys[i]);
    }
    
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

    [Benchmark()]
    public void HashSet_Add()
    {
        for (var i = 0; i < N; i++)
            hashSet.Add(lookupKeys[i]);
    }
    
    [Benchmark]
    public void IntSet_DenseValues_Iterate()
    {
        var x = 0;
        foreach (var i in intSet)
            x += i;
    }
    
    [Benchmark]
    public void DenseIdMapDenseValues_Iterate()
    {
        var x = 0;
        foreach (var i in DenseIdMap)
            x += i;
    }
    
    [Benchmark()]
    public void HashSet_DenseValues_Iterate()
    {
        var x = 0;
        foreach (var i in hashSet)
            x += i;
    }
    
    [Benchmark]
    public IntSet IntSet_MemoryUsage()
    {
        var intSet = new IntSet(aKeys);
        return intSet;
    }
    
    [Benchmark]
    public IntSet DenseIdMapMemoryUsage()
    {
        var vm = new IntSet(aKeys);
        return vm;
    }
    
    [Benchmark()]
    public HashSet<int> HashSet_MemoryUsage()
    {
        var hashSet = new HashSet<int>(aKeys);
        return hashSet;
    }
    
    [Benchmark]
    public void IntSet_MemoryUsage_2KeysExtremeValues()
    {
        var intSet = new IntSet();
        intSet.Add(0);
        intSet.Add(10000000);
    }
    
    [Benchmark()]
    public void DenseIdMapMemoryUsage_2KeysExtremeValues()
    {
        var densemap = new DenseIdMap();
        densemap.GetOrAdd(0);
        densemap.GetOrAdd(10000000);
    }
    
    [Benchmark()]
    public void HashSet_MemoryUsage_2KeysExtremeValues()
    {
        var hashSet = new HashSet<int>();
        hashSet.Add(0);
        hashSet.Add(10000000);
    }
    
    [Benchmark]
    public void IntSet_Remove()
    {
        for (var i = 0; i < N; i++)
            intSet.Remove(lookupKeys[i]);
    }
    
    [Benchmark]
    public void DenseIdMapRemove()
    {
        for (var i = 0; i < N; i++)
            DenseIdMap.Remove(lookupKeys[i]);
    }

    [Benchmark()]
    public void HashSet_Remove()
    {
        for (var i = 0; i < N; i++)
            hashSet.Remove(lookupKeys[i]);
    }
    
    [Benchmark]
    public void IntSet_ExceptWith_Span()
    {
        intSet.ExceptWith(bKeys);
    }
    
    [Benchmark]
    public void IntSet_ExceptWith_IntSet()
    {
        intSet.ExceptWith(intSetB);
    }

    [Benchmark()]
    public void HashSet_ExceptWith()
    {
        hashSet.ExceptWith(bKeys);
    }
    
    [Benchmark]
    public void IntSet_IntersectWith_Span()
    {
        intSet.IntersectWith(bKeys);
    }
    
    [Benchmark]
    public void IntSet_IntersectWith_IntSet()
    {
        intSet.IntersectWith(intSetB);
    }

    [Benchmark()]
    public void HashSet_IntersectWith()
    {
        hashSet.IntersectWith(bKeys);
    }
    
    [Benchmark]
    public void IntSet_SymmetricExceptWith_Span()
    {
        intSet.SymmetricExceptWith(bKeys);
    }
    
    [Benchmark]
    public void IntSet_SymmetricExceptWith_IntSet()
    {
        intSet.SymmetricExceptWith(intSetB);
    }

    [Benchmark()]
    public void HashSet_SymmetricExceptWith()
    {
        hashSet.SymmetricExceptWith(bKeys);
    }
    
    [Benchmark]
    public void IntSet_UnionWith_Span()
    {
        intSet.UnionWith(bKeys);
    }
    
    [Benchmark]
    public void IntSet_UnionWith_IntSet()
    {
        intSet.UnionWith(intSetB);
    }

    [Benchmark()]
    public void HashSet_UnionWith()
    {
        hashSet.UnionWith(bKeys);
    }
    
}