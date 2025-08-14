# IntSet

IntSet is a high-performance integer set implementation for .NET using Bitmaps.
It is a replacement for `HashSet<int>` and `Dictionary<int, T>` when you need fast operations and predictable memory usage.


## Why IntSet?
- **Fastest possible operations**: Add, Contains, Remove, and set operations (Union, Intersect, Except, SymmetricExcept) are significantly faster than `HashSet<int>` for most workloads.
- **No allocations for set operations**: Most set operations do not allocate additional memory.

## IntMap Benchmarks vs Dictionary<int>

| Method           | Mean       | Error     | StdDev    | Median     | Op/s            | Allocated |
|----------------- |-----------:|----------:|----------:|-----------:|----------------:|----------:|
| **Dict_Add**         | **27.5026 ns** | **0.8726 ns** | **2.4035 ns** | **27.3824 ns** |    **36,360,257.3** |      **26 B** |
| **Dict_Contains**    | **23.9300 ns** | **0.8608 ns** | **2.4280 ns** | **23.2008 ns** |    **41,788,606.9** |         **-** |
| **Dict_Iteration**   |  **0.7098 ns** | **0.0298 ns** | **0.0836 ns** |  **0.7011 ns** | **1,408,871,555.8** |         **-** |
| **Dict_Remove**      | **20.3148 ns** | **0.5686 ns** | **1.6038 ns** | **19.8500 ns** |    **49,225,216.5** |         **-** |
| **IntMap_Add**       |  **9.7803 ns** | **0.0794 ns** | **0.2226 ns** |  **9.7541 ns** |   **102,246,168.5** |       **5 B** |
| **IntMap_Contains**  |  **0.7500 ns** | **0.0094 ns** | **0.0260 ns** |  **0.7441 ns** | **1,333,351,311.1** |         **-** |
| **IntMap_Iteration** |  **0.7491 ns** | **0.0106 ns** | **0.0306 ns** |  **0.7356 ns** | **1,334,937,092.8** |         **-** |
| **IntMap_Remove**    |  **6.4778 ns** | **0.0800 ns** | **0.2245 ns** |  **6.4623 ns** |   **154,372,612.7** |         **-** |


