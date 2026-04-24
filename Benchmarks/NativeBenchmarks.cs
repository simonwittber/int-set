using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace IntSet.Benchmarks;

[MarkdownExporterAttribute.GitHub]
[SimpleJob(RuntimeMoniker.Net80, launchCount: 1, warmupCount: 3, iterationCount: 10)]
[MemoryDiagnoser(false)]
public class NativeSetBenchmarks
{
    [Params(100, 10000, 1000000)] public int KeyCount = 10000;

    private int[] aKeys, bKeys, lookupKeys;
    private UIntSet _uintSet, _uintSetB;
    private IntSet _intSet, _intSetB;
    private NativeUIntSet _nativeUIntSet, _nativeUIntSetB;
    private NativeIntSet _nativeIntSet, _nativeIntSetB;

    [IterationSetup]
    public void Setup()
    {
        aKeys = new int[KeyCount];
        bKeys = new int[KeyCount];
        lookupKeys = new int[KeyCount];
        var rng = new Random(123);
        for (var i = 0; i < KeyCount; i++)
        {
            aKeys[i] = rng.Next(0, KeyCount);
            bKeys[i] = rng.Next(0, KeyCount);
            lookupKeys[i] = rng.Next(0, KeyCount);
        }

        _uintSet = new UIntSet(aKeys);
        _uintSetB = new UIntSet(bKeys);
        _intSet = new IntSet(aKeys);
        _intSetB = new IntSet(bKeys);
        _nativeUIntSet = new NativeUIntSet(aKeys);
        _nativeUIntSetB = new NativeUIntSet(bKeys);
        _nativeIntSet = new NativeIntSet(aKeys);
        _nativeIntSetB = new NativeIntSet(bKeys);
    }

    [IterationCleanup]
    public void Cleanup()
    {
        _nativeUIntSet?.Dispose();
        _nativeUIntSetB?.Dispose();
        _nativeIntSet?.Dispose();
        _nativeIntSetB?.Dispose();
    }

    // --- Allocate ---

    [BenchmarkCategory("Allocate"), Benchmark(OperationsPerInvoke = 1)]
    public UIntSet Allocate_UIntSet() => new UIntSet(aKeys);

    [BenchmarkCategory("Allocate"), Benchmark(OperationsPerInvoke = 1)]
    public NativeUIntSet Allocate_NativeUIntSet() => new NativeUIntSet(aKeys);

    [BenchmarkCategory("Allocate"), Benchmark(OperationsPerInvoke = 1)]
    public IntSet Allocate_IntSet() => new IntSet(aKeys);

    [BenchmarkCategory("Allocate"), Benchmark(OperationsPerInvoke = 1)]
    public NativeIntSet Allocate_NativeIntSet() => new NativeIntSet(aKeys);

    // --- Contains ---

    [BenchmarkCategory("Contains"), Benchmark(OperationsPerInvoke = 10000)]
    public void Contains_UIntSet()
    {
        for (var i = 0; i < KeyCount; i++)
            _uintSet.Contains(lookupKeys[i]);
    }

    [BenchmarkCategory("Contains"), Benchmark(OperationsPerInvoke = 10000)]
    public void Contains_NativeUIntSet()
    {
        for (var i = 0; i < KeyCount; i++)
            _nativeUIntSet.Contains(lookupKeys[i]);
    }

    [BenchmarkCategory("Contains"), Benchmark(OperationsPerInvoke = 10000)]
    public void Contains_IntSet()
    {
        for (var i = 0; i < KeyCount; i++)
            _intSet.Contains(lookupKeys[i]);
    }

    [BenchmarkCategory("Contains"), Benchmark(OperationsPerInvoke = 10000)]
    public void Contains_NativeIntSet()
    {
        for (var i = 0; i < KeyCount; i++)
            _nativeIntSet.Contains(lookupKeys[i]);
    }

    // --- Add ---

