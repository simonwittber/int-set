using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;

namespace IntSet.Benchmarks;

[MarkdownExporterAttribute.GitHub]
[SimpleJob(RuntimeMoniker.Net80, launchCount: 1, warmupCount: 3, iterationCount: 100)]
[MemoryDiagnoser(false)]
[GroupBenchmarksBy(
    BenchmarkLogicalGroupRule.ByCategory,
    BenchmarkLogicalGroupRule.ByParams,
    BenchmarkLogicalGroupRule.ByJob)]
public class BitmapBenchmarks
{
    [Params(10000)] public int KeyCount = 10000;

    protected int[] aKeys, bKeys, lookupKeys, clusteredKeys;

    private HashSet<int> hashSet, hashSetB;
    private Bitmap bitmap, bitmapB;
    private ClusteredBitmap clusteredBitmap;
    private NativeClusteredBitmap nativeClusteredBitmap;

    [IterationCleanup]
    public void Cleanup()
    {
        nativeClusteredBitmap?.Dispose();
        nativeClusteredBitmap = null;
    }

    [IterationSetup]
    public void Setup()
    {
        aKeys = new int[KeyCount];
        bKeys = new int[KeyCount];
        lookupKeys = new int[KeyCount];
        clusteredKeys = new int[KeyCount];
        var rng = new Random(123);
        for (var i = 0; i < KeyCount; i++)
        {
            aKeys[i] = rng.Next(0, KeyCount);
            bKeys[i] = rng.Next(0, KeyCount);
            lookupKeys[i] = rng.Next(0, KeyCount);
            clusteredKeys[i] = 10000000 + rng.Next(0, KeyCount);
        }

        hashSet = new HashSet<int>(aKeys);
        hashSetB = new HashSet<int>(bKeys);
        bitmap = new Bitmap(aKeys);
        bitmapB = new Bitmap(bKeys);
        clusteredBitmap = new ClusteredBitmap(aKeys);
        nativeClusteredBitmap = new NativeClusteredBitmap(aKeys);
    }

    [BenchmarkCategory("Memory"), Benchmark(OperationsPerInvoke = 1, Baseline = true)]
    public HashSet<int> Allocate_HashSet_with_RandomKeys()
    {
        return new HashSet<int>(aKeys);
    }

    [BenchmarkCategory("Memory"), Benchmark(OperationsPerInvoke = 1)]
    public Bitmap Allocate_Bitmap_with_RandomKeys()
    {
        return new Bitmap(aKeys);
    }

    [BenchmarkCategory("Memory"), Benchmark(OperationsPerInvoke = 1)]
    public ClusteredBitmap Allocate_ClusteredBitmap_with_RandomKeys()
    {
        return new ClusteredBitmap(aKeys);
    }

    [BenchmarkCategory("Memory"), Benchmark(OperationsPerInvoke = 1)]
    public NativeClusteredBitmap Allocate_NativeClusteredBitmap_with_RandomKeys()
    {
        return new NativeClusteredBitmap(aKeys);
    }

    [BenchmarkCategory("Memory"), Benchmark(OperationsPerInvoke = 1)]
    public HashSet<int> Allocate_HashSet_with_ClusteredKeys()
    {
        return new HashSet<int>(clusteredKeys);
    }

    [BenchmarkCategory("Memory"), Benchmark(OperationsPerInvoke = 1)]
    public Bitmap Allocate_Bitmap_with_ClusteredKeys()
    {
        return new Bitmap(clusteredKeys);
    }

    [BenchmarkCategory("Memory"), Benchmark(OperationsPerInvoke = 1)]
    public ClusteredBitmap Allocate_ClusteredBitmap_with_ClusteredKeys()
    {
        return new ClusteredBitmap(clusteredKeys);
    }

    [BenchmarkCategory("Memory"), Benchmark(OperationsPerInvoke = 1)]
    public NativeClusteredBitmap Allocate_NativeClusteredBitmap_with_ClusteredKeys()
    {
        return new NativeClusteredBitmap(clusteredKeys);
    }

    [BenchmarkCategory("Contains"), Benchmark(OperationsPerInvoke = 10000, Baseline = true)]
    public void Contains_HashSet()
    {
        for (var i = 0; i < KeyCount; i++)
            hashSet.Contains(lookupKeys[i]);
    }

    [BenchmarkCategory("Contains"), Benchmark(OperationsPerInvoke = 10000)]
    public void Contains_Bitmap()
    {
        for (var i = 0; i < KeyCount; i++)
            bitmap.IsSet(lookupKeys[i]);
    }

