# SerializerTests

Performance comparaison of both serialization and deserialization for several serializers in 3 modes: no referenced data, referenced data without cycle, referenced data with cycles.
1. Stj = System.Text.Json
2. StjTyped = System.Text.Json with a SourceGenerationContext
3. MsgPack = Nerdbank.MessagePack
4. MemPack = Cysharp.MemoryPack

MemoryPack is the hardest to setup (no easy way to do a "witness" type), so it don't have a "no reference" mesurement.

## Serialization
| Method                   | Mean      | Error     | StdDev    | Ratio | RatioSD | Allocated | Alloc Ratio |
|------------------------- |----------:|----------:|----------:|------:|--------:|----------:|------------:|
| StjWithoutReference      | 547.42 us |  5.103 us |  4.524 us |  1.00 |    0.01 | 356.35 KB |        1.00 |
| StjTypedWithoutReference | 568.14 us |  5.760 us |  5.388 us |  1.04 |    0.01 |  366.6 KB |        1.03 |
| MsgPackWithoutReference  | 125.25 us |  1.008 us |  0.893 us |  0.23 |    0.00 |  34.49 KB |        0.10 |
| CerasWithoutReference    |  74.72 us |  0.334 us |  0.296 us |  0.14 |    0.00 |  54.43 KB |        0.15 |
| StjWithReference         | 702.09 us |  6.995 us |  5.841 us |  1.28 |    0.01 | 516.46 KB |        1.45 |
| StjTypedWithReference    | 685.16 us |  7.986 us |  7.470 us |  1.25 |    0.02 | 527.44 KB |        1.48 |
| MsgPackWithReference     | 208.93 us |  2.749 us |  2.437 us |  0.38 |    0.01 |  34.49 KB |        0.10 |
| MemPackWithReference     | 201.51 us |  1.841 us |  1.722 us |  0.37 |    0.00 |  90.51 KB |        0.25 |
| CerasWithReference       | 103.62 us |  1.991 us |  1.663 us |  0.19 |    0.00 |  54.43 KB |        0.15 |
| StjWithCycle             | 782.53 us | 11.402 us | 10.666 us |  1.43 |    0.02 | 538.58 KB |        1.51 |
| StjTypedWithCycle        | 743.53 us |  5.553 us |  5.194 us |  1.36 |    0.01 | 549.09 KB |        1.54 |
| MsgPackWithCycle         | 253.70 us |  4.830 us |  4.518 us |  0.46 |    0.01 |  48.13 KB |        0.14 |
| MemPackWithCycle         | 212.43 us |  1.458 us |  1.218 us |  0.39 |    0.00 |  93.46 KB |        0.26 |
| CerasWithCycle           | 116.66 us |  0.647 us |  0.573 us |  0.21 |    0.00 |   55.4 KB |        0.16 |

## Deserialization
| Method                   | Mean       | Error    | StdDev   | Ratio | RatioSD | Allocated | Alloc Ratio |
|------------------------- |-----------:|---------:|---------:|------:|--------:|----------:|------------:|
| StjWithoutReference      |   724.8 us |  8.62 us |  7.20 us |  1.00 |    0.01 | 197.24 KB |        1.00 |
| StjTypedWithoutReference |   740.4 us |  7.28 us |  6.81 us |  1.02 |    0.01 | 197.24 KB |        1.00 |
| MsgPackWithoutReference  |   133.6 us |  1.55 us |  1.45 us |  0.18 |    0.00 | 195.87 KB |        0.99 |
| CerasWithoutReference    |   109.8 us |  0.68 us |  0.57 us |  0.15 |    0.00 | 192.24 KB |        0.97 |
| StjWithReference         |   949.0 us | 10.21 us |  9.55 us |  1.31 |    0.02 | 335.18 KB |        1.70 |
| StjTypedWithReference    |   963.8 us | 12.90 us | 12.07 us |  1.33 |    0.02 | 335.18 KB |        1.70 |
| MsgPackWithReference     |   177.9 us |  1.78 us |  1.58 us |  0.25 |    0.00 | 195.87 KB |        0.99 |
| MemPackWithReference     |   115.9 us |  1.48 us |  1.31 us |  0.16 |    0.00 | 171.62 KB |        0.87 |
| CerasWithReference       |   161.1 us |  2.51 us |  2.68 us |  0.22 |    0.00 | 192.24 KB |        0.97 |
| StjWithCycle             | 1,157.8 us | 19.01 us | 15.87 us |  1.60 |    0.03 |  369.4 KB |        1.87 |
| StjTypedWithCycle        | 1,158.3 us | 17.14 us | 15.20 us |  1.60 |    0.03 |  369.4 KB |        1.87 |
| MsgPackWithCycle         |   292.3 us |  2.79 us |  2.61 us |  0.40 |    0.01 | 195.87 KB |        0.99 |
| MemPackWithCycle         |   126.5 us |  1.02 us |  0.86 us |  0.17 |    0.00 | 171.62 KB |        0.87 |
| CerasWithCycle           |   168.5 us |  2.10 us |  1.76 us |  0.23 |    0.00 | 192.24 KB |        0.97 |

