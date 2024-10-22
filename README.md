# PerformanceTest

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
| Method           | Mean      | Error     | StdDev    | Ratio | RatioSD |
|----------------- |----------:|----------:|----------:|------:|--------:|
| LinqSumCast      | 49.386 us | 0.4881 us | 0.4076 us | 10.94 |    0.11 |
| ForSum           |  4.516 us | 0.0358 us | 0.0280 us |  1.00 |    0.01 |
| SpanForSum       |  3.936 us | 0.0405 us | 0.0316 us |  0.87 |    0.01 |
| UnrolledForSum   |  4.117 us | 0.0457 us | 0.0427 us |  0.91 |    0.01 |
| VectorizedForSum |  1.560 us | 0.0302 us | 0.0236 us |  0.35 |    0.01 |

### Int32[10 000]
| Method           | Mean     | Error     | StdDev    | Ratio | RatioSD |
|----------------- |---------:|----------:|----------:|------:|--------:|
| LinqSum          | 4.699 us | 0.0826 us | 0.0732 us |  1.03 |    0.02 |
| ForSum           | 4.578 us | 0.0613 us | 0.0512 us |  1.00 |    0.02 |
| SpanForSum       | 3.846 us | 0.0434 us | 0.0362 us |  0.84 |    0.01 |
| UnrolledForSum   | 3.108 us | 0.0418 us | 0.0349 us |  0.68 |    0.01 |
| VectorizedForSum | 1.533 us | 0.0247 us | 0.0219 us |  0.33 |    0.01 |

### Int64[10 000]
| Method           | Mean     | Error     | StdDev    | Ratio | RatioSD |
|----------------- |---------:|----------:|----------:|------:|--------:|
| LinqSum          | 4.670 us | 0.0703 us | 0.0623 us |  1.01 |    0.02 |
| ForSum           | 4.638 us | 0.0563 us | 0.0499 us |  1.00 |    0.01 |
| SpanForSum       | 3.904 us | 0.0768 us | 0.0718 us |  0.84 |    0.02 |
| UnrolledForSum   | 3.148 us | 0.0376 us | 0.0314 us |  0.68 |    0.01 |
| VectorizedForSum | 1.569 us | 0.0284 us | 0.0265 us |  0.34 |    0.01 |

### Int128[10 000]
| Method           | Mean      | Error     | StdDev    | Median    | Ratio | RatioSD |
|----------------- |----------:|----------:|----------:|----------:|------:|--------:|
| LinqAggregate    | 80.782 us | 1.6151 us | 2.1000 us | 80.728 us |  8.95 |    0.56 |
| ForSum           |  9.056 us | 0.1871 us | 0.5458 us |  8.809 us |  1.00 |    0.08 |
| SpanForSum       |  8.248 us | 0.1348 us | 0.1195 us |  8.223 us |  0.91 |    0.05 |
| UnrolledForSum   |  7.296 us | 0.0870 us | 0.0771 us |  7.282 us |  0.81 |    0.05 |
| VectorizedForSum |  6.192 us | 0.0750 us | 0.0665 us |  6.179 us |  0.69 |    0.04 |

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
| Method           | Mean     | Error     | StdDev    | Ratio | RatioSD |
|----------------- |---------:|----------:|----------:|------:|--------:|
| LinqSumCast      | 9.965 us | 0.1198 us | 0.1176 us |  2.21 |    0.04 |
| ForSum           | 4.516 us | 0.0847 us | 0.0707 us |  1.00 |    0.02 |
| SpanForSum       | 3.936 us | 0.0609 us | 0.0540 us |  0.87 |    0.02 |
| UnrolledForSum   | 2.814 us | 0.0537 us | 0.0503 us |  0.62 |    0.01 |
| VectorizedForSum | 1.578 us | 0.0307 us | 0.0316 us |  0.35 |    0.01 |

### Int32[10 000]
| Method           | Mean     | Error     | StdDev    | Ratio | RatioSD |
|----------------- |---------:|----------:|----------:|------:|--------:|
| LinqSum          | 0.798 us | 0.0089 us | 0.0079 us |  0.18 |    0.00 |
| ForSum           | 4.540 us | 0.0601 us | 0.0562 us |  1.00 |    0.02 |
| SpanForSum       | 3.821 us | 0.0244 us | 0.0217 us |  0.84 |    0.01 |
| UnrolledForSum   | 2.662 us | 0.0169 us | 0.0141 us |  0.59 |    0.01 |
| VectorizedForSum | 1.488 us | 0.0216 us | 0.0181 us |  0.33 |    0.01 |

### Int64[10 000]
| Method           | Mean     | Error     | StdDev    | Ratio | RatioSD |
|----------------- |---------:|----------:|----------:|------:|--------:|
| LinqSum          | 1.630 us | 0.0280 us | 0.0234 us |  0.35 |    0.01 |
| ForSum           | 4.668 us | 0.0918 us | 0.0901 us |  1.00 |    0.03 |
| FixedForSum      | 3.891 us | 0.0558 us | 0.0495 us |  0.83 |    0.02 |
| UnrolledForSum   | 2.444 us | 0.0421 us | 0.0413 us |  0.52 |    0.01 |
| VectorizedForSum | 1.556 us | 0.0309 us | 0.0303 us |  0.33 |    0.01 |

### Int128[10 000]
| Method           | Mean      | Error     | StdDev    | Ratio | RatioSD |
|----------------- |----------:|----------:|----------:|------:|--------:|
| LinqAggregate    | 20.887 us | 0.3648 us | 0.3234 us |  2.12 |    0.04 |
| ForSum           |  9.852 us | 0.1310 us | 0.1161 us |  1.00 |    0.02 |
| SpanForSum       |  8.965 us | 0.1146 us | 0.0957 us |  0.91 |    0.01 |
| UnrolledForSum   |  8.899 us | 0.1326 us | 0.1176 us |  0.90 |    0.02 |
| VectorizedForSum |  7.018 us | 0.1035 us | 0.0918 us |  0.71 |    0.01 |

### Commentary
1. LINQ is now the best pick as long as you do net give it a Func:
    1. When there is a Sum method with the good type, its use of intrinsics make it as performant as the vectorized case.
    2. When you have to give a Func to the Linq method, you now are only 2 times slower.
2. Simplest algorithm I used as performance baseline.
3. Span is the same as 7.0.
4. Unrolled got faster with the Byte but slower with the Int128.
5. Vectorized  is the same as 7.0.

* The upgrade of Linq make it a simpler alternative to the Vectorize solution but only if the native methods fit the case.
* If you need to give a delegate to execute to the Linq method it will be slower than a For loop.
