# PerformanceTest

Some tests about .Net performance and optimization with .Net Core 3.1. Use BenchmarkDotNet for time measurements.

## SumTest

Just a Sum of various numeric type arrays done in various way, ordered by performance:
1. LINQ: Linq's Sum method on the array
2. For: Simple For loop over the array
3. Fixed: Same For loop but inside a fixed statement
4. Unrolled: A For loop where each loop executes 4 incrementations instead of 1
5. Vectorization: A For loop where the data is sliced in 4 parts which are incremented independently
6. SSE: A For loop where the data is sliced as 128bits vectors which are added with SSE2 Intrinsics
7. AVX: A For loop where the data is sliced as 256bits vectors which are added with AVX2 Intrinsics

Here are the result with a Haswell CPU (i5-4690K @3.50GHz) as older ones won't support as much optimizations:

### Byte[10 000]
|           Method |      Mean |     Error |    StdDev | Ratio |
|----------------- |----------:|----------:|----------:|------:|
|          LinqSum | 63.811 us | 0.3143 us | 0.2787 us | 12.67 |
|           ForSum |  5.036 us | 0.0159 us | 0.0133 us |  1.00 |
|      FixedForSum |  3.817 us | 0.0131 us | 0.0102 us |  0.76 |
|   UnrolledForSum |  3.359 us | 0.0227 us | 0.0201 us |  0.67 |
| VectorizedForSum |  2.014 us | 0.0058 us | 0.0048 us |  0.40 |

### Int32[10 000]
|           Method |      Mean |     Error |    StdDev | Ratio |
|----------------- |----------:|----------:|----------:|------:|
|          LinqSum | 63.853 us | 0.2540 ns | 0.2376 ns | 10.92 |
|           ForSum |  5.849 us | 0.1571 ns | 0.1393 ns |  1.00 |
|      FixedForSum |  6.002 us | 0.2361 ns | 0.2093 ns |  1.03 |
|   UnrolledForSum |  2.812 us | 0.1259 ns | 0.1051 ns |  0.48 |
| VectorizedForSum |  1.979 us | 0.0914 ns | 0.0810 ns |  0.34 |
|           SseSum |  1.084 us | 0.0631 ns | 0.0559 ns |  0.19 |
|           AvxSum |  0.648 us | 0.0257 ns | 0.0215 ns |  0.11 |

### Int64[10 000]
|           Method |      Mean |     Error |    StdDev | Ratio |
|----------------- |----------:|----------:|----------:|------:|
|          LinqSum | 59.011 us | 0.2525 us | 0.2108 us | 10.06 |
|           ForSum |  5.867 us | 0.0205 us | 0.0171 us |  1.00 |
|      FixedForSum |  5.994 us | 0.0196 us | 0.0174 us |  1.02 |
|   UnrolledForSum |  2.883 us | 0.0157 us | 0.0139 us |  0.49 |
| VectorizedForSum |  1.984 us | 0.0097 us | 0.0086 us |  0.34 |
|           SseSum |  2.228 us | 0.0113 us | 0.0101 us |  0.38 |
|           AvxSum |  1.280 us | 0.0096 us | 0.0090 us |  0.22 |

### Commentary

1. Awful performance being 10 time slower than the For loop. The ( i => i ) Func is very slow and allocate memory.
2. Simplest algorithm I used as performance baseline.
3. The 'fixed' statement can help by eliding some checks. But if this optimization does help performance with Byte (-24%) it degrades it with Ints. This unsafe statement cost is hard to justify by itself.
4. Manual unrolling have low code complexity, but does give large speed-up (-50%).
5. Manual vectorization leverage the multiple calculators inside each CPU core. It increases the code complexity like parallelization but without using multiple cores (so it can be added up to a real parallelization), and it does give one of the best speed-up (-66%).
6. SSE2 Intrinsics give a large performance gain over the vertorilzation for the Int32 type (-80%) but a performance loss for the Int64 (-62%). This is due to the size of the SSE2 vector.
7. AVX2 Intrinsics and its wider vector help the type Int64 to get where Int32 is with SSE2 and get better performance than the vectorized case. The Int32 type also got a performance gain (-90%).

SIMD optimisations introduce the biggest complexity and have limitations like overflowing that aren't handled in my example, but it can give the best performance with the AVX2 implementation 10 times faster than a naive For loop.