## Commentary
Ceras is the fastest to serialize and MemoryPack is the fastest to deserialize.

# SumTests

Some tests about performance and optimization with different version of .Net on a simple Sum operation.
Uses BenchmarkDotNet for time measurements.

## .Net Core 3.1

Just a Sum of various numeric type arrays done in various way, ordered by performance:
1. For: Simple For loop over the array
2. Span: A span fixed For loop
3. Unrolled: A span fixed For loop where each loop executes 4 incrementations instead of 1
4. Vectorization: A span For loop where the data is sliced in 4 parts which are incremented independently
5. SSE: A For loop where the data is sliced as 128bits vectors which are added with SSE2 Intrinsics
6. AVX: A For loop where the data is sliced as 256bits vectors which are added with AVX2 Intrinsics

Here are the result with a Haswell CPU (i5-4690K @3.50GHz) as older ones won't support as much optimizations:

### Byte[10 000]
|           Method |      Mean |     Error |    StdDev | Ratio |
|----------------- |----------:|----------:|----------:|------:|
|           ForSum |  5.036 us | 0.0159 us | 0.0133 us |  1.00 |
|       SpanForSum |  3.817 us | 0.0131 us | 0.0102 us |  0.76 |
|   UnrolledForSum |  3.359 us | 0.0227 us | 0.0201 us |  0.67 |
| VectorizedForSum |  2.014 us | 0.0058 us | 0.0048 us |  0.40 |

### Int32[10 000]
|           Method |      Mean |     Error |    StdDev | Ratio |
|----------------- |----------:|----------:|----------:|------:|
|           ForSum |  5.849 us | 0.1571 ns | 0.1393 ns |  1.00 |
|       SpanForSum |  6.002 us | 0.2361 ns | 0.2093 ns |  1.03 |
|   UnrolledForSum |  2.812 us | 0.1259 ns | 0.1051 ns |  0.48 |
| VectorizedForSum |  1.979 us | 0.0914 ns | 0.0810 ns |  0.34 |
|           SseSum |  1.084 us | 0.0631 ns | 0.0559 ns |  0.19 |
|           AvxSum |  0.648 us | 0.0257 ns | 0.0215 ns |  0.11 |

### Int64[10 000]
|           Method |      Mean |     Error |    StdDev | Ratio |
|----------------- |----------:|----------:|----------:|------:|
|           ForSum |  5.867 us | 0.0205 us | 0.0171 us |  1.00 |
|       SpanForSum |  5.994 us | 0.0196 us | 0.0174 us |  1.02 |
|   UnrolledForSum |  2.883 us | 0.0157 us | 0.0139 us |  0.49 |
| VectorizedForSum |  1.984 us | 0.0097 us | 0.0086 us |  0.34 |
|           SseSum |  2.228 us | 0.0113 us | 0.0101 us |  0.38 |
|           AvxSum |  1.280 us | 0.0096 us | 0.0090 us |  0.22 |

### Commentary
1. Simplest algorithm I used as performance baseline.
2. Using a Span for looping instead of a an array can help performance with Byte (-24%) but it degrades it with bigger types.
3. Manual unrolling have low code complexity, but does give large speed-up (-50%).
4. Manual vectorization leverage the multiple calculators inside each CPU core. It increases the code complexity like parallelization but without using multiple cores (so it can be added up to a real parallelization), and it does give one of the best speed-up (-66%).
5. SSE2 Intrinsics give a large performance gain over the vertorilzation for the Int32 type (-80%) but a performance loss for the Int64 (-62%). This is due to the size of the SSE2 vector.
6. AVX2 Intrinsics and its wider vector help the type Int64 to get where Int32 is with SSE2 and get better performance than the vectorized case. The Int32 type also got a performance gain (-90%).

* Vectorized give a good and stable performance upgrade without the need of "unsafe" unlike all the other cases.
* SIMD optimisations introduce the biggest complexity and can have limitations like overflowing that aren't handled in my example, but it also give the best performance with the AVX2 implementation: 5 to 10 times faster than a naive For loop.

## .Net 7.0

Updated to 7.0 and with the Linq equivalent depending the type.
Add the new type Int128 to see how it goes with datas larger than the CPU architecture.
Remove the unsafe 'fixed' from Span and Unrolled as it's not necessary for Vectorized so it was an unfare comparison.
Newer CPU (i7-1165G7) is a bit faster, but without impactfull architecture changes.

