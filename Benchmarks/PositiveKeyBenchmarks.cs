using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace IntSet.Benchmarks;

/// <summary>
/// Compares IntSet vs UIntSet and IntMap vs UIntMap when keys are always non-negative.
/// UIntSet/UIntMap skip ZigZag encoding, which halves the pages array and speeds up all operations.
/// </summary>
[MarkdownExporterAttribute.GitHub]
[SimpleJob(RuntimeMoniker.Net80, launchCount: 1, warmupCount: 3, iterationCount: 10)]
[MemoryDiagnoser(false)]
public class PositiveKeyBenchmarks
{
    [Params(10, 100, 1000, 10000, 100000, 1000000, 10000000)] public int KeyCount = 10000;

    private int[] aKeys, bKeys, lookupKeys;
    private HashSet<int> hashSet, hashSetB;
    private IntSet _intSet, _intSetB;
    private UIntSet _uintSet, _uintSetB;

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

        hashSet = new HashSet<int>(aKeys);
        hashSetB = new HashSet<int>(bKeys);
        _intSet = new IntSet(aKeys);
        _intSetB = new IntSet(bKeys);
        _uintSet = new UIntSet(aKeys);
        _uintSetB = new UIntSet(bKeys);
    }

    // --- Allocate ---

    [BenchmarkCategory("Allocate"), Benchmark(OperationsPerInvoke = 1)]
    public HashSet<int> Allocate_HashSet()
    {
        return new HashSet<int>(aKeys);
    }

    [BenchmarkCategory("Allocate"), Benchmark(OperationsPerInvoke = 1)]
    public IntSet Allocate_IntSet()
    {
        return new IntSet(aKeys);
    }

    [BenchmarkCategory("Allocate"), Benchmark(OperationsPerInvoke = 1)]
    public UIntSet Allocate_UIntSet()
    {
        return new UIntSet(aKeys);
    }

    // --- Contains ---

    [BenchmarkCategory("Contains"), Benchmark(OperationsPerInvoke = 10000)]
    public void Contains_HashSet()
    {
        for (var i = 0; i < KeyCount; i++)
            hashSet.Contains(lookupKeys[i]);
    }

    [BenchmarkCategory("Contains"), Benchmark(OperationsPerInvoke = 10000)]
    public void Contains_IntSet()
    {
        for (var i = 0; i < KeyCount; i++)
            _intSet.Contains(lookupKeys[i]);
    }

    [BenchmarkCategory("Contains"), Benchmark(OperationsPerInvoke = 10000)]
    public void Contains_UIntSet()
    {
        for (var i = 0; i < KeyCount; i++)
            _uintSet.Contains(lookupKeys[i]);
    }

    // --- Add ---

    [BenchmarkCategory("Add"), Benchmark(OperationsPerInvoke = 10000)]
    public void Add_HashSet()
    {
        for (var i = 0; i < KeyCount; i++)
            hashSet.Add(lookupKeys[i]);
    }

    [BenchmarkCategory("Add"), Benchmark(OperationsPerInvoke = 10000)]
    public void Add_IntSet()
    {
        for (var i = 0; i < KeyCount; i++)
            _intSet.Add(lookupKeys[i]);
    }

    [BenchmarkCategory("Add"), Benchmark(OperationsPerInvoke = 10000)]
    public void Add_UIntSet()
    {
        for (var i = 0; i < KeyCount; i++)
            _uintSet.Add(lookupKeys[i]);
    }

    // --- Remove ---

    [BenchmarkCategory("Remove"), Benchmark(OperationsPerInvoke = 10000)]
    public void Remove_HashSet()
    {
        for (var i = 0; i < KeyCount; i++)
            hashSet.Remove(lookupKeys[i]);
    }

    [BenchmarkCategory("Remove"), Benchmark(OperationsPerInvoke = 10000)]
    public void Remove_IntSet()
    {
        for (var i = 0; i < KeyCount; i++)
            _intSet.Remove(lookupKeys[i]);
    }

    [BenchmarkCategory("Remove"), Benchmark(OperationsPerInvoke = 10000)]
    public void Remove_UIntSet()
    {
        for (var i = 0; i < KeyCount; i++)
            _uintSet.Remove(lookupKeys[i]);
    }

    // --- Iterate ---

    [BenchmarkCategory("Iterate"), Benchmark(OperationsPerInvoke = 1)]
    public void Iterate_HashSet()
    {
        var x = 0;
        foreach (var i in hashSet)
            x += i;
    }

    [BenchmarkCategory("Iterate"), Benchmark(OperationsPerInvoke = 1)]
    public void Iterate_IntSet()
    {
        var x = 0;
        foreach (var i in _intSet)
            x += i;
    }

    [BenchmarkCategory("Iterate"), Benchmark(OperationsPerInvoke = 1)]
    public void Iterate_UIntSet()
    {
        var x = 0;
        foreach (var i in _uintSet)
            x += i;
    }

    // --- UnionWith ---

    [BenchmarkCategory("UnionWith"), Benchmark(OperationsPerInvoke = 1)]
    public void UnionWith_HashSet_to_Span()
    {
        hashSet.UnionWith(bKeys);
    }

    [BenchmarkCategory("UnionWith"), Benchmark(OperationsPerInvoke = 1)]
    public void UnionWith_IntSet_to_Span()
    {
        _intSet.UnionWith(bKeys);
    }

    [BenchmarkCategory("UnionWith"), Benchmark(OperationsPerInvoke = 1)]
    public void UnionWith_UIntSet_to_Span()
    {
        _uintSet.UnionWith(bKeys);
    }

    [BenchmarkCategory("UnionWith"), Benchmark(OperationsPerInvoke = 1)]
    public void UnionWith_HashSet_to_HashSet()
    {
        hashSet.UnionWith(hashSetB);
    }

    [BenchmarkCategory("UnionWith"), Benchmark(OperationsPerInvoke = 1)]
    public void UnionWith_IntSet_to_IntSet()
    {
        _intSet.UnionWith(_intSetB);
    }

    [BenchmarkCategory("UnionWith"), Benchmark(OperationsPerInvoke = 1)]
    public void UnionWith_UIntSet_to_UIntSet()
    {
        _uintSet.UnionWith(_uintSetB);
    }

    // --- IntersectWith ---

    [BenchmarkCategory("IntersectWith"), Benchmark(OperationsPerInvoke = 1)]
    public void IntersectWith_HashSet_to_Span()
    {
        hashSet.IntersectWith(bKeys);
    }

    [BenchmarkCategory("IntersectWith"), Benchmark(OperationsPerInvoke = 1)]
    public void IntersectWith_IntSet_to_Span()
    {
        _intSet.IntersectWith(bKeys);
    }

    [BenchmarkCategory("IntersectWith"), Benchmark(OperationsPerInvoke = 1)]
    public void IntersectWith_UIntSet_to_Span()
    {
        _uintSet.IntersectWith(bKeys);
    }

    [BenchmarkCategory("IntersectWith"), Benchmark(OperationsPerInvoke = 1)]
    public void IntersectWith_HashSet_to_HashSet()
    {
        hashSet.IntersectWith(hashSetB);
    }

    [BenchmarkCategory("IntersectWith"), Benchmark(OperationsPerInvoke = 1)]
    public void IntersectWith_IntSet_to_IntSet()
    {
        _intSet.IntersectWith(_intSetB);
    }

    [BenchmarkCategory("IntersectWith"), Benchmark(OperationsPerInvoke = 1)]
    public void IntersectWith_UIntSet_to_UIntSet()
    {
        _uintSet.IntersectWith(_uintSetB);
    }

    // --- ExceptWith ---

    [BenchmarkCategory("ExceptWith"), Benchmark(OperationsPerInvoke = 1)]
    public void ExceptWith_HashSet_to_Span()
    {
        hashSet.ExceptWith(bKeys);
    }

    [BenchmarkCategory("ExceptWith"), Benchmark(OperationsPerInvoke = 1)]
    public void ExceptWith_IntSet_to_Span()
    {
        _intSet.ExceptWith(bKeys);
    }

    [BenchmarkCategory("ExceptWith"), Benchmark(OperationsPerInvoke = 1)]
    public void ExceptWith_UIntSet_to_Span()
    {
        _uintSet.ExceptWith(bKeys);
    }

    [BenchmarkCategory("ExceptWith"), Benchmark(OperationsPerInvoke = 1)]
    public void ExceptWith_HashSet_to_HashSet()
    {
        hashSet.ExceptWith(hashSetB);
    }

    [BenchmarkCategory("ExceptWith"), Benchmark(OperationsPerInvoke = 1)]
    public void ExceptWith_IntSet_to_IntSet()
    {
        _intSet.ExceptWith(_intSetB);
    }

    [BenchmarkCategory("ExceptWith"), Benchmark(OperationsPerInvoke = 1)]
    public void ExceptWith_UIntSet_to_UIntSet()
    {
        _uintSet.ExceptWith(_uintSetB);
    }

    // --- SymmetricExceptWith ---

    [BenchmarkCategory("SymmetricExceptWith"), Benchmark(OperationsPerInvoke = 1)]
    public void SymmetricExceptWith_HashSet_to_Span()
    {
        hashSet.SymmetricExceptWith(bKeys);
    }

    [BenchmarkCategory("SymmetricExceptWith"), Benchmark(OperationsPerInvoke = 1)]
    public void SymmetricExceptWith_IntSet_to_Span()
    {
        _intSet.SymmetricExceptWith(bKeys);
    }

    [BenchmarkCategory("SymmetricExceptWith"), Benchmark(OperationsPerInvoke = 1)]
    public void SymmetricExceptWith_UIntSet_to_Span()
    {
        _uintSet.SymmetricExceptWith(bKeys);
    }

    [BenchmarkCategory("SymmetricExceptWith"), Benchmark(OperationsPerInvoke = 1)]
    public void SymmetricExceptWith_HashSet_to_HashSet()
    {
        hashSet.SymmetricExceptWith(hashSetB);
    }

    [BenchmarkCategory("SymmetricExceptWith"), Benchmark(OperationsPerInvoke = 1)]
    public void SymmetricExceptWith_IntSet_to_IntSet()
    {
        _intSet.SymmetricExceptWith(_intSetB);
    }

    [BenchmarkCategory("SymmetricExceptWith"), Benchmark(OperationsPerInvoke = 1)]
    public void SymmetricExceptWith_UIntSet_to_UIntSet()
    {
        _uintSet.SymmetricExceptWith(_uintSetB);
    }
}

