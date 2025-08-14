using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace IntSet.Benchmarks;

[MarkdownExporterAttribute.GitHub]
[SimpleJob(RuntimeMoniker.Net80, launchCount: 1, warmupCount: 3, iterationCount: 100)]
[MemoryDiagnoser(false)]
public class BitmapBenchmarks
{
    [Params(100, 1000, 10000, 100000)]
    public int N = 10000;

    protected int[] aKeys, bKeys, lookupKeys, clusteredKeys;
    
    private HashSet<int> hashSet;
    private Bitmap bitmap, bitmapB;
    private ClusteredBitmap clusteredBitmap;
    private NativeClusteredBitmap nativeClusteredBitmap; // added

    [IterationCleanup]
    public void Cleanup()
    {
        nativeClusteredBitmap?.Dispose();
        nativeClusteredBitmap = null;
    }
    
    [IterationSetup]
    public void Setup()
    {
        aKeys = new int[N];
        bKeys = new int[N];
        lookupKeys = new int[N];
        clusteredKeys = new int[N];
        var rng = new Random(123);
        for (var i = 0; i < N; i++)
        {
            aKeys[i] = rng.Next(0, N);
            bKeys[i] = rng.Next(0, N);
            lookupKeys[i] = rng.Next(0, N);
            clusteredKeys[i] = 10000000 + rng.Next(0, N);
        }

        hashSet = new HashSet<int>(aKeys);
        bitmap = new Bitmap(aKeys);
        bitmapB = new Bitmap(bKeys);
        clusteredBitmap = new ClusteredBitmap(aKeys);
        nativeClusteredBitmap = new NativeClusteredBitmap(aKeys); // added
    }

    [Benchmark]
    public Bitmap MemoryUsed_Bitmap()
    {
        return new Bitmap(aKeys);
    }
    
    [Benchmark]
    public ClusteredBitmap MemoryUsed_ClusteredBitmap()
    {
        return new ClusteredBitmap(aKeys);
    }
    
    [Benchmark]
    public HashSet<int> MemoryUsed_HashSet()
    {
        return new HashSet<int>(aKeys);
    }
    
    [Benchmark]
    public Bitmap MemoryUsed_ClusteredKeys_Bitmap()
    {
        return new Bitmap(clusteredKeys);
    }
    
    [Benchmark]
    public ClusteredBitmap MemoryUsed_ClusteredKeys_ClusteredBitmap()
    {
        return new ClusteredBitmap(clusteredKeys);
    }
    
    [Benchmark]
    public HashSet<int> MemoryUsed_ClusteredKeys_HashSet()
    {
        return new HashSet<int>(clusteredKeys);
    }
    
    [Benchmark]
    public NativeClusteredBitmap MemoryUsed_NativeClusteredBitmap() => new NativeClusteredBitmap(aKeys); // added
    [Benchmark]
    public NativeClusteredBitmap MemoryUsed_ClusteredKeys_NativeClusteredBitmap() => new NativeClusteredBitmap(clusteredKeys); // added
    
    [Benchmark]
    public void Contains_Bitmap()
    {
        for (var i = 0; i < N; i++)
            bitmap.IsSet(lookupKeys[i]);
    }
    
    [Benchmark]
    public void Contains_ClusteredBitmap()
    {
        for (var i = 0; i < N; i++)
            clusteredBitmap.IsSet(lookupKeys[i]);
    }
    
    [Benchmark()]
    public void Contains_HashSet()
    {
        for (var i = 0; i < N; i++)
            hashSet.Contains(lookupKeys[i]);
    }
    
    [Benchmark]
    public void Contains_NativeClusteredBitmap() // added
    {
        for (var i = 0; i < N; i++)
            nativeClusteredBitmap.IsSet(lookupKeys[i]);
    }
    
    [Benchmark]
    public void Add_Bitmap()
    {
        for (var i = 0; i < N; i++)
            bitmap.Set(lookupKeys[i]);
    }
    
    [Benchmark]
    public void Add_ClusteredBitmap()
    {
        for (var i = 0; i < N; i++)
            clusteredBitmap.Set(lookupKeys[i]);
    }
    
    [Benchmark()]
    public void Add_HashSet()
    {
        for (var i = 0; i < N; i++)
            hashSet.Add(lookupKeys[i]);
    }
    
    [Benchmark]
    public void Add_NativeClusteredBitmap() // added
    {
        for (var i = 0; i < N; i++)
            nativeClusteredBitmap.Set(lookupKeys[i]);
    }
    