### Byte[10 000]
| Method           | Mean      | Error     | StdDev    | Ratio |
|----------------- |----------:|----------:|----------:|------:|
| LinqSumCast      | 49.386 us | 0.4881 us | 0.4076 us | 10.94 |
| ForSum           |  4.516 us | 0.0358 us | 0.0280 us |  1.00 |
| SpanForSum       |  3.936 us | 0.0405 us | 0.0316 us |  0.87 |
| UnrolledForSum   |  4.117 us | 0.0457 us | 0.0427 us |  0.91 |
| VectorizedForSum |  1.560 us | 0.0302 us | 0.0236 us |  0.35 |

### Int32[10 000]
| Method           | Mean     | Error     | StdDev    | Ratio |
|----------------- |---------:|----------:|----------:|------:|
| LinqSum          | 4.699 us | 0.0826 us | 0.0732 us |  1.03 |
| ForSum           | 4.578 us | 0.0613 us | 0.0512 us |  1.00 |
| SpanForSum       | 3.846 us | 0.0434 us | 0.0362 us |  0.84 |
| UnrolledForSum   | 3.108 us | 0.0418 us | 0.0349 us |  0.68 |
| VectorizedForSum | 1.533 us | 0.0247 us | 0.0219 us |  0.33 |

### Int64[10 000]
| Method           | Mean     | Error     | StdDev    | Ratio |
|----------------- |---------:|----------:|----------:|------:|
| LinqSum          | 4.670 us | 0.0703 us | 0.0623 us |  1.01 |
| ForSum           | 4.638 us | 0.0563 us | 0.0499 us |  1.00 |
| SpanForSum       | 3.904 us | 0.0768 us | 0.0718 us |  0.84 |
| UnrolledForSum   | 3.148 us | 0.0376 us | 0.0314 us |  0.68 |
| VectorizedForSum | 1.569 us | 0.0284 us | 0.0265 us |  0.34 |

### Int128[10 000]
| Method           | Mean      | Error     | StdDev    | Ratio |
|----------------- |----------:|----------:|----------:|------:|
| LinqAggregate    | 80.782 us | 1.6151 us | 2.1000 us |  8.95 |
| ForSum           |  9.056 us | 0.1871 us | 0.5458 us |  1.00 |
| SpanForSum       |  8.248 us | 0.1348 us | 0.1195 us |  0.91 |
| UnrolledForSum   |  7.296 us | 0.0870 us | 0.0771 us |  0.81 |
| VectorizedForSum |  6.192 us | 0.0750 us | 0.0665 us |  0.69 |

### Commentary
1. LINQ can be as good as the For loop or much slower depending its usage:
    1. When there is a Sum method with the good type, no problem.
    2. When you have to give a Func to the Sum method like with the Byte case that need casting, you get 10 times slower.
    3. When you can't use the Sum method at all and fallback on Aggregate like the Int128 that can't be cast, you get a comparable slow down.
