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

### IntMap vs Dictionary

| Method           | Mean        | Error       | StdDev      | Median      | Op/s     | Allocated  |
|----------------- |------------:|------------:|------------:|------------:|---------:|-----------:|
| IntMap_Add       | 15,001.6 μs |   316.59 μs |   892.94 μs | 15,005.5 μs |    66.66 |  7953312 B |
| Dict_Add         | 29,555.9 μs |   973.25 μs | 2,745.06 μs | 29,262.0 μs |    33.83 | 25983264 B |
| IntMap_Contains  |    957.9 μs |    54.06 μs |   153.35 μs |    998.5 μs | 1,043.96 |          - |
| Dict_Contains    | 29,363.4 μs | 2,025.83 μs | 5,877.29 μs | 27,123.3 μs |    34.06 |          - |
| IntMap_Remove    |  7,772.9 μs |   142.39 μs |   401.61 μs |  7,664.4 μs |   128.65 |          - |
| Dict_Remove      | 19,943.4 μs |   534.14 μs | 1,506.56 μs | 19,818.6 μs |    50.14 |          - |
| IntMap_Iteration |  1,014.0 μs |    93.37 μs |   273.83 μs |  1,143.7 μs |   986.17 |          - |
| Dict_Iteration   |    690.4 μs |    22.09 μs |    61.21 μs |    707.2 μs | 1,448.54 |          - |


### IntSet vs HashSet

| Method                            | Mean       | Error     | StdDev     | Median     | Op/s      | Allocated |
|---------------------------------- |-----------:|----------:|-----------:|-----------:|----------:|----------:|
| IntSet_Contains                   |  20.410 μs | 0.2930 μs |  0.8313 μs |  20.200 μs |  48,996.4 |         - |
| HashSet_Contains                  |  98.690 μs | 0.4036 μs |  1.0843 μs |  98.400 μs |  10,132.7 |         - |
| IntSet_Add                        |  60.762 μs | 0.4171 μs |  1.1204 μs |  60.450 μs |  16,457.7 |         - |
| HashSet_Add                       |  94.316 μs | 0.4887 μs |  1.2703 μs |  94.100 μs |  10,602.6 |         - |
| IntSet_DenseValues_Iterate        |  29.728 μs | 0.7883 μs |  2.2871 μs |  29.000 μs |  33,638.5 |         - |
| HashSet_DenseValues_Iterate       |  15.567 μs | 0.4117 μs |  1.1746 μs |  15.300 μs |  64,238.4 |         - |
| IntSet_Remove                     |  55.401 μs | 0.3568 μs |  1.0005 μs |  55.100 μs |  18,050.2 |         - |
| HashSet_Remove                    |  80.070 μs | 0.6005 μs |  1.6539 μs |  79.550 μs |  12,489.0 |         - |
| IntSet_ExceptWith_Span            |  53.456 μs | 0.4827 μs |  1.3694 μs |  53.100 μs |  18,707.0 |         - |
| IntSet_ExceptWith_IntSet          |   8.945 μs | 0.7440 μs |  2.1584 μs |   8.700 μs | 111,789.8 |         - |
| HashSet_ExceptWith                | 120.430 μs | 2.8579 μs |  7.8234 μs | 117.600 μs |   8,303.6 |      32 B |
| IntSet_IntersectWith_Span         |  10.413 μs | 0.0516 μs |  0.1413 μs |  10.400 μs |  96,037.1 |         - |
| IntSet_IntersectWith_IntSet       |   6.461 μs | 0.4876 μs |  1.4377 μs |   6.300 μs | 154,774.8 |         - |
| HashSet_IntersectWith             | 163.645 μs | 1.0931 μs |  2.9924 μs | 162.500 μs |   6,110.8 |     848 B |
| IntSet_SymmetricExceptWith_Span   |  20.040 μs | 0.0302 μs |  0.0828 μs |  20.000 μs |  49,899.6 |         - |
| IntSet_SymmetricExceptWith_IntSet |  11.454 μs | 0.8290 μs |  2.4442 μs |  11.300 μs |  87,305.7 |         - |
| HashSet_SymmetricExceptWith       | 196.834 μs | 4.7433 μs | 13.2225 μs | 190.700 μs |   5,080.4 |    1664 B |
| IntSet_UnionWith_Span             |  54.532 μs | 0.3342 μs |  0.8978 μs |  54.300 μs |  18,337.8 |         - |
| IntSet_UnionWith_IntSet           |  11.504 μs | 0.8296 μs |  2.4069 μs |  11.500 μs |  86,925.4 |         - |
| HashSet_UnionWith                 | 120.065 μs | 0.3118 μs |  0.8376 μs | 119.900 μs |   8,328.8 |      32 B |


### Memory Usage