/// <summary>
/// Compares IntMap vs UIntMap when keys are always non-negative.
/// </summary>
[MarkdownExporterAttribute.GitHub]
[SimpleJob(RuntimeMoniker.Net80, launchCount: 1, warmupCount: 3, iterationCount: 100)]
[MemoryDiagnoser(false)]
public class PositiveKeyMapBenchmarks
{
    public const int N = 100000;

    private int[] keys, values, lookup;
    private IntMap<int> map;
    private UIntMap<int> umap;
    private Dictionary<int, int> dict;

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

        map = new IntMap<int>();
        umap = new UIntMap<int>();
        dict = new Dictionary<int, int>();
        for (var i = 0; i < N; i++)
        {
            map[keys[i]] = values[i];
            umap[keys[i]] = values[i];
            dict[keys[i]] = values[i];
        }
    }

    [BenchmarkCategory("MapAdd"), Benchmark(OperationsPerInvoke = 100000)]
    public void Dict_Add()
    {
        var d = new Dictionary<int, int>();
        for (var i = 0; i < N; i++)
            d[keys[i]] = values[i];
    }

    [BenchmarkCategory("MapAdd"), Benchmark(OperationsPerInvoke = 100000)]
    public void IntMap_Add()
    {
        var m = new IntMap<int>();
        for (var i = 0; i < N; i++)
            m[keys[i]] = values[i];
    }

    [BenchmarkCategory("MapAdd"), Benchmark(OperationsPerInvoke = 100000)]
    public void UIntMap_Add()
    {
        var m = new UIntMap<int>();
        for (var i = 0; i < N; i++)
            m[keys[i]] = values[i];
    }

    [BenchmarkCategory("MapContains"), Benchmark(OperationsPerInvoke = 100000)]
    public void Dict_Contains()
    {
        foreach (var i in lookup)
            _ = dict.ContainsKey(i);
    }

    [BenchmarkCategory("MapContains"), Benchmark(OperationsPerInvoke = 100000)]
    public void IntMap_Contains()
    {
        foreach (var i in lookup)
            _ = map.ContainsKey(i);
    }

    [BenchmarkCategory("MapContains"), Benchmark(OperationsPerInvoke = 100000)]
    public void UIntMap_Contains()
    {
        foreach (var i in lookup)
            _ = umap.ContainsKey(i);
    }

    [BenchmarkCategory("MapRemove"), Benchmark(OperationsPerInvoke = 100000)]
    public void Dict_Remove()
    {
        foreach (var i in lookup)
            _ = dict.Remove(i);
    }

    [BenchmarkCategory("MapRemove"), Benchmark(OperationsPerInvoke = 100000)]
    public void IntMap_Remove()
    {
        foreach (var i in lookup)
            _ = map.Remove(i);
    }

    [BenchmarkCategory("MapRemove"), Benchmark(OperationsPerInvoke = 100000)]
    public void UIntMap_Remove()
    {
        foreach (var i in lookup)
            _ = umap.Remove(i);
    }

    [BenchmarkCategory("MapIterate"), Benchmark(OperationsPerInvoke = 1)]
    public void Dict_Iterate()
    {
        foreach (var kvp in dict)
            _ = kvp.Key;
    }

    [BenchmarkCategory("MapIterate"), Benchmark(OperationsPerInvoke = 1)]
    public void IntMap_Iterate()
    {
        foreach (var i in map.Keys)
            _ = i;
    }

    [BenchmarkCategory("MapIterate"), Benchmark(OperationsPerInvoke = 1)]
    public void UIntMap_Iterate()
    {
        foreach (var i in umap.Keys)
            _ = i;
    }
}