2. Simplest algorithm I used as performance baseline.
3. Using a Span is now always a win (-15%) but a smaller one because it is not 'fixed' anymore.
4. Unrolled also have a smaller win without 'fixed' (-30%) and degrades the performance of Byte.
5. No change for Vectorized (it didn't used 'fixed').

* Without 'fixed' Span and Unrolled give smaller gain but there may be better way to do this with the new .Net APIs.
* As expected the Int128 perform exactly 2 times slower than Int64, as it is two Int64.

## .Net 8.0

Update to 8.0 as Linq now use intinsics.

### Byte[10 000]
| Method           | Mean     | Error     | StdDev    | Ratio |
|----------------- |---------:|----------:|----------:|------:|
| LinqSumCast      | 9.965 us | 0.1198 us | 0.1176 us |  2.21 |
| ForSum           | 4.516 us | 0.0847 us | 0.0707 us |  1.00 |
| SpanForSum       | 3.936 us | 0.0609 us | 0.0540 us |  0.87 |
| UnrolledForSum   | 2.814 us | 0.0537 us | 0.0503 us |  0.62 |
| VectorizedForSum | 1.578 us | 0.0307 us | 0.0316 us |  0.35 |

### Int32[10 000]
| Method           | Mean     | Error     | StdDev    | Ratio |
|----------------- |---------:|----------:|----------:|------:|
| LinqSum          | 0.798 us | 0.0089 us | 0.0079 us |  0.18 |
| ForSum           | 4.540 us | 0.0601 us | 0.0562 us |  1.00 |
| SpanForSum       | 3.821 us | 0.0244 us | 0.0217 us |  0.84 |
| UnrolledForSum   | 2.662 us | 0.0169 us | 0.0141 us |  0.59 |
| VectorizedForSum | 1.488 us | 0.0216 us | 0.0181 us |  0.33 |

### Int64[10 000]
| Method           | Mean     | Error     | StdDev    | Ratio |
|----------------- |---------:|----------:|----------:|------:|
| LinqSum          | 1.630 us | 0.0280 us | 0.0234 us |  0.35 |
| ForSum           | 4.668 us | 0.0918 us | 0.0901 us |  1.00 |
| SpanForSum       | 3.891 us | 0.0558 us | 0.0495 us |  0.83 |
| UnrolledForSum   | 2.444 us | 0.0421 us | 0.0413 us |  0.52 |
| VectorizedForSum | 1.556 us | 0.0309 us | 0.0303 us |  0.33 |

### Int128[10 000]
| Method           | Mean      | Error     | StdDev    | Ratio |
|----------------- |----------:|----------:|----------:|------:|
| LinqAggregate    | 20.887 us | 0.3648 us | 0.3234 us |  2.12 |
| ForSum           |  9.852 us | 0.1310 us | 0.1161 us |  1.00 |
| SpanForSum       |  8.965 us | 0.1146 us | 0.0957 us |  0.91 |
| UnrolledForSum   |  8.899 us | 0.1326 us | 0.1176 us |  0.90 |
| VectorizedForSum |  7.018 us | 0.1035 us | 0.0918 us |  0.71 |

### Commentary
1. LINQ is now the best pick as long as you do not give it a Func:
    1. When there is a Sum method with the good type, its use of intrinsics make it as performant as the vectorized case.
    2. When you have to give a Func to the Linq method, you now are only 2 times slower.
2. Simplest algorithm I used as performance baseline.
3. Span is the same as 7.0.
4. Unrolled got faster with the Byte but slower with the Int128.
5. Vectorized  is the same as 7.0.

* The upgrade of Linq make it a simpler alternative to the Vectorize solution but only if the native methods fit the case.
* If you need to give a delegate to execute to the Linq method it will be slower than a For loop.

## .Net 9.0

### Byte[10 000]
| Method           | Mean      | Error     | StdDev    | Ratio |
|----------------- |----------:|----------:|----------:|------:|
| LinqSumCast      | 11.017 us | 0.0790 us | 0.0739 us |  2.77 |
| ForSum           |  3.981 us | 0.0387 us | 0.0362 us |  1.00 |
| SpanForSum       |  3.315 us | 0.0464 us | 0.0387 us |  0.83 |
| UnrolledForSum   |  2.473 us | 0.0415 us | 0.0389 us |  0.62 |
| VectorizedForSum |  1.480 us | 0.0189 us | 0.0186 us |  0.37 |

### Int32[10 000]
| Method           | Mean      | Error     | StdDev    | Ratio |
|----------------- |----------:|----------:|----------:|------:|
| LinqSum          | 0.6319 us | 0.0087 us | 0.0081 us |  0.16 |
| ForSum           | 3.8573 us | 0.0604 us | 0.0565 us |  1.00 |
| SpanForSum       | 2.9409 us | 0.0486 us | 0.0454 us |  0.76 |
| UnrolledForSum   | 2.4531 us | 0.0387 us | 0.0362 us |  0.64 |
| VectorizedForSum | 1.4416 us | 0.0273 us | 0.0255 us |  0.37 |

### Int64[10 000]
| Method           | Mean     | Error     | StdDev    | Ratio |
|----------------- |---------:|----------:|----------:|------:|
| LinqSum          | 1.395 us | 0.0273 us | 0.0256 us |  0.34 |
| ForSum           | 4.070 us | 0.0452 us | 0.0401 us |  1.00 |
| SpanForSum       | 3.000 us | 0.0496 us | 0.0464 us |  0.74 |
| UnrolledForSum   | 2.510 us | 0.0467 us | 0.0437 us |  0.62 |
| VectorizedForSum | 1.480 us | 0.0275 us | 0.0257 us |  0.36 |

### Int128[10 000]
| Method           | Mean     | Error     | StdDev    | Ratio | 
|----------------- |---------:|----------:|----------:|------:|
| LinqAggregate    | 9.961 us | 0.1430 us | 0.1338 us |  1.07 | 
| ForSum           | 9.319 us | 0.0752 us | 0.0628 us |  1.00 | 
| SpanForSum       | 7.311 us | 0.1149 us | 0.1075 us |  0.78 | 
| UnrolledForSum   | 7.517 us | 0.1129 us | 0.1056 us |  0.81 | 
| VectorizedForSum | 6.404 us | 0.1079 us | 0.1010 us |  0.69 | 

### Commentary
1. .Net9 have optimized some LINQ query, and we see it with the Aggregate function which got a large performance gain (-50%).
2. General optimizations deliver up to -20% gain and at least the same performance that .net8.
3. The special case of the LINQ Cast+Sum function is the only exception with a +20% performance degradation.

* Optimized LINQ expression looks great with the Aggregate function being as fast as the for loop now!
