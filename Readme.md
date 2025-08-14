# IntSet

IntSet is a high-performance integer set implementation for .NET using Bitmaps.
It is a replacement for `HashSet<int>` and `Dictionary<int, T>` when you need fast operations and predictable memory usage.


## Why IntSet?
- **Fastest possible operations**: Add, Contains, Remove, and set operations (Union, Intersect, Except, SymmetricExcept) are significantly faster than `HashSet<int>` for most workloads.
- **No allocations for set operations**: Most set operations do not allocate additional memory.



## Benchmarks
| Method                                         | N      | Mean           | Error         | StdDev        | Median         | Op/s        | Allocated |
|----------------------------------------------- |------- |---------------:|--------------:|--------------:|---------------:|------------:|----------:|
| MemoryUsed_Bitmap                              | 100    |     1,116.3 ns |      70.77 ns |     206.44 ns |     1,100.0 ns |   895,795.2 |     184 B |
| MemoryUsed_ClusteredBitmap                     | 100    |     1,145.0 ns |      75.51 ns |     222.64 ns |     1,100.0 ns |   873,362.4 |     192 B |
| MemoryUsed_HashSet                             | 100    |     3,262.1 ns |     221.49 ns |     635.51 ns |     3,300.0 ns |   306,550.5 |    1864 B |
| MemoryUsed_ClusteredKeys_Bitmap                | 100    |   420,588.8 ns |   4,835.51 ns |  13,399.17 ns |   419,200.0 ns |     2,377.6 | 2500264 B |
| MemoryUsed_ClusteredKeys_ClusteredBitmap       | 100    |   811,378.9 ns |  12,121.98 ns |  33,791.23 ns |   804,400.0 ns |     1,232.5 | 5000304 B |
| MemoryUsed_ClusteredKeys_HashSet               | 100    |     2,903.1 ns |     148.54 ns |     428.57 ns |     3,000.0 ns |   344,456.4 |    1864 B |
| MemoryUsed_NativeClusteredBitmap               | 100    |     1,764.2 ns |     117.47 ns |     337.04 ns |     1,700.0 ns |   566,825.8 |      48 B |
| MemoryUsed_ClusteredKeys_NativeClusteredBitmap | 100    |   858,043.2 ns |  13,730.12 ns |  39,394.29 ns |   845,900.0 ns |     1,165.4 | 2500104 B |
| Contains_Bitmap                                | 100    |     1,430.0 ns |      80.01 ns |     235.92 ns |     1,300.0 ns |   699,300.7 |         - |
| Contains_ClusteredBitmap                       | 100    |     1,693.9 ns |      86.70 ns |     254.28 ns |     1,600.0 ns |   590,339.9 |         - |
| Contains_HashSet                               | 100    |     1,658.8 ns |      86.85 ns |     251.97 ns |     1,600.0 ns |   602,858.9 |         - |
| Contains_NativeClusteredBitmap                 | 100    |     1,743.3 ns |      70.13 ns |     195.48 ns |     1,700.0 ns |   573,613.8 |         - |
| Add_Bitmap                                     | 100    |     1,731.6 ns |      30.24 ns |      76.96 ns |     1,700.0 ns |   577,507.6 |         - |
| Add_ClusteredBitmap                            | 100    |     2,167.7 ns |      91.88 ns |     269.48 ns |     2,100.0 ns |   461,323.4 |         - |
| Add_HashSet                                    | 100    |     1,641.2 ns |      98.47 ns |     285.68 ns |     1,500.0 ns |   609,296.5 |         - |
| Add_NativeClusteredBitmap                      | 100    |     1,883.7 ns |      76.04 ns |     206.86 ns |     1,800.0 ns |   530,864.2 |         - |
| Iterate_Bitmap                                 | 100    |     1,071.1 ns |      21.10 ns |      53.70 ns |     1,100.0 ns |   933,660.9 |         - |
| Iterate_ClusteredBitmap                        | 100    |     1,212.4 ns |      26.45 ns |      76.73 ns |     1,200.0 ns |   824,829.9 |         - |
| Iterate_HashSet                                | 100    |       706.0 ns |      31.91 ns |      94.09 ns |       700.0 ns | 1,416,430.6 |         - |
| Iterate_NativeClusteredBitmap                  | 100    |     1,188.7 ns |      24.31 ns |      70.53 ns |     1,200.0 ns |   841,283.6 |         - |
| Remove_Bitmap                                  | 100    |     1,920.7 ns |      73.93 ns |     202.39 ns |     1,900.0 ns |   520,646.3 |         - |
| Remove_ClusteredBitmap                         | 100    |     2,136.0 ns |      95.63 ns |     281.96 ns |     2,000.0 ns |   468,164.8 |         - |
| Remove_HashSet                                 | 100    |     1,653.5 ns |      91.17 ns |     267.38 ns |     1,500.0 ns |   604,764.8 |         - |
| Remove_NativeClusteredBitmap                   | 100    |     2,421.2 ns |     105.63 ns |     309.80 ns |     2,300.0 ns |   413,016.3 |         - |
| ExceptWith_Span_Bitmap                         | 100    |     1,861.5 ns |      42.91 ns |     110.76 ns |     1,900.0 ns |   537,190.1 |         - |
| ExceptWith_Span_ClusteredBitmap                | 100    |     2,066.7 ns |      38.32 ns |      98.91 ns |     2,100.0 ns |   483,871.0 |         - |
| ExceptWith_Bitmap_Bitmap                       | 100    |       236.8 ns |      30.53 ns |      87.58 ns |       200.0 ns | 4,222,222.2 |         - |
| ExceptWith_HashSet                             | 100    |     4,533.3 ns |     149.66 ns |     431.81 ns |     4,500.0 ns |   220,588.2 |      32 B |
| ExceptWith_Span_NativeClusteredBitmap          | 100    |     2,337.2 ns |      53.02 ns |     136.86 ns |     2,300.0 ns |   427,866.2 |         - |
| IntersectWith_Span_Bitmap                      | 100    |       279.3 ns |      14.43 ns |      40.70 ns |       300.0 ns | 3,579,766.5 |         - |
| IntersectWith_Span_ClusteredBitmap             | 100    |       345.4 ns |      24.89 ns |      72.20 ns |       300.0 ns | 2,895,522.4 |         - |
| IntersectWith_Bitmap_Bitmap                    | 100    |       305.0 ns |      24.28 ns |      71.60 ns |       300.0 ns | 3,278,688.5 |         - |
| IntersectWith_HashSet                          | 100    |     2,886.9 ns |     143.23 ns |     420.07 ns |     2,800.0 ns |   346,396.1 |      32 B |
| IntersectWith_Span_NativeClusteredBitmap       | 100    |       345.5 ns |      25.90 ns |      75.96 ns |       300.0 ns | 2,894,736.8 |         - |
| SymmetricExceptWith_Span_Bitmap                | 100    |       313.1 ns |      26.52 ns |      77.78 ns |       300.0 ns | 3,193,548.4 |         - |
| SymmetricExceptWith_Span_ClusteredBitmap       | 100    |       334.3 ns |      24.43 ns |      71.66 ns |       300.0 ns | 2,990,936.6 |         - |
| SymmetricExceptWith_Bitmap_Bitmap              | 100    |       325.0 ns |      28.00 ns |      80.79 ns |       300.0 ns | 3,076,923.1 |         - |
| SymmetricExceptWith_HashSet                    | 100    |     3,121.4 ns |      98.85 ns |     288.35 ns |     3,100.0 ns |   320,366.1 |      32 B |
| SymmetricExceptWith_Span_NativeClusteredBitmap | 100    |       437.8 ns |      23.87 ns |      69.63 ns |       400.0 ns | 2,284,382.3 |         - |
| UnionWith_Span_Bitmap                          | 100    |       361.1 ns |      25.57 ns |      73.36 ns |       400.0 ns | 2,769,679.3 |         - |
| UnionWith_Span_ClusteredBitmap                 | 100    |       417.2 ns |      27.14 ns |      79.59 ns |       400.0 ns | 2,397,094.4 |         - |
| UnionWith_Bitmap_Bitmap                        | 100    |       261.6 ns |      24.70 ns |      72.43 ns |       300.0 ns | 3,822,393.8 |         - |
| UnionWith_HashSet                              | 100    |     2,407.1 ns |     110.57 ns |     324.27 ns |     2,400.0 ns |   415,442.7 |      32 B |
| UnionWith_Span_NativeClusteredBitmap           | 100    |       494.9 ns |      34.91 ns |     102.39 ns |       500.0 ns | 2,020,408.2 |         - |
| MemoryUsed_Bitmap                              | 1000   |     2,322.6 ns |     135.74 ns |     385.08 ns |     2,300.0 ns |   430,555.6 |     184 B |
| MemoryUsed_ClusteredBitmap                     | 1000   |     2,708.3 ns |     100.79 ns |     290.79 ns |     2,550.0 ns |   369,230.8 |     472 B |
| MemoryUsed_HashSet                             | 1000   |    24,641.0 ns |   2,687.28 ns |   7,923.51 ns |    26,600.0 ns |    40,582.8 |   17800 B |
| MemoryUsed_ClusteredKeys_Bitmap                | 1000   |   420,598.9 ns |   8,059.27 ns |  22,731.31 ns |   415,900.0 ns |     2,377.6 | 2500488 B |
| MemoryUsed_ClusteredKeys_ClusteredBitmap       | 1000   |   806,972.0 ns |  16,798.92 ns |  47,655.73 ns |   785,200.0 ns |     1,239.2 | 5000752 B |
| MemoryUsed_ClusteredKeys_HashSet               | 1000   |    23,190.9 ns |   2,406.79 ns |   7,058.69 ns |    24,500.0 ns |    43,120.3 |   17800 B |
| MemoryUsed_NativeClusteredBitmap               | 1000   |     3,038.4 ns |      59.18 ns |     161.00 ns |     3,000.0 ns |   329,123.6 |      48 B |
| MemoryUsed_ClusteredKeys_NativeClusteredBitmap | 1000   |   844,288.8 ns |  14,650.96 ns |  40,597.75 ns |   831,400.0 ns |     1,184.4 | 2500328 B |
| Contains_Bitmap                                | 1000   |    14,129.5 ns |     434.45 ns |   1,246.52 ns |    13,900.0 ns |    70,774.0 |         - |
| Contains_ClusteredBitmap                       | 1000   |    15,525.5 ns |     455.64 ns |   1,299.95 ns |    15,300.0 ns |    64,410.0 |         - |
| Contains_HashSet                               | 1000   |    16,534.4 ns |     554.85 ns |   1,574.01 ns |    16,100.0 ns |    60,479.9 |         - |
| Contains_NativeClusteredBitmap                 | 1000   |    16,272.8 ns |     454.19 ns |   1,281.06 ns |    16,050.0 ns |    61,452.1 |         - |
| Add_Bitmap                                     | 1000   |    18,800.0 ns |     436.04 ns |   1,258.07 ns |    18,550.0 ns |    53,191.5 |         - |
| Add_ClusteredBitmap                            | 1000   |    20,890.5 ns |     633.02 ns |   1,816.24 ns |    20,300.0 ns |    47,868.6 |         - |
| Add_HashSet                                    | 1000   |    15,391.5 ns |     438.31 ns |   1,250.52 ns |    15,200.0 ns |    64,971.0 |         - |
| Add_NativeClusteredBitmap                      | 1000   |    19,443.0 ns |     444.41 ns |   1,260.73 ns |    19,300.0 ns |    51,432.4 |         - |
| Iterate_Bitmap                                 | 1000   |    10,615.2 ns |     486.95 ns |   1,373.45 ns |    10,200.0 ns |    94,204.4 |         - |
| Iterate_ClusteredBitmap                        | 1000   |    12,384.7 ns |     583.86 ns |   1,703.14 ns |    11,700.0 ns |    80,744.8 |         - |
| Iterate_HashSet                                | 1000   |     5,392.6 ns |     295.62 ns |     848.18 ns |     5,100.0 ns |   185,438.2 |         - |
| Iterate_NativeClusteredBitmap                  | 1000   |    11,950.0 ns |     515.53 ns |   1,437.09 ns |    11,600.0 ns |    83,682.0 |         - |
| Remove_Bitmap                                  | 1000   |    20,002.1 ns |     626.49 ns |   1,787.41 ns |    19,550.0 ns |    49,994.7 |         - |
| Remove_ClusteredBitmap                         | 1000   |    21,231.2 ns |     529.88 ns |   1,503.18 ns |    20,800.0 ns |    47,100.5 |         - |
| Remove_HashSet                                 | 1000   |    15,638.3 ns |     416.79 ns |   1,189.12 ns |    15,400.0 ns |    63,945.6 |         - |
| Remove_NativeClusteredBitmap                   | 1000   |    23,069.5 ns |     512.24 ns |   1,469.70 ns |    22,700.0 ns |    43,347.3 |         - |
| ExceptWith_Span_Bitmap                         | 1000   |    18,631.9 ns |     363.99 ns |   1,020.66 ns |    18,600.0 ns |    53,671.5 |         - |
| ExceptWith_Span_ClusteredBitmap                | 1000   |    20,813.4 ns |     492.25 ns |   1,428.11 ns |    20,400.0 ns |    48,046.0 |         - |
| ExceptWith_Bitmap_Bitmap                       | 1000   |       388.0 ns |      12.95 ns |      32.71 ns |       400.0 ns | 2,577,319.6 |         - |
| ExceptWith_HashSet                             | 1000   |    33,892.2 ns |     463.57 ns |   1,292.26 ns |    33,700.0 ns |    29,505.3 |      32 B |
| ExceptWith_Span_NativeClusteredBitmap          | 1000   |    22,398.9 ns |     371.70 ns |   1,054.44 ns |    22,200.0 ns |    44,645.0 |         - |
| IntersectWith_Span_Bitmap                      | 1000   |       983.0 ns |      22.78 ns |      64.99 ns |     1,000.0 ns | 1,017,316.0 |         - |
| IntersectWith_Span_ClusteredBitmap             | 1000   |     1,255.7 ns |      19.25 ns |      55.84 ns |     1,300.0 ns |   796,387.5 |         - |
| IntersectWith_Bitmap_Bitmap                    | 1000   |       435.7 ns |      21.60 ns |      63.00 ns |       400.0 ns | 2,295,082.0 |         - |
| IntersectWith_HashSet                          | 1000   |    16,645.6 ns |     269.96 ns |     752.55 ns |    16,400.0 ns |    60,076.1 |      32 B |
| IntersectWith_Span_NativeClusteredBitmap       | 1000   |     1,258.9 ns |      19.36 ns |      55.53 ns |     1,300.0 ns |   794,314.4 |         - |
| SymmetricExceptWith_Span_Bitmap                | 1000   |       940.2 ns |      21.49 ns |      62.35 ns |       900.0 ns | 1,063,596.5 |         - |
| SymmetricExceptWith_Span_ClusteredBitmap       | 1000   |     1,266.0 ns |      27.65 ns |      80.22 ns |     1,300.0 ns |   789,902.3 |         - |
| SymmetricExceptWith_Bitmap_Bitmap              | 1000   |       482.3 ns |      20.11 ns |      58.03 ns |       500.0 ns | 2,073,434.1 |         - |
| SymmetricExceptWith_HashSet                    | 1000   |    17,988.0 ns |     233.58 ns |     658.81 ns |    17,700.0 ns |    55,592.5 |      32 B |
| SymmetricExceptWith_Span_NativeClusteredBitmap | 1000   |     2,473.8 ns |      17.98 ns |      47.05 ns |     2,500.0 ns |   404,244.6 |         - |
| UnionWith_Span_Bitmap                          | 1000   |     1,370.1 ns |      17.97 ns |      46.07 ns |     1,400.0 ns |   729,857.8 |         - |
| UnionWith_Span_ClusteredBitmap                 | 1000   |     1,907.5 ns |      24.51 ns |      69.53 ns |     1,900.0 ns |   524,239.0 |         - |
| UnionWith_Bitmap_Bitmap                        | 1000   |       434.1 ns |      21.41 ns |      60.04 ns |       400.0 ns | 2,303,797.5 |         - |
| UnionWith_HashSet                              | 1000   |    12,545.3 ns |      49.37 ns |     134.30 ns |    12,500.0 ns |    79,710.8 |      32 B |
| UnionWith_Span_NativeClusteredBitmap           | 1000   |     1,915.1 ns |      29.83 ns |      84.63 ns |     1,900.0 ns |   522,178.6 |         - |
| MemoryUsed_Bitmap                              | 10000  |    11,561.8 ns |      45.36 ns |     125.69 ns |    11,600.0 ns |    86,491.7 |    1464 B |
| MemoryUsed_ClusteredBitmap                     | 10000  |    17,096.6 ns |      46.53 ns |     128.16 ns |    17,100.0 ns |    58,491.2 |    2720 B |
| MemoryUsed_HashSet                             | 10000  |   131,352.9 ns |     829.87 ns |   2,271.76 ns |   130,400.0 ns |     7,613.1 |  161800 B |
| MemoryUsed_ClusteredKeys_Bitmap                | 10000  |   411,406.5 ns |   5,500.48 ns |  15,514.20 ns |   409,500.0 ns |     2,430.7 | 2502744 B |
| MemoryUsed_ClusteredKeys_ClusteredBitmap       | 10000  |   879,585.6 ns |  14,372.17 ns |  40,063.88 ns |   866,850.0 ns |     1,136.9 | 5005248 B |
| MemoryUsed_ClusteredKeys_HashSet               | 10000  |   130,972.3 ns |     613.46 ns |   1,750.23 ns |   130,500.0 ns |     7,635.2 |  161800 B |
| MemoryUsed_NativeClusteredBitmap               | 10000  |    18,459.3 ns |     147.32 ns |     413.11 ns |    18,300.0 ns |    54,173.1 |      48 B |
| MemoryUsed_ClusteredKeys_NativeClusteredBitmap | 10000  |   830,388.2 ns |   7,253.31 ns |  19,609.75 ns |   830,700.0 ns |     1,204.3 | 2502576 B |
| Contains_Bitmap                                | 10000  |    19,109.6 ns |     252.07 ns |     719.18 ns |    18,900.0 ns |    52,329.8 |         - |
| Contains_ClusteredBitmap                       | 10000  |    22,008.4 ns |     272.62 ns |     782.20 ns |    21,800.0 ns |    45,437.2 |         - |
| Contains_HashSet                               | 10000  |   100,716.3 ns |     461.68 ns |   1,302.18 ns |   100,500.0 ns |     9,928.9 |         - |
| Contains_NativeClusteredBitmap                 | 10000  |    24,658.9 ns |     310.26 ns |     890.19 ns |    24,400.0 ns |    40,553.2 |         - |
| Add_Bitmap                                     | 10000  |    47,817.2 ns |     327.13 ns |     928.02 ns |    47,500.0 ns |    20,913.0 |         - |
| Add_ClusteredBitmap                            | 10000  |    54,822.8 ns |     302.63 ns |     853.58 ns |    54,700.0 ns |    18,240.6 |         - |
| Add_HashSet                                    | 10000  |    94,766.0 ns |     732.13 ns |   2,088.81 ns |    94,100.0 ns |    10,552.3 |         - |
| Add_NativeClusteredBitmap                      | 10000  |    56,022.5 ns |     491.69 ns |   1,362.46 ns |    55,600.0 ns |    17,850.0 |         - |
| Iterate_Bitmap                                 | 10000  |    23,958.9 ns |     518.24 ns |   1,486.93 ns |    23,700.0 ns |    41,738.1 |         - |
| Iterate_ClusteredBitmap                        | 10000  |    29,370.5 ns |     719.54 ns |   2,064.49 ns |    28,900.0 ns |    34,047.7 |         - |
| Iterate_HashSet                                | 10000  |    15,304.3 ns |     321.07 ns |     916.04 ns |    15,100.0 ns |    65,341.3 |         - |
| Iterate_NativeClusteredBitmap                  | 10000  |    28,964.5 ns |     535.78 ns |   1,519.91 ns |    28,700.0 ns |    34,525.0 |         - |
| Remove_Bitmap                                  | 10000  |    54,912.0 ns |     324.13 ns |     914.22 ns |    54,800.0 ns |    18,211.0 |         - |
| Remove_ClusteredBitmap                         | 10000  |    60,025.5 ns |     359.94 ns |   1,026.94 ns |    59,750.0 ns |    16,659.6 |         - |
| Remove_HashSet                                 | 10000  |    77,663.0 ns |     547.79 ns |   1,545.05 ns |    77,350.0 ns |    12,876.1 |         - |
| Remove_NativeClusteredBitmap                   | 10000  |    66,107.4 ns |     467.55 ns |   1,333.94 ns |    65,700.0 ns |    15,126.9 |         - |
| ExceptWith_Span_Bitmap                         | 10000  |    52,040.0 ns |     451.96 ns |   1,296.76 ns |    51,700.0 ns |    19,216.0 |         - |
| ExceptWith_Span_ClusteredBitmap                | 10000  |    57,337.5 ns |     486.15 ns |   1,402.65 ns |    56,800.0 ns |    17,440.6 |         - |
| ExceptWith_Bitmap_Bitmap                       | 10000  |     3,637.0 ns |     380.60 ns |   1,122.20 ns |     4,450.0 ns |   274,951.9 |         - |
| ExceptWith_HashSet                             | 10000  |   115,806.5 ns |     591.84 ns |   1,678.95 ns |   115,600.0 ns |     8,635.1 |      32 B |
| ExceptWith_Span_NativeClusteredBitmap          | 10000  |    59,947.4 ns |     531.88 ns |   1,543.07 ns |    59,400.0 ns |    16,681.3 |         - |
| IntersectWith_Span_Bitmap                      | 10000  |     7,127.0 ns |      24.19 ns |      67.02 ns |     7,100.0 ns |   140,312.2 |         - |
| IntersectWith_Span_ClusteredBitmap             | 10000  |    10,279.2 ns |      24.59 ns |      70.96 ns |    10,300.0 ns |    97,284.2 |         - |
| IntersectWith_Bitmap_Bitmap                    | 10000  |     3,714.0 ns |     402.45 ns |   1,186.63 ns |     4,450.0 ns |   269,251.5 |         - |
| IntersectWith_HashSet                          | 10000  |   163,562.2 ns |     784.18 ns |   2,185.99 ns |   163,100.0 ns |     6,113.9 |     840 B |
| IntersectWith_Span_NativeClusteredBitmap       | 10000  |    10,348.3 ns |      25.00 ns |      69.27 ns |    10,400.0 ns |    96,634.1 |         - |
| SymmetricExceptWith_Span_Bitmap                | 10000  |     7,647.1 ns |      26.52 ns |      71.69 ns |     7,600.0 ns |   130,769.2 |         - |
| SymmetricExceptWith_Span_ClusteredBitmap       | 10000  |    10,302.3 ns |      62.77 ns |     171.84 ns |    10,300.0 ns |    97,065.7 |         - |
| SymmetricExceptWith_Bitmap_Bitmap              | 10000  |     3,713.0 ns |     395.65 ns |   1,166.59 ns |     4,500.0 ns |   269,324.0 |         - |
| SymmetricExceptWith_HashSet                    | 10000  |   188,967.7 ns |   1,031.50 ns |   2,926.20 ns |   188,600.0 ns |     5,291.9 |    1648 B |
| SymmetricExceptWith_Span_NativeClusteredBitmap | 10000  |    22,925.5 ns |      30.81 ns |      87.91 ns |    22,900.0 ns |    43,619.5 |         - |
| UnionWith_Span_Bitmap                          | 10000  |    10,871.1 ns |      25.36 ns |      70.70 ns |    10,900.0 ns |    91,986.9 |         - |
| UnionWith_Span_ClusteredBitmap                 | 10000  |    17,371.2 ns |      21.23 ns |      55.56 ns |    17,400.0 ns |    57,566.4 |         - |
| UnionWith_Bitmap_Bitmap                        | 10000  |     3,565.0 ns |     366.05 ns |   1,079.32 ns |     4,400.0 ns |   280,504.9 |         - |
| UnionWith_HashSet                              | 10000  |   120,377.5 ns |     354.28 ns |     927.09 ns |   120,250.0 ns |     8,307.2 |      32 B |
| UnionWith_Span_NativeClusteredBitmap           | 10000  |    16,413.7 ns |      31.18 ns |      89.46 ns |    16,400.0 ns |    60,924.8 |         - |
| MemoryUsed_Bitmap                              | 100000 |   111,302.2 ns |     952.52 ns |   2,655.26 ns |   111,450.0 ns |     8,984.5 |   12712 B |
| MemoryUsed_ClusteredBitmap                     | 100000 |   172,494.6 ns |   1,124.62 ns |   3,172.01 ns |   172,250.0 ns |     5,797.3 |   25216 B |
| MemoryUsed_HashSet                             | 100000 | 2,182,017.0 ns | 260,152.32 ns | 767,065.05 ns | 1,777,250.0 ns |       458.3 | 1738248 B |
| MemoryUsed_ClusteredKeys_Bitmap                | 100000 |   467,921.0 ns |  35,428.14 ns | 104,460.67 ns |   508,950.0 ns |     2,137.1 | 2525240 B |
| MemoryUsed_ClusteredKeys_ClusteredBitmap       | 100000 | 1,010,484.7 ns |  33,172.06 ns |  96,764.45 ns | 1,037,750.0 ns |       989.6 | 5050240 B |
| MemoryUsed_ClusteredKeys_HashSet               | 100000 | 2,174,031.0 ns | 294,202.84 ns | 867,463.77 ns | 1,779,600.0 ns |       460.0 | 1738248 B |
| MemoryUsed_NativeClusteredBitmap               | 100000 |   168,592.9 ns |   1,004.32 ns |   2,698.05 ns |   167,800.0 ns |     5,931.4 |      48 B |
| MemoryUsed_ClusteredKeys_NativeClusteredBitmap | 100000 |   987,181.6 ns |  30,095.01 ns |  87,788.55 ns | 1,008,950.0 ns |     1,013.0 | 2525072 B |
| Contains_Bitmap                                | 100000 |    78,381.5 ns |   1,810.65 ns |   5,106.96 ns |    77,550.0 ns |    12,758.1 |         - |
| Contains_ClusteredBitmap                       | 100000 |    92,001.0 ns |   2,088.49 ns |   6,059.08 ns |    91,700.0 ns |    10,869.4 |         - |
| Contains_HashSet                               | 100000 | 1,145,471.8 ns |  36,361.51 ns |  98,305.47 ns | 1,171,600.0 ns |       873.0 |         - |
| Contains_NativeClusteredBitmap                 | 100000 |   113,858.7 ns |   1,319.65 ns |   3,722.11 ns |   113,450.0 ns |     8,782.8 |         - |
| Add_Bitmap                                     | 100000 |   345,531.9 ns |   2,424.80 ns |   6,799.41 ns |   344,900.0 ns |     2,894.1 |         - |
| Add_ClusteredBitmap                            | 100000 |   394,193.3 ns |   2,566.45 ns |   7,154.24 ns |   393,200.0 ns |     2,536.8 |         - |
| Add_HashSet                                    | 100000 | 1,152,765.5 ns |  43,800.72 ns | 119,903.77 ns | 1,100,500.0 ns |       867.5 |         - |
| Add_NativeClusteredBitmap                      | 100000 |   420,110.7 ns |   2,362.18 ns |   6,345.86 ns |   419,250.0 ns |     2,380.3 |         - |
| Iterate_Bitmap                                 | 100000 |   104,021.3 ns |   1,154.90 ns |   3,294.99 ns |   103,200.0 ns |     9,613.4 |         - |
| Iterate_ClusteredBitmap                        | 100000 |   135,100.0 ns |   2,652.02 ns |   7,693.98 ns |   137,000.0 ns |     7,401.9 |         - |
| Iterate_HashSet                                | 100000 |    99,548.9 ns |   1,319.64 ns |   3,765.00 ns |    98,400.0 ns |    10,045.3 |         - |
| Iterate_NativeClusteredBitmap                  | 100000 |   138,209.8 ns |   1,065.49 ns |   3,005.24 ns |   138,000.0 ns |     7,235.4 |         - |
| Remove_Bitmap                                  | 100000 |   411,142.2 ns |   2,091.95 ns |   5,831.53 ns |   410,400.0 ns |     2,432.2 |         - |
| Remove_ClusteredBitmap                         | 100000 |   452,871.3 ns |   2,273.71 ns |   6,487.04 ns |   451,000.0 ns |     2,208.1 |         - |
| Remove_HashSet                                 | 100000 |   840,382.1 ns |  18,251.08 ns |  49,030.35 ns |   819,150.0 ns |     1,189.9 |         - |
| Remove_NativeClusteredBitmap                   | 100000 |   492,207.0 ns |   3,940.84 ns |  10,721.33 ns |   490,900.0 ns |     2,031.7 |         - |
| ExceptWith_Span_Bitmap                         | 100000 |   381,364.5 ns |   2,500.99 ns |   7,094.88 ns |   380,500.0 ns |     2,622.2 |         - |
| ExceptWith_Span_ClusteredBitmap                | 100000 |   426,043.5 ns |   2,300.63 ns |   6,488.96 ns |   426,050.0 ns |     2,347.2 |         - |
| ExceptWith_Bitmap_Bitmap                       | 100000 |    12,689.4 ns |     663.81 ns |   1,794.64 ns |    12,600.0 ns |    78,805.9 |         - |
| ExceptWith_HashSet                             | 100000 | 1,255,797.8 ns |  39,270.20 ns | 110,117.86 ns | 1,205,500.0 ns |       796.3 |      32 B |
| ExceptWith_Span_NativeClusteredBitmap          | 100000 |   432,457.9 ns |   5,568.32 ns |  15,976.56 ns |   437,700.0 ns |     2,312.4 |         - |
| IntersectWith_Span_Bitmap                      | 100000 |    68,651.8 ns |     247.24 ns |     668.44 ns |    68,500.0 ns |    14,566.3 |         - |
| IntersectWith_Span_ClusteredBitmap             | 100000 |   103,675.3 ns |     242.34 ns |     655.18 ns |   103,700.0 ns |     9,645.5 |         - |
| IntersectWith_Bitmap_Bitmap                    | 100000 |    12,128.3 ns |     704.66 ns |   1,987.51 ns |    11,750.0 ns |    82,452.1 |         - |
| IntersectWith_HashSet                          | 100000 | 1,964,598.8 ns |  56,789.07 ns | 154,498.87 ns | 1,915,550.0 ns |       509.0 |    7960 B |
| IntersectWith_Span_NativeClusteredBitmap       | 100000 |   104,158.4 ns |     652.15 ns |   1,807.11 ns |   103,500.0 ns |     9,600.8 |         - |
| SymmetricExceptWith_Span_Bitmap                | 100000 |    74,327.9 ns |     232.24 ns |     631.83 ns |    74,100.0 ns |    13,453.9 |         - |
| SymmetricExceptWith_Span_ClusteredBitmap       | 100000 |   104,320.0 ns |     762.98 ns |   2,126.89 ns |   103,800.0 ns |     9,585.9 |         - |
| SymmetricExceptWith_Bitmap_Bitmap              | 100000 |    12,004.7 ns |     760.68 ns |   2,069.49 ns |    11,700.0 ns |    83,301.0 |         - |
| SymmetricExceptWith_HashSet                    | 100000 | 2,531,408.0 ns |  91,873.43 ns | 251,502.06 ns | 2,440,900.0 ns |       395.0 |   15888 B |
| SymmetricExceptWith_Span_NativeClusteredBitmap | 100000 |   229,526.9 ns |   1,511.41 ns |   3,901.44 ns |   228,300.0 ns |     4,356.8 |         - |
| UnionWith_Span_Bitmap                          | 100000 |   106,868.6 ns |     675.94 ns |   1,838.94 ns |   106,200.0 ns |     9,357.3 |         - |
| UnionWith_Span_ClusteredBitmap                 | 100000 |   175,253.3 ns |   1,009.42 ns |   2,813.86 ns |   174,300.0 ns |     5,706.0 |         - |
| UnionWith_Bitmap_Bitmap                        | 100000 |    11,994.7 ns |     817.17 ns |   2,331.43 ns |    11,250.0 ns |    83,370.3 |         - |
| UnionWith_HashSet                              | 100000 | 2,124,316.0 ns | 258,382.26 ns | 761,845.99 ns | 1,661,800.0 ns |       470.7 |      32 B |
| UnionWith_Span_NativeClusteredBitmap           | 100000 |   166,210.0 ns |     903.12 ns |   2,363.31 ns |   165,200.0 ns |     6,016.5 |         - |
