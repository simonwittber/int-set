# IntSet

IntSet is a high-performance integer set implementation for .NET using Bitmaps.
It is a replacement for `HashSet<int>` and `Dictionary<int, T>` when you need fast operations and predictable memory usage.


## Why IntSet?
- **Fastest possible operations**: Add, Contains, Remove, and set operations (Union, Intersect, Except, SymmetricExcept) are significantly faster than `HashSet<int>` for most workloads.
- **No allocations for set operations**: Most set operations do not allocate additional memory.



## Benchmarks

| Method                                            | KeyCount | Mean           | Op/s          | Ratio | Alloc Ratio |
|-------------------------------------------------- |--------- |---------------:|--------------:|------:|------------:|
| **Add_Bitmap**                                        | **10000**    |       **5.050 ns** | **198,028,516.1** |  **0.53** |          **NA** |
| **Add_ClusteredBitmap**                               | **10000**    |       **5.519 ns** | **181,207,293.6** |  **0.58** |          **NA** |
| **Add_HashSet**                                       | **10000**    |       **9.498 ns** | **105,289,546.3** |  **1.00** |          **NA** |
| **Add_NativeClusteredBitmap**                         | **10000**    |       **5.419 ns** | **184,534,100.0** |  **0.57** |          **NA** |
|                                                   |          |                |               |       |             |
| **Contains_Bitmap**                                   | **10000**    |       **2.051 ns** | **487,498,034.3** |  **0.21** |          **NA** |
| **Contains_ClusteredBitmap**                          | **10000**    |       **2.389 ns** | **418,647,246.1** |  **0.25** |          **NA** |
| **Contains_HashSet**                                  | **10000**    |       **9.723 ns** | **102,846,604.9** |  **1.00** |          **NA** |
| **Contains_NativeClusteredBitmap**                    | **10000**    |       **2.379 ns** | **420,263,003.3** |  **0.24** |          **NA** |
|                                                   |          |                |               |       |             |
| **ExceptWith_Bitmap_to_Bitmap**                       | **10000**    |   **3,641.000 ns** |     **274,649.8** |  **0.03** |        **0.00** |
| **ExceptWith_Bitmap_to_Span**                         | **10000**    |  **53,975.000 ns** |      **18,527.1** |  **0.47** |        **0.00** |
| **ExceptWith_ClusteredBitmap_to_Span**                | **10000**    |  **58,544.681 ns** |      **17,081.0** |  **0.51** |        **0.00** |
| **ExceptWith_HashSet_to_HashSet**                     | **10000**    | **100,824.211 ns** |       **9,918.3** |  **0.87** |        **1.25** |
| **ExceptWith_HashSet_to_Span**                        | **10000**    | **115,496.429 ns** |       **8,658.3** |  **1.00** |        **1.00** |
| **ExceptWith_NativeClusteredBitmap_to_Span**          | **10000**    |  **60,523.913 ns** |      **16,522.4** |  **0.52** |        **0.00** |
|                                                   |          |                |               |       |             |
| **IntersectWith_Bitmap_to_Bitmap**                    | **10000**    |   **3,772.000 ns** |     **265,111.3** |  **0.02** |        **0.00** |
| **IntersectWith_Bitmap_to_Span**                      | **10000**    |   **7,079.381 ns** |     **141,255.3** |  **0.04** |        **0.00** |
| **IntersectWith_ClusteredBitmap_to_Span**             | **10000**    |  **10,227.083 ns** |      **97,779.6** |  **0.06** |        **0.00** |
| **IntersectWith_HashSet_to_HashSet**                  | **10000**    |  **82,796.809 ns** |      **12,077.8** |  **0.50** |        **0.00** |
| **IntersectWith_HashSet_to_Span**                     | **10000**    | **165,010.465 ns** |       **6,060.2** |  **1.00** |        **1.00** |
| **IntersectWith_NativeClusteredBitmap_to_Span**       | **10000**    |  **10,279.747 ns** |      **97,278.7** |  **0.06** |        **0.00** |
|                                                   |          |                |               |       |             |
| **Iterate_Bitmap**                                    | **10000**    |  **24,257.732 ns** |      **41,224.0** |  **1.61** |          **NA** |
| **Iterate_ClusteredBitmap**                           | **10000**    |  **28,581.053 ns** |      **34,988.2** |  **1.90** |          **NA** |
| **Iterate_HashSet**                                   | **10000**    |  **15,120.879 ns** |      **66,133.7** |  **1.00** |          **NA** |
| **Iterate_NativeClusteredBitmap**                     | **10000**    |  **28,958.947 ns** |      **34,531.6** |  **1.92** |          **NA** |
|                                                   |          |                |               |       |             |
| **Allocate_Bitmap_with_ClusteredKeys**                | **10000**    | **424,880.928 ns** |       **2,353.6** |  **3.26** |      **15.468** |
| **Allocate_Bitmap_with_RandomKeys**                   | **10000**    |  **11,682.418 ns** |      **85,598.7** |  **0.09** |       **0.009** |
| **Allocate_ClusteredBitmap_with_ClusteredKeys**       | **10000**    | **813,018.280 ns** |       **1,230.0** |  **6.23** |      **30.935** |
| **Allocate_ClusteredBitmap_with_RandomKeys**          | **10000**    |  **18,000.000 ns** |      **55,555.6** |  **0.14** |       **0.017** |
| **Allocate_HashSet**                                  | **10000**    | **130,507.143 ns** |       **7,662.4** |  **1.00** |       **1.000** |
| **Allocate_HashSet_with_ClusteredKeys**               | **10000**    | **131,197.849 ns** |       **7,622.1** |  **1.01** |       **1.000** |
| **Allocate_NativeClusteredBitmap_with_ClusteredKeys** | **10000**    | **894,182.105 ns** |       **1,118.3** |  **6.85** |      **15.467** |
| **Allocate_NativeClusteredBitmap_with_RandomKeys**    | **10000**    |  **18,024.699 ns** |      **55,479.4** |  **0.14** |       **0.000** |
|                                                   |          |                |               |       |             |
| **Remove_Bitmap**                                     | **10000**    |       **5.331 ns** | **187,583,564.5** |  **0.69** |          **NA** |
| **Remove_ClusteredBitmap**                            | **10000**    |       **5.834 ns** | **171,414,730.5** |  **0.75** |          **NA** |
| **Remove_HashSet**                                    | **10000**    |       **7.785 ns** | **128,449,431.9** |  **1.00** |          **NA** |
| **Remove_NativeClusteredBitmap**                      | **10000**    |       **6.513 ns** | **153,548,962.3** |  **0.84** |          **NA** |
|                                                   |          |                |               |       |             |
| **SymmetricExceptWith_Bitmap_to_Bitmap**              | **10000**    |   **3,663.000 ns** |     **273,000.3** |  **0.02** |        **0.00** |
| **SymmetricExceptWith_Bitmap_to_Span**                | **10000**    |   **7,569.333 ns** |     **132,112.0** |  **0.04** |        **0.00** |
| **SymmetricExceptWith_ClusteredBitmap_to_Span**       | **10000**    |  **12,520.482 ns** |      **79,869.1** |  **0.07** |        **0.00** |
| **SymmetricExceptWith_HashSet_to_HashSet**            | **10000**    |  **72,545.977 ns** |      **13,784.4** |  **0.39** |        **0.00** |
| **SymmetricExceptWith_HashSet_to_Span**               | **10000**    | **186,297.727 ns** |       **5,367.8** |  **1.00** |        **1.00** |
| **SymmetricExceptWith_NativeClusteredBitmap_to_Span** | **10000**    |  **22,725.806 ns** |      **44,002.8** |  **0.12** |        **0.00** |
|                                                   |          |                |               |       |             |
| **UnionWith_Bitmap_to_Bitmap**                        | **10000**    |   **3,697.000 ns** |     **270,489.6** |  **0.03** |        **0.00** |
| **UnionWith_Bitmap_to_Span**                          | **10000**    |  **10,791.011 ns** |      **92,669.7** |  **0.09** |        **0.00** |
| **UnionWith_ClusteredBitmap_to_Span**                 | **10000**    |  **16,391.209 ns** |      **61,008.3** |  **0.14** |        **0.00** |
| **UnionWith_HashSet_to_HashSet**                      | **10000**    | **104,173.256 ns** |       **9,599.4** |  **0.87** |        **1.25** |
| **UnionWith_HashSet_to_Span**                         | **10000**    | **120,078.049 ns** |       **8,327.9** |  **1.00** |        **1.00** |
| **UnionWith_NativeClusteredBitmap_to_Span**           | **10000**    |  **16,441.379 ns** |      **60,822.1** |  **0.14** |        **0.00** |