    [BenchmarkCategory("Add"), Benchmark(OperationsPerInvoke = 10000)]
    public void Add_UIntSet()
    {
        for (var i = 0; i < KeyCount; i++)
            _uintSet.Add(lookupKeys[i]);
    }

    [BenchmarkCategory("Add"), Benchmark(OperationsPerInvoke = 10000)]
    public void Add_NativeUIntSet()
    {
        for (var i = 0; i < KeyCount; i++)
            _nativeUIntSet.Add(lookupKeys[i]);
    }

    [BenchmarkCategory("Add"), Benchmark(OperationsPerInvoke = 10000)]
    public void Add_IntSet()
    {
        for (var i = 0; i < KeyCount; i++)
            _intSet.Add(lookupKeys[i]);
    }

    [BenchmarkCategory("Add"), Benchmark(OperationsPerInvoke = 10000)]
    public void Add_NativeIntSet()
    {
        for (var i = 0; i < KeyCount; i++)
            _nativeIntSet.Add(lookupKeys[i]);
    }

    // --- UnionWith ---

    [BenchmarkCategory("UnionWith"), Benchmark(OperationsPerInvoke = 1)]
    public void UnionWith_UIntSet_Span() => _uintSet.UnionWith(bKeys);

    [BenchmarkCategory("UnionWith"), Benchmark(OperationsPerInvoke = 1)]
    public void UnionWith_NativeUIntSet_Span() => _nativeUIntSet.UnionWith(bKeys);

    [BenchmarkCategory("UnionWith"), Benchmark(OperationsPerInvoke = 1)]
    public void UnionWith_IntSet_Span() => _intSet.UnionWith(bKeys);

    [BenchmarkCategory("UnionWith"), Benchmark(OperationsPerInvoke = 1)]
    public void UnionWith_NativeIntSet_Span() => _nativeIntSet.UnionWith(bKeys);

    // --- IntersectWith ---

    [BenchmarkCategory("IntersectWith"), Benchmark(OperationsPerInvoke = 1)]
    public void IntersectWith_UIntSet_Span() => _uintSet.IntersectWith(bKeys);

    [BenchmarkCategory("IntersectWith"), Benchmark(OperationsPerInvoke = 1)]
    public void IntersectWith_NativeUIntSet_Span() => _nativeUIntSet.IntersectWith(bKeys);

    [BenchmarkCategory("IntersectWith"), Benchmark(OperationsPerInvoke = 1)]
    public void IntersectWith_IntSet_Span() => _intSet.IntersectWith(bKeys);

    [BenchmarkCategory("IntersectWith"), Benchmark(OperationsPerInvoke = 1)]
    public void IntersectWith_NativeIntSet_Span() => _nativeIntSet.IntersectWith(bKeys);
}

[MarkdownExporterAttribute.GitHub]
[SimpleJob(RuntimeMoniker.Net80, launchCount: 1, warmupCount: 3, iterationCount: 10)]
[MemoryDiagnoser(false)]
public class NativeMapBenchmarks
{
    public const int N = 100000;

    private int[] keys, values, lookup;
    private UIntMap<int> umap;
    private NativeUIntMap<int> nativeUmap;
    private IntMap<int> imap;
    private NativeIntMap<int> nativeImap;

    [IterationSetup]
    public void Setup()
    {
        keys = new int[N];
        values = new int[N];
        lookup = new int[N];
        var rng = new Random(123);
        for (var i = 0; i < N; i++)
        {
            keys[i] = rng.Next(0, N);
            values[i] = rng.Next(0, N);
            lookup[i] = rng.Next(0, N);
        }

        umap = new UIntMap<int>();
        nativeUmap = new NativeUIntMap<int>();
        imap = new IntMap<int>();
        nativeImap = new NativeIntMap<int>();
        for (var i = 0; i < N; i++)
        {
            umap[keys[i]] = values[i];
            nativeUmap[keys[i]] = values[i];
            imap[keys[i]] = values[i];
            nativeImap[keys[i]] = values[i];
        }
    }

