# IntSet

IntSet is a high-performance integer set implementation for .NET using Bitmaps.
It is a replacement for `HashSet<int>` and `Dictionary<int, T>` when you need fast operations and predictable memory usage.


## Why IntSet?
- **Fastest possible operations**: Add, Contains, Remove, and set operations (Union, Intersect, Except, SymmetricExcept) are significantly faster than `HashSet<int>` for most workloads.
- **No allocations for set operations**: Most set operations do not allocate additional memory.



## Benchmarks

| Method                                         | N      | Mean           | Error         | StdDev        | Median         | Op/s        | Allocated |
|----------------------------------------------- |------- |---------------:|--------------:|--------------:|---------------:|------------:|----------:|
| MemoryUsed_Bitmap                              | 100    |     3,109.8 ns |     105.50 ns |     306.07 ns |     3,150.0 ns |   321,564.7 |     184 B |
| MemoryUsed_ClusteredBitmap                     | 100    |     3,464.3 ns |     131.19 ns |     382.68 ns |     3,450.0 ns |   288,659.8 |     192 B |
| MemoryUsed_HashSet                             | 100    |     3,025.0 ns |     162.57 ns |     469.04 ns |     3,050.0 ns |   330,578.5 |    1864 B |
| MemoryUsed_ClusteredKeys_Bitmap                | 100    |    13,858.6 ns |   1,433.10 ns |   4,203.04 ns |    11,900.0 ns |    72,157.4 | 1250224 B |
| MemoryUsed_ClusteredKeys_ClusteredBitmap       | 100    |     3,389.6 ns |     157.23 ns |     453.64 ns |     3,500.0 ns |   295,021.5 |     192 B |
| MemoryUsed_ClusteredKeys_HashSet               | 100    |     2,929.6 ns |     172.30 ns |     502.61 ns |     2,900.0 ns |   341,344.5 |    1864 B |
| MemoryUsed_NativeClusteredBitmap               | 100    |     3,972.2 ns |     182.71 ns |     530.08 ns |     4,000.0 ns |   251,751.9 |      48 B |
| MemoryUsed_ClusteredKeys_NativeClusteredBitmap | 100    |     3,941.7 ns |     179.76 ns |     518.64 ns |     3,950.0 ns |   253,699.8 |      48 B |
| Contains_Bitmap                                | 100    |     1,501.0 ns |     101.06 ns |     294.80 ns |     1,400.0 ns |   666,213.5 |         - |
| Contains_ClusteredBitmap                       | 100    |     1,940.4 ns |     139.48 ns |     409.06 ns |     1,800.0 ns |   515,356.6 |         - |
| Contains_HashSet                               | 100    |     1,729.0 ns |      99.76 ns |     294.15 ns |     1,650.0 ns |   578,369.0 |         - |
| Contains_NativeClusteredBitmap                 | 100    |     1,913.1 ns |     112.94 ns |     331.25 ns |     1,800.0 ns |   522,703.3 |         - |
| Add_Bitmap                                     | 100    |     1,882.8 ns |      91.65 ns |     268.80 ns |     1,800.0 ns |   531,115.9 |         - |
| Add_ClusteredBitmap                            | 100    |     2,155.0 ns |     103.21 ns |     304.30 ns |     2,050.0 ns |   464,037.1 |         - |
| Add_HashSet                                    | 100    |     1,686.7 ns |      96.92 ns |     282.71 ns |     1,600.0 ns |   592,861.5 |         - |
| Add_NativeClusteredBitmap                      | 100    |     2,085.0 ns |      99.77 ns |     294.18 ns |     2,000.0 ns |   479,616.3 |         - |
| Iterate_Bitmap                                 | 100    |     1,138.5 ns |      27.25 ns |      78.63 ns |     1,100.0 ns |   878,316.6 |         - |
| Iterate_ClusteredBitmap                        | 100    |     1,236.8 ns |      37.83 ns |     103.56 ns |     1,300.0 ns |   808,550.2 |         - |
| Iterate_HashSet                                | 100    |       705.1 ns |      42.74 ns |     124.66 ns |       700.0 ns | 1,418,234.4 |         - |
| Iterate_NativeClusteredBitmap                  | 100    |     1,242.6 ns |      27.43 ns |      78.27 ns |     1,300.0 ns |   804,794.5 |         - |
| Remove_Bitmap                                  | 100    |     2,047.5 ns |     101.28 ns |     297.04 ns |     1,900.0 ns |   488,406.5 |         - |
| Remove_ClusteredBitmap                         | 100    |     2,218.2 ns |     109.07 ns |     319.87 ns |     2,100.0 ns |   450,819.7 |         - |
| Remove_HashSet                                 | 100    |     1,676.5 ns |     101.34 ns |     295.60 ns |     1,600.0 ns |   596,469.9 |         - |
| Remove_NativeClusteredBitmap                   | 100    |     2,430.9 ns |      83.45 ns |     242.11 ns |     2,400.0 ns |   411,365.6 |         - |
| ExceptWith_Span_Bitmap                         | 100    |     2,102.0 ns |      94.30 ns |     275.07 ns |     2,000.0 ns |   475,728.2 |         - |
| ExceptWith_Span_ClusteredBitmap                | 100    |     2,126.6 ns |      51.44 ns |     133.69 ns |     2,100.0 ns |   470,238.1 |         - |
| ExceptWith_Bitmap_Bitmap                       | 100    |       304.0 ns |      34.42 ns |     100.93 ns |       300.0 ns | 3,289,036.5 |         - |
| ExceptWith_HashSet                             | 100    |     4,501.0 ns |     140.29 ns |     411.44 ns |     4,400.0 ns |   222,172.4 |      32 B |
| ExceptWith_Span_NativeClusteredBitmap          | 100    |     2,361.2 ns |      52.83 ns |     138.25 ns |     2,400.0 ns |   423,504.5 |         - |
| IntersectWith_Span_Bitmap                      | 100    |       276.0 ns |      24.51 ns |      70.70 ns |       300.0 ns | 3,622,641.5 |         - |
| IntersectWith_Span_ClusteredBitmap             | 100    |       277.3 ns |      16.68 ns |      42.15 ns |       300.0 ns | 3,605,769.2 |         - |
| IntersectWith_Bitmap_Bitmap                    | 100    |       249.5 ns |      27.79 ns |      79.73 ns |       200.0 ns | 4,008,438.8 |         - |
| IntersectWith_HashSet                          | 100    |     3,320.8 ns |     190.03 ns |     548.28 ns |     3,200.0 ns |   301,129.2 |      32 B |
| IntersectWith_Span_NativeClusteredBitmap       | 100    |       362.2 ns |      28.50 ns |      83.13 ns |       400.0 ns | 2,760,563.4 |         - |
| SymmetricExceptWith_Span_Bitmap                | 100    |       310.0 ns |      31.79 ns |      93.74 ns |       300.0 ns | 3,225,806.5 |         - |
| SymmetricExceptWith_Span_ClusteredBitmap       | 100    |       367.0 ns |      22.09 ns |      64.10 ns |       400.0 ns | 2,724,719.1 |         - |
| SymmetricExceptWith_Bitmap_Bitmap              | 100    |       395.9 ns |      50.66 ns |     147.78 ns |       400.0 ns | 2,525,773.2 |         - |
| SymmetricExceptWith_HashSet                    | 100    |     2,942.4 ns |     128.51 ns |     376.91 ns |     2,900.0 ns |   339,855.8 |      32 B |
| SymmetricExceptWith_Span_NativeClusteredBitmap | 100    |       409.0 ns |      32.01 ns |      94.38 ns |       400.0 ns | 2,444,987.8 |         - |
| UnionWith_Span_Bitmap                          | 100    |     2,138.4 ns |      87.09 ns |     255.43 ns |     2,200.0 ns |   467,642.9 |         - |
| UnionWith_Span_ClusteredBitmap                 | 100    |     2,276.0 ns |      96.06 ns |     283.24 ns |     2,400.0 ns |   439,367.3 |         - |
| UnionWith_Bitmap_Bitmap                        | 100    |       234.0 ns |      27.20 ns |      78.91 ns |       200.0 ns | 4,273,127.8 |         - |
| UnionWith_HashSet                              | 100    |     2,328.4 ns |      95.20 ns |     273.15 ns |     2,300.0 ns |   429,475.6 |      32 B |
| UnionWith_Span_NativeClusteredBitmap           | 100    |     2,154.0 ns |      96.70 ns |     285.13 ns |     2,200.0 ns |   464,252.6 |         - |
| MemoryUsed_Bitmap                              | 1000   |    21,141.8 ns |     232.58 ns |     652.19 ns |    20,900.0 ns |    47,299.8 |     184 B |
| MemoryUsed_ClusteredBitmap                     | 1000   |    23,225.6 ns |     290.67 ns |     810.27 ns |    22,900.0 ns |    43,056.0 |     472 B |
| MemoryUsed_HashSet                             | 1000   |    21,784.8 ns |   1,941.04 ns |   5,692.73 ns |    24,200.0 ns |    45,903.5 |   17800 B |
| MemoryUsed_ClusteredKeys_Bitmap                | 1000   |   268,064.9 ns |   7,840.29 ns |  22,368.79 ns |   257,800.0 ns |     3,730.4 | 3750544 B |
| MemoryUsed_ClusteredKeys_ClusteredBitmap       | 1000   |    23,438.7 ns |     269.67 ns |     765.00 ns |    23,100.0 ns |    42,664.5 |     472 B |
| MemoryUsed_ClusteredKeys_HashSet               | 1000   |    25,545.9 ns |   2,983.68 ns |   8,703.53 ns |    26,800.0 ns |    39,145.2 |   17800 B |
| MemoryUsed_NativeClusteredBitmap               | 1000   |    25,080.4 ns |   1,199.88 ns |   3,481.07 ns |    23,500.0 ns |    39,871.8 |      48 B |
| MemoryUsed_ClusteredKeys_NativeClusteredBitmap | 1000   |    22,231.5 ns |     294.07 ns |     814.86 ns |    22,000.0 ns |    44,981.3 |      48 B |
| Contains_Bitmap                                | 1000   |    14,178.6 ns |     498.71 ns |   1,454.76 ns |    13,800.0 ns |    70,529.0 |         - |
| Contains_ClusteredBitmap                       | 1000   |    15,371.4 ns |     451.63 ns |   1,266.43 ns |    15,100.0 ns |    65,055.8 |         - |
| Contains_HashSet                               | 1000   |    16,338.0 ns |     407.61 ns |   1,149.66 ns |    16,100.0 ns |    61,206.8 |         - |
| Contains_NativeClusteredBitmap                 | 1000   |    16,520.4 ns |     455.02 ns |   1,290.82 ns |    16,300.0 ns |    60,531.1 |         - |
| Add_Bitmap                                     | 1000   |    18,693.8 ns |     486.16 ns |   1,402.69 ns |    18,400.0 ns |    53,493.8 |         - |
| Add_ClusteredBitmap                            | 1000   |    20,505.1 ns |     507.50 ns |   1,480.41 ns |    20,100.0 ns |    48,768.4 |         - |
| Add_HashSet                                    | 1000   |    16,343.2 ns |     586.36 ns |   1,682.36 ns |    15,900.0 ns |    61,187.7 |         - |
| Add_NativeClusteredBitmap                      | 1000   |    19,159.1 ns |     469.96 ns |   1,333.21 ns |    18,900.0 ns |    52,194.4 |         - |
| Iterate_Bitmap                                 | 1000   |    10,948.4 ns |     534.83 ns |   1,499.73 ns |    10,400.0 ns |    91,338.0 |         - |
| Iterate_ClusteredBitmap                        | 1000   |    11,779.8 ns |     475.82 ns |   1,318.50 ns |    11,500.0 ns |    84,891.3 |         - |
| Iterate_HashSet                                | 1000   |     5,583.0 ns |     323.01 ns |     952.41 ns |     5,200.0 ns |   179,115.2 |         - |
| Iterate_NativeClusteredBitmap                  | 1000   |    12,328.3 ns |     604.73 ns |   1,773.56 ns |    11,600.0 ns |    81,114.3 |         - |
| Remove_Bitmap                                  | 1000   |    19,657.8 ns |     465.71 ns |   1,298.23 ns |    19,300.0 ns |    50,870.4 |         - |
| Remove_ClusteredBitmap                         | 1000   |    21,568.5 ns |     611.82 ns |   1,695.35 ns |    21,100.0 ns |    46,363.8 |         - |
| Remove_HashSet                                 | 1000   |    20,936.2 ns |     850.59 ns |   2,426.77 ns |    21,700.0 ns |    47,764.2 |         - |
| Remove_NativeClusteredBitmap                   | 1000   |    28,018.2 ns |   1,424.23 ns |   4,177.01 ns |    27,000.0 ns |    35,691.1 |         - |
| ExceptWith_Span_Bitmap                         | 1000   |    19,401.1 ns |     517.84 ns |   1,460.58 ns |    18,950.0 ns |    51,543.5 |         - |
| ExceptWith_Span_ClusteredBitmap                | 1000   |    21,454.8 ns |     439.19 ns |   1,245.91 ns |    21,400.0 ns |    46,609.5 |         - |
| ExceptWith_Bitmap_Bitmap                       | 1000   |       601.2 ns |      46.54 ns |     125.83 ns |       600.0 ns | 1,663,405.1 |         - |
| ExceptWith_HashSet                             | 1000   |    45,375.0 ns |   1,403.99 ns |   3,959.98 ns |    46,450.0 ns |    22,038.6 |      32 B |
| ExceptWith_Span_NativeClusteredBitmap          | 1000   |    22,916.8 ns |     417.92 ns |   1,199.08 ns |    22,700.0 ns |    43,636.0 |         - |
| IntersectWith_Span_Bitmap                      | 1000   |       913.5 ns |      32.73 ns |      90.69 ns |       900.0 ns | 1,094,710.9 |         - |
| IntersectWith_Span_ClusteredBitmap             | 1000   |     1,241.3 ns |      24.74 ns |      69.78 ns |     1,200.0 ns |   805,604.2 |         - |
| IntersectWith_Bitmap_Bitmap                    | 1000   |       518.8 ns |      31.52 ns |      85.21 ns |       500.0 ns | 1,927,437.6 |         - |
| IntersectWith_HashSet                          | 1000   |    22,385.0 ns |   1,315.45 ns |   3,878.65 ns |    24,150.0 ns |    44,672.8 |      32 B |
| IntersectWith_Span_NativeClusteredBitmap       | 1000   |     1,244.6 ns |      23.72 ns |      66.90 ns |     1,200.0 ns |   803,493.4 |         - |
| SymmetricExceptWith_Span_Bitmap                | 1000   |     1,016.8 ns |      26.25 ns |      75.30 ns |     1,000.0 ns |   983,436.9 |         - |
| SymmetricExceptWith_Span_ClusteredBitmap       | 1000   |     1,432.2 ns |      31.74 ns |      88.47 ns |     1,400.0 ns |   698,215.7 |         - |
| SymmetricExceptWith_Bitmap_Bitmap              | 1000   |       694.8 ns |      70.95 ns |     205.84 ns |       600.0 ns | 1,439,169.1 |         - |
| SymmetricExceptWith_HashSet                    | 1000   |    18,670.2 ns |     127.66 ns |     364.21 ns |    18,600.0 ns |    53,561.3 |      32 B |
| SymmetricExceptWith_Span_NativeClusteredBitmap | 1000   |     2,804.3 ns |      70.46 ns |     201.03 ns |     2,800.0 ns |   356,600.9 |         - |
| UnionWith_Span_Bitmap                          | 1000   |    18,207.5 ns |     308.50 ns |     875.17 ns |    18,000.0 ns |    54,922.3 |         - |
| UnionWith_Span_ClusteredBitmap                 | 1000   |    24,834.3 ns |   1,421.27 ns |   4,168.35 ns |    25,400.0 ns |    40,266.8 |         - |
| UnionWith_Bitmap_Bitmap                        | 1000   |       631.6 ns |      68.45 ns |     196.38 ns |       600.0 ns | 1,583,333.3 |         - |
| UnionWith_HashSet                              | 1000   |    13,143.2 ns |     187.50 ns |     493.95 ns |    13,000.0 ns |    76,084.9 |      32 B |
| UnionWith_Span_NativeClusteredBitmap           | 1000   |    18,672.4 ns |     281.62 ns |     770.94 ns |    18,400.0 ns |    53,554.9 |         - |
| MemoryUsed_Bitmap                              | 10000  |    55,087.1 ns |     561.09 ns |   1,516.95 ns |    54,600.0 ns |    18,153.1 |    3928 B |
| MemoryUsed_ClusteredBitmap                     | 10000  |    63,423.5 ns |     577.62 ns |   1,561.64 ns |    62,800.0 ns |    15,767.0 |    6808 B |
| MemoryUsed_HashSet                             | 10000  |   135,551.6 ns |   1,487.35 ns |   4,170.70 ns |   134,700.0 ns |     7,377.3 |  161800 B |
| MemoryUsed_ClusteredKeys_Bitmap                | 10000  |   307,520.5 ns |   6,633.36 ns |  18,270.21 ns |   304,400.0 ns |     3,251.8 | 3753280 B |
| MemoryUsed_ClusteredKeys_ClusteredBitmap       | 10000  |    61,822.2 ns |     360.89 ns |   1,006.02 ns |    61,500.0 ns |    16,175.4 |    5208 B |
| MemoryUsed_ClusteredKeys_HashSet               | 10000  |   131,151.1 ns |     719.14 ns |   2,028.34 ns |   130,500.0 ns |     7,624.8 |  161800 B |
| MemoryUsed_NativeClusteredBitmap               | 10000  |    60,764.0 ns |     516.14 ns |   1,430.21 ns |    60,300.0 ns |    16,457.1 |      48 B |
| MemoryUsed_ClusteredKeys_NativeClusteredBitmap | 10000  |    62,159.7 ns |     530.85 ns |   1,505.93 ns |    61,750.0 ns |    16,087.6 |      48 B |
| Contains_Bitmap                                | 10000  |    20,254.4 ns |     284.16 ns |     792.12 ns |    20,100.0 ns |    49,371.9 |         - |
| Contains_ClusteredBitmap                       | 10000  |    23,687.6 ns |     259.43 ns |     718.87 ns |    23,600.0 ns |    42,216.1 |         - |
| Contains_HashSet                               | 10000  |    98,689.4 ns |     571.91 ns |   1,546.20 ns |    98,300.0 ns |    10,132.8 |         - |
| Contains_NativeClusteredBitmap                 | 10000  |    29,416.2 ns |   2,480.53 ns |   7,274.96 ns |    25,400.0 ns |    33,994.9 |         - |
| Add_Bitmap                                     | 10000  |    49,841.8 ns |     357.61 ns |   1,002.78 ns |    49,500.0 ns |    20,063.5 |         - |
| Add_ClusteredBitmap                            | 10000  |    56,263.0 ns |     591.07 ns |   1,667.12 ns |    55,700.0 ns |    17,773.7 |         - |
| Add_HashSet                                    | 10000  |    94,625.0 ns |     398.49 ns |   1,070.51 ns |    94,400.0 ns |    10,568.0 |         - |
| Add_NativeClusteredBitmap                      | 10000  |    59,522.1 ns |   2,203.83 ns |   5,995.69 ns |    57,000.0 ns |    16,800.5 |         - |
| Iterate_Bitmap                                 | 10000  |    25,052.1 ns |     783.01 ns |   2,259.16 ns |    24,550.0 ns |    39,916.8 |         - |
| Iterate_ClusteredBitmap                        | 10000  |    28,806.3 ns |     564.92 ns |   1,620.86 ns |    28,400.0 ns |    34,714.6 |         - |
| Iterate_HashSet                                | 10000  |    14,973.6 ns |     272.36 ns |     763.74 ns |    14,900.0 ns |    66,784.1 |         - |
| Iterate_NativeClusteredBitmap                  | 10000  |    30,156.8 ns |     784.49 ns |   2,160.71 ns |    29,200.0 ns |    33,160.0 |         - |
| Remove_Bitmap                                  | 10000  |    54,518.5 ns |     550.66 ns |   1,553.16 ns |    54,100.0 ns |    18,342.4 |         - |
| Remove_ClusteredBitmap                         | 10000  |    59,181.3 ns |     509.41 ns |   1,428.43 ns |    58,700.0 ns |    16,897.2 |         - |
| Remove_HashSet                                 | 10000  |    79,588.8 ns |     562.73 ns |   1,559.31 ns |    79,000.0 ns |    12,564.6 |         - |
| Remove_NativeClusteredBitmap                   | 10000  |    65,662.9 ns |     323.44 ns |     896.25 ns |    65,400.0 ns |    15,229.3 |         - |
| ExceptWith_Span_Bitmap                         | 10000  |    52,608.6 ns |     380.68 ns |   1,079.92 ns |    52,500.0 ns |    19,008.3 |         - |
| ExceptWith_Span_ClusteredBitmap                | 10000  |    58,246.2 ns |     397.41 ns |   1,114.38 ns |    57,900.0 ns |    17,168.5 |         - |
| ExceptWith_Bitmap_Bitmap                       | 10000  |     3,624.0 ns |     376.93 ns |   1,111.38 ns |     4,400.0 ns |   275,938.2 |         - |
| ExceptWith_HashSet                             | 10000  |   114,096.5 ns |     488.36 ns |   1,320.30 ns |   114,000.0 ns |     8,764.5 |      32 B |
| ExceptWith_Span_NativeClusteredBitmap          | 10000  |    60,471.4 ns |     391.39 ns |   1,097.50 ns |    60,200.0 ns |    16,536.7 |         - |
| IntersectWith_Span_Bitmap                      | 10000  |     7,161.2 ns |      43.50 ns |     117.61 ns |     7,100.0 ns |   139,641.9 |         - |
| IntersectWith_Span_ClusteredBitmap             | 10000  |    10,303.2 ns |      36.92 ns |     104.73 ns |    10,300.0 ns |    97,057.0 |         - |
| IntersectWith_Bitmap_Bitmap                    | 10000  |     3,647.5 ns |     375.45 ns |   1,101.14 ns |     4,400.0 ns |   274,162.3 |         - |
| IntersectWith_HashSet                          | 10000  |   172,972.4 ns |   1,058.51 ns |   2,897.66 ns |   172,200.0 ns |     5,781.3 |     840 B |
| IntersectWith_Span_NativeClusteredBitmap       | 10000  |    10,340.5 ns |      28.96 ns |      77.80 ns |    10,300.0 ns |    96,707.3 |         - |
| SymmetricExceptWith_Span_Bitmap                | 10000  |     7,702.4 ns |      46.36 ns |     125.33 ns |     7,700.0 ns |   129,830.5 |         - |
| SymmetricExceptWith_Span_ClusteredBitmap       | 10000  |    12,622.4 ns |      55.74 ns |     150.69 ns |    12,600.0 ns |    79,224.5 |         - |
| SymmetricExceptWith_Bitmap_Bitmap              | 10000  |     3,989.0 ns |     384.34 ns |   1,133.23 ns |     4,600.0 ns |   250,689.4 |         - |
| SymmetricExceptWith_HashSet                    | 10000  |   212,267.7 ns |  10,755.06 ns |  30,510.32 ns |   195,300.0 ns |     4,711.0 |    1648 B |
| SymmetricExceptWith_Span_NativeClusteredBitmap | 10000  |    24,418.2 ns |     351.09 ns |   1,029.70 ns |    24,000.0 ns |    40,953.1 |         - |
| UnionWith_Span_Bitmap                          | 10000  |    47,767.4 ns |     319.27 ns |     868.59 ns |    47,600.0 ns |    20,934.8 |         - |
| UnionWith_Span_ClusteredBitmap                 | 10000  |    51,220.5 ns |     263.15 ns |     724.79 ns |    51,100.0 ns |    19,523.5 |         - |
| UnionWith_Bitmap_Bitmap                        | 10000  |     3,583.0 ns |     392.39 ns |   1,156.98 ns |     4,400.0 ns |   279,095.7 |         - |
| UnionWith_HashSet                              | 10000  |   120,654.2 ns |     218.01 ns |     581.90 ns |   120,600.0 ns |     8,288.1 |      32 B |
| UnionWith_Span_NativeClusteredBitmap           | 10000  |    51,098.9 ns |     278.30 ns |     780.38 ns |    51,000.0 ns |    19,569.9 |         - |
| MemoryUsed_Bitmap                              | 100000 |   377,688.4 ns |   6,320.94 ns |  17,196.58 ns |   372,350.0 ns |     2,647.7 |   37168 B |
| MemoryUsed_ClusteredBitmap                     | 100000 |   446,460.6 ns |   7,609.40 ns |  22,317.05 ns |   452,000.0 ns |     2,239.8 |   65672 B |
| MemoryUsed_HashSet                             | 100000 | 2,174,214.0 ns | 264,013.04 ns | 778,448.47 ns | 1,776,000.0 ns |       459.9 | 1738248 B |
| MemoryUsed_ClusteredKeys_Bitmap                | 100000 |   628,739.4 ns |  15,300.22 ns |  43,652.40 ns |   626,450.0 ns |     1,590.5 | 3780688 B |
| MemoryUsed_ClusteredKeys_ClusteredBitmap       | 100000 |   439,819.6 ns |   7,893.92 ns |  22,264.94 ns |   438,800.0 ns |     2,273.7 |   49920 B |
| MemoryUsed_ClusteredKeys_HashSet               | 100000 | 2,514,907.0 ns | 298,686.18 ns | 880,683.01 ns | 2,825,500.0 ns |       397.6 | 1738248 B |
| MemoryUsed_NativeClusteredBitmap               | 100000 |   458,824.4 ns |   4,965.02 ns |  13,840.49 ns |   459,250.0 ns |     2,179.5 |      48 B |
| MemoryUsed_ClusteredKeys_NativeClusteredBitmap | 100000 |   451,550.0 ns |   3,944.79 ns |  10,461.03 ns |   450,400.0 ns |     2,214.6 |      48 B |
| Contains_Bitmap                                | 100000 |    84,355.3 ns |   2,061.44 ns |   5,881.40 ns |    84,250.0 ns |    11,854.6 |         - |
| Contains_ClusteredBitmap                       | 100000 |   107,729.2 ns |   1,824.49 ns |   5,055.65 ns |   109,100.0 ns |     9,282.5 |         - |
| Contains_HashSet                               | 100000 | 1,203,021.3 ns |  50,328.39 ns | 143,589.74 ns | 1,191,900.0 ns |       831.2 |         - |
| Contains_NativeClusteredBitmap                 | 100000 |    96,039.8 ns |   2,181.82 ns |   6,189.46 ns |    96,800.0 ns |    10,412.4 |         - |
| Add_Bitmap                                     | 100000 |   339,171.6 ns |   2,982.57 ns |   8,214.87 ns |   339,100.0 ns |     2,948.4 |         - |
| Add_ClusteredBitmap                            | 100000 |   396,226.7 ns |   3,266.38 ns |   8,886.43 ns |   395,600.0 ns |     2,523.8 |         - |
| Add_HashSet                                    | 100000 | 1,354,593.3 ns |  86,166.75 ns | 240,198.48 ns | 1,312,850.0 ns |       738.2 |         - |
| Add_NativeClusteredBitmap                      | 100000 |   408,128.9 ns |   4,704.25 ns |  13,113.57 ns |   408,050.0 ns |     2,450.2 |         - |
| Iterate_Bitmap                                 | 100000 |    98,786.8 ns |   2,135.53 ns |   5,988.25 ns |    96,200.0 ns |    10,122.8 |         - |
| Iterate_ClusteredBitmap                        | 100000 |   130,888.7 ns |   2,436.21 ns |   7,067.88 ns |   130,600.0 ns |     7,640.1 |         - |
| Iterate_HashSet                                | 100000 |    87,076.3 ns |   1,989.40 ns |   5,643.59 ns |    85,500.0 ns |    11,484.2 |         - |
| Iterate_NativeClusteredBitmap                  | 100000 |   132,497.9 ns |   2,378.03 ns |   6,823.02 ns |   129,100.0 ns |     7,547.3 |         - |
| Remove_Bitmap                                  | 100000 |   391,269.7 ns |   2,298.13 ns |   6,368.11 ns |   391,800.0 ns |     2,555.8 |         - |
| Remove_ClusteredBitmap                         | 100000 |   423,479.1 ns |   2,525.27 ns |   6,870.19 ns |   422,800.0 ns |     2,361.4 |         - |
| Remove_HashSet                                 | 100000 |   880,876.7 ns |  31,623.75 ns |  86,034.75 ns |   842,950.0 ns |     1,135.2 |         - |
| Remove_NativeClusteredBitmap                   | 100000 |   486,529.2 ns |   3,618.72 ns |  10,027.45 ns |   488,300.0 ns |     2,055.4 |         - |
| ExceptWith_Span_Bitmap                         | 100000 |   378,186.2 ns |   2,623.24 ns |   7,181.07 ns |   377,600.0 ns |     2,644.2 |         - |
| ExceptWith_Span_ClusteredBitmap                | 100000 |   420,373.9 ns |   2,670.29 ns |   7,354.76 ns |   419,600.0 ns |     2,378.8 |         - |
| ExceptWith_Bitmap_Bitmap                       | 100000 |    11,020.0 ns |     342.03 ns |     953.43 ns |    10,800.0 ns |    90,744.1 |         - |
| ExceptWith_HashSet                             | 100000 | 1,233,743.7 ns |  36,861.49 ns | 100,907.75 ns | 1,189,500.0 ns |       810.5 |      32 B |
| ExceptWith_Span_NativeClusteredBitmap          | 100000 |   424,169.0 ns |   3,051.17 ns |   8,352.54 ns |   425,500.0 ns |     2,357.6 |         - |
| IntersectWith_Span_Bitmap                      | 100000 |    69,518.8 ns |     461.68 ns |   1,248.19 ns |    69,200.0 ns |    14,384.6 |         - |
| IntersectWith_Span_ClusteredBitmap             | 100000 |   104,725.9 ns |     610.00 ns |   1,649.18 ns |   104,200.0 ns |     9,548.7 |         - |
| IntersectWith_Bitmap_Bitmap                    | 100000 |    10,934.1 ns |     538.65 ns |   1,510.42 ns |    10,500.0 ns |    91,457.3 |         - |
| IntersectWith_HashSet                          | 100000 | 2,196,136.1 ns | 123,530.70 ns | 358,384.98 ns | 2,036,200.0 ns |       455.3 |    7960 B |
| IntersectWith_Span_NativeClusteredBitmap       | 100000 |   104,124.4 ns |     378.31 ns |   1,029.21 ns |   104,000.0 ns |     9,603.9 |         - |
| SymmetricExceptWith_Span_Bitmap                | 100000 |    76,505.4 ns |     795.59 ns |   2,243.98 ns |    75,800.0 ns |    13,071.0 |         - |
| SymmetricExceptWith_Span_ClusteredBitmap       | 100000 |   127,002.3 ns |   1,086.86 ns |   2,993.52 ns |   125,900.0 ns |     7,873.9 |         - |
| SymmetricExceptWith_Bitmap_Bitmap              | 100000 |    11,730.4 ns |     700.72 ns |   1,976.38 ns |    11,200.0 ns |    85,248.3 |         - |
| SymmetricExceptWith_HashSet                    | 100000 | 2,723,467.0 ns | 145,712.93 ns | 415,727.23 ns | 2,565,950.0 ns |       367.2 |   15888 B |
| SymmetricExceptWith_Span_NativeClusteredBitmap | 100000 |   230,056.2 ns |   1,312.73 ns |   3,637.56 ns |   229,100.0 ns |     4,346.8 |         - |
| UnionWith_Span_Bitmap                          | 100000 |   301,929.2 ns |   7,929.69 ns |  22,878.98 ns |   291,250.0 ns |     3,312.0 |         - |
| UnionWith_Span_ClusteredBitmap                 | 100000 |   347,813.6 ns |   4,175.00 ns |  11,499.16 ns |   345,600.0 ns |     2,875.1 |         - |
| UnionWith_Bitmap_Bitmap                        | 100000 |    10,966.3 ns |     431.69 ns |   1,196.20 ns |    10,600.0 ns |    91,188.5 |         - |
| UnionWith_HashSet                              | 100000 | 2,173,706.0 ns | 274,200.99 ns | 808,487.88 ns | 1,701,700.0 ns |       460.0 |      32 B |
| UnionWith_Span_NativeClusteredBitmap           | 100000 |   350,316.5 ns |   5,014.82 ns |  14,548.90 ns |   351,800.0 ns |     2,854.6 |         - |
