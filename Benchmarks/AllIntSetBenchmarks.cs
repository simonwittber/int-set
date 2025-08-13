using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace IntSet.Benchmarks;

[MarkdownExporterAttribute.GitHub]
[SimpleJob(RuntimeMoniker.Net80, launchCount: 1, warmupCount: 3, iterationCount: 100)]
[MemoryDiagnoser(false)]
public class AllIntSetBenchmarks : IntSetBenchmarkBase
{

    [Benchmark]
    public void PagedSet_Contains()
    {
        for (var i = 0; i < N; i++)
            IntSetPaged.Contains(lookupKeys[i]);
    }
    
    [Benchmark]
    public void ClusSet_Contains()
    {
        for (var i = 0; i < N; i++)
            clusSet.Contains(lookupKeys[i]);
    }

    [Benchmark()]
    public void HashSet_Contains()
    {
        for (var i = 0; i < N; i++)
            hashSet.Contains(lookupKeys[i]);
    }
    
    [Benchmark]
    public void PagedSet_Add()
    {
        for (var i = 0; i < N; i++)
            IntSetPaged.Add(lookupKeys[i]);
    }
    [Benchmark]
    public void ClusSet_Add()
    {
        for (var i = 0; i < N; i++)
            clusSet.Add(lookupKeys[i]);
    }
    [Benchmark()]
    public void HashSet_Add()
    {
        for (var i = 0; i < N; i++)
            hashSet.Add(lookupKeys[i]);
    }
    
    [Benchmark]
    public void PagedSet_DenseValues_Iterate()
    {
        var x = 0;
        foreach (var i in IntSetPaged)
            x += i;
    }
    [Benchmark]
    public void ClusSet_DenseValues_Iterate()
    {
        var x = 0;
        foreach (var i in clusSet)
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
    public void PagedSet_Remove()
    {
        for (var i = 0; i < N; i++)
            IntSetPaged.Remove(lookupKeys[i]);
    }
    
    [Benchmark]
    public void ClusSet_Remove()
    {
        for (var i = 0; i < N; i++)
            clusSet.Remove(lookupKeys[i]);
    }

    [Benchmark()]
    public void HashSet_Remove()
    {
        for (var i = 0; i < N; i++)
            hashSet.Remove(lookupKeys[i]);
    }
    
    [Benchmark]
    public void PagedSet_ExceptWith_Span()
    {
        IntSetPaged.ExceptWith(bKeys);
    }
    
    [Benchmark]
    public void PagedSet_ExceptWith_IntSet()
    {
        IntSetPaged.ExceptWith(IntSetPagedB);
    }
    
    [Benchmark]
    public void ClusSet_ExceptWith_Span()
    {
        clusSet.ExceptWith(bKeys);
    }
    
    [Benchmark]
    public void ClusSet_ExceptWith_IntSet()
    {
        clusSet.ExceptWith(clusSetB);
    }

    [Benchmark()]
    public void HashSet_ExceptWith()
    {
        hashSet.ExceptWith(bKeys);
    }
    
    [Benchmark]
    public void PagedSet_IntersectWith_Span()
    {
        IntSetPaged.IntersectWith(bKeys);
    }
    
    [Benchmark]
    public void PagedSet_IntersectWith_IntSet()
    {
        IntSetPaged.IntersectWith(IntSetPagedB);
    }
    
    [Benchmark]
    public void ClusSet_IntersectWith_Span()
    {
        clusSet.IntersectWith(bKeys);
    }
    
    [Benchmark]
    public void ClusSet_IntersectWith_IntSet()
    {
        clusSet.IntersectWith(clusSetB);
    }

    [Benchmark()]
    public void HashSet_IntersectWith()
    {
        hashSet.IntersectWith(bKeys);
    }
    
    [Benchmark]
    public void PagedSet_SymmetricExceptWith_Span()
    {
        IntSetPaged.SymmetricExceptWith(bKeys);
    }
    
    [Benchmark]
    public void PagedSet_SymmetricExceptWith_IntSet()
    {
        IntSetPaged.SymmetricExceptWith(IntSetPagedB);
    }
    
    [Benchmark]
    public void ClusSet_SymmetricExceptWith_Span()
    {
        clusSet.SymmetricExceptWith(bKeys);
    }
    
    [Benchmark]
    public void ClusSet_SymmetricExceptWith_IntSet()
    {
        clusSet.SymmetricExceptWith(clusSet);
    }

    [Benchmark()]
    public void HashSet_SymmetricExceptWith()
    {
        hashSet.SymmetricExceptWith(bKeys);
    }
    
    [Benchmark]
    public void PagedSet_UnionWith_Span()
    {
        IntSetPaged.UnionWith(bKeys);
    }
    
    [Benchmark]
    public void PagedSet_UnionWith_IntSet()
    {
        IntSetPaged.UnionWith(IntSetPagedB);
    }
    
    [Benchmark]
    public void ClusSet_UnionWith_Span()
    {
        clusSet.UnionWith(bKeys);
    }
    
    [Benchmark]
    public void ClusSet_UnionWith_IntSet()
    {
        clusSet.UnionWith(clusSetB);
    }

    [Benchmark()]
    public void HashSet_UnionWith()
    {
        hashSet.UnionWith(bKeys);
    }
    
}