    [IterationCleanup]
    public void Cleanup()
    {
        nativeUmap?.Dispose();
        nativeImap?.Dispose();
    }

    [BenchmarkCategory("MapAdd"), Benchmark(OperationsPerInvoke = 100000)]
    public void UIntMap_Add()
    {
        var m = new UIntMap<int>();
        for (var i = 0; i < N; i++)
            m[keys[i]] = values[i];
    }

    [BenchmarkCategory("MapAdd"), Benchmark(OperationsPerInvoke = 100000)]
    public void NativeUIntMap_Add()
    {
        using var m = new NativeUIntMap<int>();
        for (var i = 0; i < N; i++)
            m[keys[i]] = values[i];
    }

    [BenchmarkCategory("MapAdd"), Benchmark(OperationsPerInvoke = 100000)]
    public void IntMap_Add()
    {
        var m = new IntMap<int>();
        for (var i = 0; i < N; i++)
            m[keys[i]] = values[i];
    }

    [BenchmarkCategory("MapAdd"), Benchmark(OperationsPerInvoke = 100000)]
    public void NativeIntMap_Add()
    {
        using var m = new NativeIntMap<int>();
        for (var i = 0; i < N; i++)
            m[keys[i]] = values[i];
    }

    [BenchmarkCategory("MapContains"), Benchmark(OperationsPerInvoke = 100000)]
    public void UIntMap_Contains()
    {
        foreach (var i in lookup)
            _ = umap.ContainsKey(i);
    }

    [BenchmarkCategory("MapContains"), Benchmark(OperationsPerInvoke = 100000)]
    public void NativeUIntMap_Contains()
    {
        foreach (var i in lookup)
            _ = nativeUmap.ContainsKey(i);
    }

    [BenchmarkCategory("MapContains"), Benchmark(OperationsPerInvoke = 100000)]
    public void IntMap_Contains()
    {
        foreach (var i in lookup)
            _ = imap.ContainsKey(i);
    }

    [BenchmarkCategory("MapContains"), Benchmark(OperationsPerInvoke = 100000)]
    public void NativeIntMap_Contains()
    {
        foreach (var i in lookup)
            _ = nativeImap.ContainsKey(i);
    }

    [BenchmarkCategory("MapRemove"), Benchmark(OperationsPerInvoke = 100000)]
    public void UIntMap_Remove()
    {
        foreach (var i in lookup)
            _ = umap.Remove(i);
    }

    [BenchmarkCategory("MapRemove"), Benchmark(OperationsPerInvoke = 100000)]
    public void NativeUIntMap_Remove()
    {
        foreach (var i in lookup)
            _ = nativeUmap.Remove(i);
    }

    [BenchmarkCategory("MapRemove"), Benchmark(OperationsPerInvoke = 100000)]
    public void IntMap_Remove()
    {
        foreach (var i in lookup)
            _ = imap.Remove(i);
    }

    [BenchmarkCategory("MapRemove"), Benchmark(OperationsPerInvoke = 100000)]
    public void NativeIntMap_Remove()
    {
        foreach (var i in lookup)
            _ = nativeImap.Remove(i);
    }

    [BenchmarkCategory("MapIterate"), Benchmark(OperationsPerInvoke = 1)]
    public void UIntMap_Iterate()
    {
        foreach (var i in umap.Keys)
            _ = i;
    }

    [BenchmarkCategory("MapIterate"), Benchmark(OperationsPerInvoke = 1)]
    public void NativeUIntMap_Iterate()
    {
        foreach (var i in nativeUmap.Keys)
            _ = i;
    }

    [BenchmarkCategory("MapIterate"), Benchmark(OperationsPerInvoke = 1)]
    public void IntMap_Iterate()
    {
        foreach (var i in imap.Keys)
            _ = i;
    }

    [BenchmarkCategory("MapIterate"), Benchmark(OperationsPerInvoke = 1)]
    public void NativeIntMap_Iterate()
    {
        foreach (var i in nativeImap.Keys)
            _ = i;
    }
}
