using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace IntSet.Benchmarks;

[MarkdownExporterAttribute.GitHub]
[SimpleJob(RuntimeMoniker.Net80, launchCount: 1, warmupCount: 3, iterationCount: 100)]
[MemoryDiagnoser(false)]
public class BitmapBenchmarks
{
    [Params(100, 1000, 10000, 100000,1000000)]
    public int N = 10000;

    protected int[] aKeys, bKeys, lookupKeys;
    
    private HashSet<int> hashSet;
    private Bitmap bitmap, bitmapB;
    private ClusteredBitmap clusteredBitmap;
    
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

        hashSet = new HashSet<int>(aKeys);
        bitmap = new Bitmap(aKeys);
        bitmapB = new Bitmap(bKeys);
        clusteredBitmap = new ClusteredBitmap(aKeys);
    }

    [Benchmark]
    public void Bitmap_Contains()
    {
        for (var i = 0; i < N; i++)
            bitmap.IsSet(lookupKeys[i]);
    }
    
    [Benchmark]
    public void ClusteredBitmap_Contains()
    {
        for (var i = 0; i < N; i++)
            clusteredBitmap.IsSet(lookupKeys[i]);
    }
    
    [Benchmark()]
    public void HashSet_Contains()
    {
        for (var i = 0; i < N; i++)
            hashSet.Contains(lookupKeys[i]);
    }
    
    [Benchmark]
    public void Bitmap_Add()
    {
        for (var i = 0; i < N; i++)
            bitmap.Set(lookupKeys[i]);
    }
    
    [Benchmark]
    public void ClusteredBitmap_Add()
    {
        for (var i = 0; i < N; i++)
            clusteredBitmap.Set(lookupKeys[i]);
    }
    
    [Benchmark()]
    public void HashSet_Add()
    {
        for (var i = 0; i < N; i++)
            hashSet.Add(lookupKeys[i]);
    }
    
    [Benchmark]
    public void Bitmap_Iterate()
    {
        var x = 0;
        foreach (var i in bitmap)
            x += i;
    }
    
    [Benchmark]
    public void ClusteredBitmap_Iterate()
    {
        var x = 0;
        foreach (var i in clusteredBitmap)
            x += i;
    }

    [Benchmark()]
    public void HashSet_Iterate()
    {
        var x = 0;
        foreach (var i in hashSet)
            x += i;
    }
    
    [Benchmark]
    public void Bitmap_Remove()
    {
        for (var i = 0; i < N; i++)
            bitmap.UnSet(lookupKeys[i]);
    }
    
    [Benchmark]
    public void ClusteredBitmap_Remove()
    {
        for (var i = 0; i < N; i++)
            clusteredBitmap.UnSet(lookupKeys[i]);
    }
    
    [Benchmark()]
    public void HashSet_Remove()
    {
        for (var i = 0; i < N; i++)
            hashSet.Remove(lookupKeys[i]);
    }
    
    [Benchmark]
    public void Bitmap_ExceptWith_Span()
    {
        bitmap.Not(bKeys);
    }
    
    [Benchmark]
    public void ClusteredBitmap_ExceptWith_Span()
    {
        clusteredBitmap.Not(bKeys);
    }
    
    [Benchmark]
    public void Bitmap_ExceptWith_Bitmap()
    {
        bitmap.Not(bitmapB);
    }
    
    [Benchmark()]
    public void HashSet_ExceptWith()
    {
        hashSet.ExceptWith(bKeys);
    }
    
    [Benchmark]
    public void Bitmap_IntersectWith_Span()
    {
        bitmap.And(bKeys);
    }
    
    [Benchmark]
    public void ClusteredBitmap_IntersectWith_Span()
    {
        clusteredBitmap.And(bKeys);
    }
    
    [Benchmark]
    public void Bitmap_IntersectWith_Bitmap()
    {
        bitmap.And(bitmapB);
    }
    
    [Benchmark()]
    public void HashSet_IntersectWith()
    {
        hashSet.IntersectWith(bKeys);
    }
    
    [Benchmark]
    public void Bitmap_SymmetricExceptWith_Span()
    {
        bitmap.Xor(bKeys);
    }
    [Benchmark]
    public void ClusteredBitmap_SymmetricExceptWith_Span()
    {
        clusteredBitmap.Xor(bKeys);
    }
    
    [Benchmark]
    public void Bitmap_SymmetricExceptWith_Bitmap()
    {
        bitmap.Xor(bitmapB);
    }

    [Benchmark()]
    public void HashSet_SymmetricExceptWith()
    {
        hashSet.SymmetricExceptWith(bKeys);
    }
    
    [Benchmark]
    public void Bitmap_UnionWith_Span()
    {
        bitmap.Or(bKeys);
    }
    
    [Benchmark]
    public void ClusteredBitmap_UnionWith_Span()
    {
        clusteredBitmap.Or(bKeys);
    }
    
    [Benchmark]
    public void Bitmap_UnionWith_Bitmap()
    {
        bitmap.Or(bitmapB);
    }

    [Benchmark()]
    public void HashSet_UnionWith()
    {
        hashSet.UnionWith(bKeys);
    }
    
}