    [BenchmarkCategory("Contains"), Benchmark(OperationsPerInvoke = 10000)]
    public void Contains_ClusteredBitmap()
    {
        for (var i = 0; i < KeyCount; i++)
            clusteredBitmap.IsSet(lookupKeys[i]);
    }

    [BenchmarkCategory("Contains"), Benchmark(OperationsPerInvoke = 10000)]
    public void Contains_NativeClusteredBitmap()
    {
        for (var i = 0; i < KeyCount; i++)
            nativeClusteredBitmap.IsSet(lookupKeys[i]);
    }

    [BenchmarkCategory("Add"), Benchmark(OperationsPerInvoke = 10000, Baseline = true)]
    public void Add_HashSet()
    {
        for (var i = 0; i < KeyCount; i++)
            hashSet.Add(lookupKeys[i]);
    }

    [BenchmarkCategory("Add"), Benchmark(OperationsPerInvoke = 10000)]
    public void Add_Bitmap()
    {
        for (var i = 0; i < KeyCount; i++)
            bitmap.Set(lookupKeys[i]);
    }

    [BenchmarkCategory("Add"), Benchmark(OperationsPerInvoke = 10000)]
    public void Add_ClusteredBitmap()
    {
        for (var i = 0; i < KeyCount; i++)
            clusteredBitmap.Set(lookupKeys[i]);
    }


    [BenchmarkCategory("Add"), Benchmark(OperationsPerInvoke = 10000)]
    public void Add_NativeClusteredBitmap()
    {
        for (var i = 0; i < KeyCount; i++)
            nativeClusteredBitmap.Set(lookupKeys[i]);
    }

    [BenchmarkCategory("Iterate"), Benchmark(OperationsPerInvoke = 1, Baseline = true)]
    public void Iterate_HashSet()
    {
        var x = 0;
        foreach (var i in hashSet)
            x += i;
    }

    [BenchmarkCategory("Iterate"), Benchmark(OperationsPerInvoke = 1)]
    public void Iterate_Bitmap()
    {
        var x = 0;
        foreach (var i in bitmap)
            x += i;
    }

    [BenchmarkCategory("Iterate"), Benchmark(OperationsPerInvoke = 1)]
    public void Iterate_ClusteredBitmap()
    {
        var x = 0;
        foreach (var i in clusteredBitmap)
            x += i;
    }

    [BenchmarkCategory("Iterate"), Benchmark(OperationsPerInvoke = 1)]
    public void Iterate_NativeClusteredBitmap()
    {
        var x = 0;
        foreach (var i in nativeClusteredBitmap)
            x += i;
    }

    [BenchmarkCategory("Remove"), Benchmark(OperationsPerInvoke = 10000, Baseline = true)]
    public void Remove_HashSet()
    {
        for (var i = 0; i < KeyCount; i++)
            hashSet.Remove(lookupKeys[i]);
    }

    [BenchmarkCategory("Remove"), Benchmark(OperationsPerInvoke = 10000)]
    public void Remove_Bitmap()
    {
        for (var i = 0; i < KeyCount; i++)
            bitmap.UnSet(lookupKeys[i]);
    }

    [BenchmarkCategory("Remove"), Benchmark(OperationsPerInvoke = 10000)]
    public void Remove_ClusteredBitmap()
    {
        for (var i = 0; i < KeyCount; i++)
            clusteredBitmap.UnSet(lookupKeys[i]);
    }


    [BenchmarkCategory("Remove"), Benchmark(OperationsPerInvoke = 10000)]
    public void Remove_NativeClusteredBitmap()
    {
        for (var i = 0; i < KeyCount; i++)
            nativeClusteredBitmap.UnSet(lookupKeys[i]);
    }

    [BenchmarkCategory("ExceptWith"), Benchmark(OperationsPerInvoke = 1, Baseline = true)]
    public void ExceptWith_HashSet_to_Span()
    {
        hashSet.ExceptWith(bKeys);
    }

    [BenchmarkCategory("ExceptWith"), Benchmark(OperationsPerInvoke = 1)]
    public void ExceptWith_Bitmap_to_Span()
    {
        bitmap.Not(bKeys);
    }

    [BenchmarkCategory("ExceptWith"), Benchmark(OperationsPerInvoke = 1)]
    public void ExceptWith_ClusteredBitmap_to_Span()
    {
        clusteredBitmap.Not(bKeys);
    }

    [BenchmarkCategory("ExceptWith"), Benchmark(OperationsPerInvoke = 1)]
    public void ExceptWith_NativeClusteredBitmap_to_Span()
    {
        nativeClusteredBitmap.Not(bKeys);
    }

