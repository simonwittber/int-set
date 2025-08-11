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

    [Benchmark()]
    public void HashSet_DenseValues_Iterate()
    {
        var x = 0;
        foreach (var i in hashSet)
            x += i;
    }
    
    [Benchmark]
    public void IntSet_Remove()
    {
        for (var i = 0; i < N; i++)
            intSet.Remove(lookupKeys[i]);
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