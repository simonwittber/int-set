# IntSet

IntSet is a high-performance integer set implementation for .NET, designed as a replacement for `HashSet<int>` and `Dictionary<int, T>` when you need fast operations and predictable memory usage.

Unlike `HashSet<int>`, IntSet's memory usage is dictated by the largest key value in the set, not the number of items. This makes it ideal for scenarios where you need to store many small integers efficiently, but can be pathological if you store a few keys with very large values.

## Why IntSet?
- **Fastest possible operations**: Add, Contains, Remove, and set operations (Union, Intersect, Except, SymmetricExcept) are significantly faster than `HashSet<int>` for most workloads.
- **Predictable memory usage**: Memory is allocated based on the largest key, not the count. For dense sets of small integers, this is extremely efficient. Usually an IntSet with 10 million items uses around 2.5 MB of memory vs the 152 MB used by HashSet.
- **No allocations for set operations**: Most set operations do not allocate additional memory.

## API Usage
IntSet uses a Span-based API.

```csharp
var set = new IntSet();
set.Add(1);
set.Add(100);
set.Contains(1); // true
set.Remove(100);

// Bulk operations with Span
Span<int> values = stackalloc int[] { 1, 2, 3 };
set.AddRange(values);
set.IntersectWith(values);
set.UnionWith(values);
set.ExceptWith(values);
set.SymmetricExceptWith(values);
```

## Benchmarks

### Add & Contains
| Method         | Mean     | Ratio | Allocated |
|---------------|----------|-------|-----------|
| IntSet_Add    | 53.45 μs | 0.57  | -         |
| HashSet_Add   | 93.66 μs | 1.00  | -         |
| IntSet_Contains | 17.65 μs | 0.18  | -         |
| HashSet_Contains | 98.50 μs | 1.00  | -         |
| IntSet_Remove | 54.11 μs | 0.68  | -         |
| HashSet_Remove | 79.78 μs | 1.00  | -         |

### Set Operations
| Method                  | Mean      | Ratio | Allocated |
|------------------------|-----------|-------|-----------|
| IntSet_UnionWith        | 47.85 μs  | 0.40  | -         |
| HashSet_UnionWith       | 118.83 μs | 1.00  | 32 B      |
| IntSet_IntersectWith    | 10.74 μs  | 0.07  | -         |
| HashSet_IntersectWith   | 163.45 μs | 1.00  | 848 B     |
| IntSet_ExceptWith       | 52.54 μs  | 0.45  | -         |
| HashSet_ExceptWith      | 117.24 μs | 1.00  | 32 B      |
| IntSet_SymmetricExceptWith | 16.97 μs | 0.09 | -      |
| HashSet_SymmetricExceptWith | 188.81 μs | 1.00 | 1664 B |

### Iteration

Iteration speed is the one area where HashSet<int> outperforms IntSet, especially for sparse sets. However, IntSet is still competitive for dense sets.

| Method                      | Mean    | Ratio |
|----------------------------|---------|-------|
| IntSet_DenseValues_Iterate  | 131.87 μs | 1.57  |
| HashSet_DenseValues_Iterate | 84.65 μs | 1.00  |
| IntSet_SparseValues_Iterate | 400.7 μs  | 3.33  |
| HashSet_SparseValues_Iterate| 121.0 μs | 1.00  |

## Memory Usage
IntSet's memory usage is determined by the largest key value, not the number of items. For example, storing `{1, 2, 3}` uses very little memory, but storing `{1, 10000000}` uses memory proportional to the largest key (10,000,000).
Even with this limitation, memory usage in the general case is still better than HashSet<int>.

### Pathological Case: Two Extreme Keys
In this benchmark, IntSet stores two keys: one small and one extremely large. The memory usage is much higher than HashSet, demonstrating the tradeoff.

| Method                              | N        | Mean      | Allocated   | Alloc Ratio |
|-------------------------------------|----------|-----------|-------------|-------------|
| IntSet_MemoryUsage_2KeysExtremeValues | 10       | 12.000 μs | 2,500,216 B | 5,787.54    |
| HashSet_MemoryUsage_2KeysExtremeValues| 10       | 1.400 μs  | 168 B       | 0.39        |


### General Memory Usage (N = number of items)

| Method      | N        | Allocated   | Alloc Ratio |
|-------------|----------|------------:|------------:|
| **IntSet**  | **10**       |       **0.00018 MB** |        **0.43** |
| HashSet     | 10       |       0.00043 MB |        1.00 |
|             |          |             |             |
| **IntSet**  | **100**      |       **0.00018 MB** |        **0.09** |
| HashSet     | 100      |      0.00197 MB |        1.00 |
|             |          |             |             |
| **IntSet**  | **1000**     |       **0.00046 MB** |       **0.026** |
| HashSet     | 1000     |     0.01790 MB |       1.000 |
|             |          |             |             |
| **IntSet**  | **10000**    |      **0.00271 MB** |       **0.017** |
| HashSet     | 10000    |    0.16190 MB |       1.000 |
|             |          |             |             |
| **IntSet**  | **100000**   |     **0.02522 MB** |       **0.015** |
| HashSet     | 100000   |   1.73835 MB |       1.000 |
|             |          |             |             |
| **IntSet**  | **1000000**  |    **0.25022 MB** |       **0.013** |
| HashSet     | 1000000  |  18.60325 MB |       1.000 |
|             |          |             |             |
| **IntSet**  | **10000000** |   **2.50022 MB** |       **0.016** |
| HashSet     | 10000000 | 152.56367 MB |       1.000 |

## When to Use IntSet
- Use IntSet for sets of small, dense integer values where performance and memory efficiency are critical.
- Avoid IntSet if you need to store a few keys with very large values, as memory usage will be high.