## Bitmap, ClusteredBitmap, NativeClusteredBitmap Benchmarks vs HashSet
| Method                                            | KeyCount | Mean           | Op/s          |
|-------------------------------------------------- |--------- |---------------:|--------------:|
| Add_HashSet                                       | 10000    |       9.387 ns | 106,533,645.8 |
| Add_Bitmap                                        | 10000    |       4.960 ns | 201,626,453.4 |
| Add_ClusteredBitmap                               | 10000    |       5.485 ns | 182,313,579.4 |
| Add_NativeClusteredBitmap                         | 10000    |       5.412 ns | 184,786,581.7 |
|                                                   |          |                |               |
| Contains_HashSet                                  | 10000    |       9.707 ns | 103,018,214.5 |
| Contains_Bitmap                                   | 10000    |       1.990 ns | 502,512,562.8 |
| Contains_ClusteredBitmap                          | 10000    |       2.363 ns | 423,156,567.9 |
| Contains_NativeClusteredBitmap                    | 10000    |       2.323 ns | 430,560,644.9 |
|                                                   |          |                |               |
| ExceptWith_HashSet_to_Span                        | 10000    | 114,019.792 ns |       8,770.4 |
| ExceptWith_Bitmap_to_Span                         | 10000    |  54,144.186 ns |      18,469.2 |
| ExceptWith_ClusteredBitmap_to_Span                | 10000    |  58,078.947 ns |      17,217.9 |
| ExceptWith_NativeClusteredBitmap_to_Span          | 10000    |  60,449.485 ns |      16,542.7 |
| ExceptWith_HashSet_to_HashSet                     | 10000    | 101,813.542 ns |       9,821.9 |
| ExceptWith_Bitmap_to_Bitmap                       | 10000    |   3,596.000 ns |     278,086.8 |
|                                                   |          |                |               |
| IntersectWith_HashSet_to_Span                     | 10000    | 164,390.909 ns |       6,083.1 |
| IntersectWith_Bitmap_to_Span                      | 10000    |   7,065.217 ns |     141,538.5 |
| IntersectWith_ClusteredBitmap_to_Span             | 10000    |  10,223.656 ns |      97,812.4 |
| IntersectWith_NativeClusteredBitmap_to_Span       | 10000    |  10,272.368 ns |      97,348.5 |
| IntersectWith_HashSet_to_HashSet                  | 10000    |  82,578.022 ns |      12,109.8 |
| IntersectWith_Bitmap_to_Bitmap                    | 10000    |   3,654.000 ns |     273,672.7 |
|                                                   |          |                |               |
| Iterate_HashSet                                   | 10000    |  15,026.596 ns |      66,548.7 |
| Iterate_Bitmap                                    | 10000    |  24,263.830 ns |      41,213.6 |
| Iterate_ClusteredBitmap                           | 10000    |  29,372.449 ns |      34,045.5 |
| Iterate_NativeClusteredBitmap                     | 10000    |  29,051.064 ns |      34,422.1 |
|                                                   |          |                |               |
| Allocate_HashSet_with_RandomKeys                  | 10000    | 130,323.256 ns |       7,673.2 |
| Allocate_Bitmap_with_RandomKeys                   | 10000    |  11,410.227 ns |      87,640.7 |
| Allocate_ClusteredBitmap_with_RandomKeys          | 10000    |  17,946.591 ns |      55,720.9 |
| Allocate_NativeClusteredBitmap_with_RandomKeys    | 10000    |  17,904.494 ns |      55,851.9 |
| Allocate_HashSet_with_ClusteredKeys               | 10000    | 130,505.747 ns |       7,662.5 |
| Allocate_Bitmap_with_ClusteredKeys                | 10000    | 453,322.680 ns |       2,205.9 |
| Allocate_ClusteredBitmap_with_ClusteredKeys       | 10000    | 816,041.304 ns |       1,225.4 |
| Allocate_NativeClusteredBitmap_with_ClusteredKeys | 10000    | 899,163.043 ns |       1,112.1 |
|                                                   |          |                |               |
| Remove_HashSet                                    | 10000    |       7.779 ns | 128,557,802.6 |
| Remove_Bitmap                                     | 10000    |       5.339 ns | 187,288,791.2 |
| Remove_ClusteredBitmap                            | 10000    |       5.763 ns | 173,517,390.3 |
| Remove_NativeClusteredBitmap                      | 10000    |       6.430 ns | 155,520,995.3 |
|                                                   |          |                |               |
| SymmetricExceptWith_HashSet_to_Span               | 10000    | 187,059.091 ns |       5,345.9 |
| SymmetricExceptWith_Bitmap_to_Span                | 10000    |   7,732.911 ns |     129,317.4 |
| SymmetricExceptWith_ClusteredBitmap_to_Span       | 10000    |  12,527.778 ns |      79,822.6 |
| SymmetricExceptWith_NativeClusteredBitmap_to_Span | 10000    |  22,913.483 ns |      43,642.4 |
| SymmetricExceptWith_HashSet_to_HashSet            | 10000    |  72,951.648 ns |      13,707.7 |
| SymmetricExceptWith_Bitmap_to_Bitmap              | 10000    |   3,700.000 ns |     270,270.3 |
|                                                   |          |                |               |
| UnionWith_HashSet_to_Span                         | 10000    | 119,576.744 ns |       8,362.8 |
| UnionWith_Bitmap_to_Span                          | 10000    |  10,777.907 ns |      92,782.4 |
| UnionWith_ClusteredBitmap_to_Span                 | 10000    |  16,315.730 ns |      61,290.5 |
| UnionWith_NativeClusteredBitmap_to_Span           | 10000    |  16,340.110 ns |      61,199.1 |
| UnionWith_HashSet_to_HashSet                      | 10000    | 104,676.829 ns |       9,553.2 |
| UnionWith_Bitmap_to_Bitmap                        | 10000    |   3,592.000 ns |     278,396.4 |