    [BenchmarkCategory("ExceptWith"), Benchmark(OperationsPerInvoke = 1)]
    public void ExceptWith_HashSet_to_HashSet()
    {
        hashSet.ExceptWith(hashSetB);
    }

    [BenchmarkCategory("ExceptWith"), Benchmark(OperationsPerInvoke = 1)]
    public void ExceptWith_Bitmap_to_Bitmap()
    {
        bitmap.Not(bitmapB);
    }

    [BenchmarkCategory("InterSectWith"), Benchmark(OperationsPerInvoke = 1, Baseline = true)]
    public void IntersectWith_HashSet_to_Span()
    {
        hashSet.IntersectWith(bKeys);
    }

    [BenchmarkCategory("InterSectWith"), Benchmark(OperationsPerInvoke = 1)]
    public void IntersectWith_Bitmap_to_Span()
    {
        bitmap.And(bKeys);
    }

    [BenchmarkCategory("InterSectWith"), Benchmark(OperationsPerInvoke = 1)]
    public void IntersectWith_ClusteredBitmap_to_Span()
    {
        clusteredBitmap.And(bKeys);
    }

    [BenchmarkCategory("InterSectWith"), Benchmark(OperationsPerInvoke = 1)]
    public void IntersectWith_NativeClusteredBitmap_to_Span()
    {
        nativeClusteredBitmap.And(bKeys);
    }

    [BenchmarkCategory("InterSectWith"), Benchmark(OperationsPerInvoke = 1)]
    public void IntersectWith_HashSet_to_HashSet()
    {
        hashSet.IntersectWith(hashSetB);
    }

    [BenchmarkCategory("InterSectWith"), Benchmark(OperationsPerInvoke = 1)]
    public void IntersectWith_Bitmap_to_Bitmap()
    {
        bitmap.And(bitmapB);
    }


    [BenchmarkCategory("SymmetricExceptWith"), Benchmark(OperationsPerInvoke = 1, Baseline = true)]
    public void SymmetricExceptWith_HashSet_to_Span()
    {
        hashSet.SymmetricExceptWith(bKeys);
    }

    [BenchmarkCategory("SymmetricExceptWith"), Benchmark(OperationsPerInvoke = 1)]
    public void SymmetricExceptWith_Bitmap_to_Span()
    {
        bitmap.Xor(bKeys);
    }

    [BenchmarkCategory("SymmetricExceptWith"), Benchmark(OperationsPerInvoke = 1)]
    public void SymmetricExceptWith_ClusteredBitmap_to_Span()
    {
        clusteredBitmap.Xor(bKeys);
    }

    [BenchmarkCategory("SymmetricExceptWith"), Benchmark(OperationsPerInvoke = 1)]
    public void SymmetricExceptWith_NativeClusteredBitmap_to_Span()
    {
        nativeClusteredBitmap.Xor(bKeys);
    }

    [BenchmarkCategory("SymmetricExceptWith"), Benchmark(OperationsPerInvoke = 1)]
    public void SymmetricExceptWith_HashSet_to_HashSet()
    {
        hashSet.SymmetricExceptWith(hashSetB);
    }

    [BenchmarkCategory("SymmetricExceptWith"), Benchmark(OperationsPerInvoke = 1)]
    public void SymmetricExceptWith_Bitmap_to_Bitmap()
    {
        bitmap.Xor(bitmapB);
    }


    [BenchmarkCategory("UnionWith"), Benchmark(OperationsPerInvoke = 1, Baseline = true)]
    public void UnionWith_HashSet_to_Span()
    {
        hashSet.UnionWith(bKeys);
    }

    [BenchmarkCategory("UnionWith"), Benchmark(OperationsPerInvoke = 1)]
    public void UnionWith_Bitmap_to_Span()
    {
        bitmap.Or(bKeys);
    }

    [BenchmarkCategory("UnionWith"), Benchmark(OperationsPerInvoke = 1)]
    public void UnionWith_ClusteredBitmap_to_Span()
    {
        clusteredBitmap.Or(bKeys);
    }

    [BenchmarkCategory("UnionWith"), Benchmark(OperationsPerInvoke = 1)]
    public void UnionWith_NativeClusteredBitmap_to_Span()
    {
        nativeClusteredBitmap.Or(bKeys);
    }

    [BenchmarkCategory("UnionWith"), Benchmark(OperationsPerInvoke = 1)]
    public void UnionWith_HashSet_to_HashSet()
    {
        hashSet.UnionWith(hashSetB);
    }

    [BenchmarkCategory("UnionWith"), Benchmark(OperationsPerInvoke = 1)]
    public void UnionWith_Bitmap_to_Bitmap()
    {
        bitmap.Or(bitmapB);
    }
}