    [Benchmark]
    public void Iterate_Bitmap()
    {
        var x = 0;
        foreach (var i in bitmap)
            x += i;
    }
    
    [Benchmark]
    public void Iterate_ClusteredBitmap()
    {
        var x = 0;
        foreach (var i in clusteredBitmap)
            x += i;
    }
    
    [Benchmark()]
    public void Iterate_HashSet()
    {
        var x = 0;
        foreach (var i in hashSet)
            x += i;
    }
    
    [Benchmark]
    public void Iterate_NativeClusteredBitmap() // added
    {
        var x = 0;
        foreach (var i in nativeClusteredBitmap)
            x += i;
    }
    
    [Benchmark]
    public void Remove_Bitmap()
    {
        for (var i = 0; i < N; i++)
            bitmap.UnSet(lookupKeys[i]);
    }
    
    [Benchmark]
    public void Remove_ClusteredBitmap()
    {
        for (var i = 0; i < N; i++)
            clusteredBitmap.UnSet(lookupKeys[i]);
    }
    
    [Benchmark()]
    public void Remove_HashSet()
    {
        for (var i = 0; i < N; i++)
            hashSet.Remove(lookupKeys[i]);
    }
    
    [Benchmark]
    public void Remove_NativeClusteredBitmap() // added
    {
        for (var i = 0; i < N; i++)
            nativeClusteredBitmap.UnSet(lookupKeys[i]);
    }
    
    [Benchmark]
    public void ExceptWith_Span_Bitmap()
    {
        bitmap.Not(bKeys);
    }
    
    [Benchmark]
    public void ExceptWith_Span_ClusteredBitmap()
    {
        clusteredBitmap.Not(bKeys);
    }
    
    [Benchmark]
    public void ExceptWith_Bitmap_Bitmap()
    {
        bitmap.Not(bitmapB);
    }
    
    [Benchmark()]
    public void ExceptWith_HashSet()
    {
        hashSet.ExceptWith(bKeys);
    }
    
    [Benchmark]
    public void ExceptWith_Span_NativeClusteredBitmap() // added
    {
        nativeClusteredBitmap.Not(bKeys);
    }
    
    [Benchmark]
    public void IntersectWith_Span_Bitmap()
    {
        bitmap.And(bKeys);
    }
    
    [Benchmark]
    public void IntersectWith_Span_ClusteredBitmap()
    {
        clusteredBitmap.And(bKeys);
    }
    
    [Benchmark]
    public void IntersectWith_Bitmap_Bitmap()
    {
        bitmap.And(bitmapB);
    }
    
    [Benchmark()]
    public void IntersectWith_HashSet()
    {
        hashSet.IntersectWith(bKeys);
    }
    
    [Benchmark]
    public void IntersectWith_Span_NativeClusteredBitmap() // added
    {
        nativeClusteredBitmap.And(bKeys);
    }
    
    [Benchmark]
    public void SymmetricExceptWith_Span_Bitmap()
    {
        bitmap.Xor(bKeys);
    }
    
    [Benchmark]
    public void SymmetricExceptWith_Span_ClusteredBitmap()
    {
        clusteredBitmap.Xor(bKeys);
    }
    
    [Benchmark]
    public void SymmetricExceptWith_Bitmap_Bitmap()
    {
        bitmap.Xor(bitmapB);
    }
    
    [Benchmark()]
    public void SymmetricExceptWith_HashSet()
    {
        hashSet.SymmetricExceptWith(bKeys);
    }
    
    [Benchmark]
    public void SymmetricExceptWith_Span_NativeClusteredBitmap() // added
    {
        nativeClusteredBitmap.Xor(bKeys);
    }
    
    [Benchmark]
    public void UnionWith_Span_Bitmap()
    {
        bitmap.Or(bKeys);
    }
    
    [Benchmark]
    public void UnionWith_Span_ClusteredBitmap()
    {
        clusteredBitmap.Or(bKeys);
    }
    
    [Benchmark]
    public void UnionWith_Bitmap_Bitmap()
    {
        bitmap.Or(bitmapB);
    }
    
    [Benchmark()]
    public void UnionWith_HashSet()
    {
        hashSet.UnionWith(bKeys);
    }
    
    [Benchmark]
    public void UnionWith_Span_NativeClusteredBitmap() // added
    {
        nativeClusteredBitmap.Or(bKeys);
    }
    
}