| Method                                 | N        | Mean           | Error | Op/s        | Ratio | Allocated   | Alloc Ratio |
|--------------------------------------- |--------- |---------------:|------:|------------:|------:|------------:|------------:|
| **IntSet_MemoryUsage**                     | **10**       |       **1.100 μs** |    **NA** | **909,090.909** |  **0.52** |       **192 B** |        **0.59** |
| DenseIdMapMemoryUsage                  | 10       |       1.500 μs |    NA | 666,666.667 |  0.71 |       192 B |        0.59 |
| HashSet_MemoryUsage                    | 10       |       2.100 μs |    NA | 476,190.476 |  1.00 |       328 B |        1.00 |
| IntSet_MemoryUsage_2KeysExtremeValues  | 10       |      12.100 μs |    NA |  82,644.628 |  5.76 |   2500224 B |    7,622.63 |
| HashSet_MemoryUsage_2KeysExtremeValues | 10       |       1.300 μs |    NA | 769,230.769 |  0.62 |       168 B |        0.51 |
|                                        |          |                |       |             |       |             |             |
| **IntSet_MemoryUsage**                     | **100**      |       **3.000 μs** |    **NA** | **333,333.333** |  **1.07** |       **192 B** |        **0.10** |
| DenseIdMapMemoryUsage                  | 100      |       3.800 μs |    NA | 263,157.895 |  1.36 |       192 B |        0.10 |
| HashSet_MemoryUsage                    | 100      |       2.800 μs |    NA | 357,142.857 |  1.00 |      1864 B |        1.00 |
| IntSet_MemoryUsage_2KeysExtremeValues  | 100      |      11.800 μs |    NA |  84,745.763 |  4.21 |   2500224 B |    1,341.32 |
| HashSet_MemoryUsage_2KeysExtremeValues | 100      |       1.200 μs |    NA | 833,333.333 |  0.43 |       168 B |        0.09 |
|                                        |          |                |       |             |       |             |             |
| **IntSet_MemoryUsage**                     | **1000**     |      **17.000 μs** |    **NA** |  **58,823.529** |  **1.55** |       **472 B** |       **0.027** |
| DenseIdMapMemoryUsage                  | 1000     |      17.300 μs |    NA |  57,803.468 |  1.57 |       472 B |       0.027 |
| HashSet_MemoryUsage                    | 1000     |      11.000 μs |    NA |  90,909.091 |  1.00 |     17800 B |       1.000 |
| IntSet_MemoryUsage_2KeysExtremeValues  | 1000     |      19.600 μs |    NA |  51,020.408 |  1.78 |   2500224 B |     140.462 |
| HashSet_MemoryUsage_2KeysExtremeValues | 1000     |       1.200 μs |    NA | 833,333.333 |  0.11 |       168 B |       0.009 |
|                                        |          |                |       |             |       |             |             |
| **IntSet_MemoryUsage**                     | **10000**    |      **46.300 μs** |    **NA** |  **21,598.272** |  **0.51** |      **8248 B** |       **0.051** |
| DenseIdMapMemoryUsage                  | 10000    |      45.100 μs |    NA |  22,172.949 |  0.49 |      8248 B |       0.051 |
| HashSet_MemoryUsage                    | 10000    |      91.300 μs |    NA |  10,952.903 |  1.00 |    161800 B |       1.000 |
| IntSet_MemoryUsage_2KeysExtremeValues  | 10000    |      12.600 μs |    NA |  79,365.079 |  0.14 |   2500224 B |      15.453 |
| HashSet_MemoryUsage_2KeysExtremeValues | 10000    |       1.400 μs |    NA | 714,285.714 |  0.02 |       168 B |       0.001 |
|                                        |          |                |       |             |       |             |             |
| **IntSet_MemoryUsage**                     | **100000**   |     **340.200 μs** |    **NA** |   **2,939.447** | **0.328** |     **65664 B** |       **0.038** |
| DenseIdMapMemoryUsage                  | 100000   |     280.200 μs |    NA |   3,568.879 | 0.270 |     65664 B |       0.038 |
| HashSet_MemoryUsage                    | 100000   |   1,037.500 μs |    NA |     963.855 | 1.000 |   1738248 B |       1.000 |
| IntSet_MemoryUsage_2KeysExtremeValues  | 100000   |      13.900 μs |    NA |  71,942.446 | 0.013 |   2500224 B |       1.438 |
| HashSet_MemoryUsage_2KeysExtremeValues | 100000   |       1.100 μs |    NA | 909,090.909 | 0.001 |       168 B |       0.000 |
|                                        |          |                |       |             |       |             |             |
| **IntSet_MemoryUsage**                     | **1000000**  |   **2,476.900 μs** |    **NA** |     **403.730** | **0.244** |    **524488 B** |       **0.028** |
| DenseIdMapMemoryUsage                  | 1000000  |   3,375.400 μs |    NA |     296.261 | 0.332 |    524488 B |       0.028 |
| HashSet_MemoryUsage                    | 1000000  |  10,159.700 μs |    NA |      98.428 | 1.000 |  18603144 B |       1.000 |
| IntSet_MemoryUsage_2KeysExtremeValues  | 1000000  |      23.600 μs |    NA |  42,372.881 | 0.002 |   2500224 B |       0.134 |
| HashSet_MemoryUsage_2KeysExtremeValues | 1000000  |       1.600 μs |    NA | 625,000.000 | 0.000 |       168 B |       0.000 |
|                                        |          |                |       |             |       |             |             |
| **IntSet_MemoryUsage**                     | **10000000** |  **26,267.900 μs** |    **NA** |      **38.069** | **0.257** |   **8388904 B** |       **0.052** |
| DenseIdMapMemoryUsage                  | 10000000 |  24,824.800 μs |    NA |      40.282 | 0.242 |   8388904 B |       0.052 |
| HashSet_MemoryUsage                    | 10000000 | 102,408.800 μs |    NA |       9.765 | 1.000 | 160000456 B |       1.000 |
| IntSet_MemoryUsage_2KeysExtremeValues  | 10000000 |      28.100 μs |    NA |  35,587.189 | 0.000 |   2500224 B |       0.016 |
| HashSet_MemoryUsage_2KeysExtremeValues | 10000000 |       3.200 μs |    NA | 312,500.000 | 0.000 |       168 B |       0.000 |