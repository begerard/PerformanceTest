# PerformanceTest

Some tests about .Net performance and optimization with .Net Core 3.1. Use BenchmarkDotNet for time measurements.

## SumTest

Just a Sum of various numeric type arrays done in various way, ordered by performance:
1. LINQ: Linq's Sum method on the array
2. For: Simple For loop over the array
3. Fixed: Same For loop but inside a fixed statement
4. Unrolled: A For loop where each loop executes 4 incrementations instead of 1
5. Vectorization: A For loop where the array is Sliced in 4 part
6. SIMD: A For loop over Intrinsics Add

Here are the result with a Haswell CPU (i5-4690K @ 4.4GHz) as older ones won't support as much optimizations:

### Byte[10 000]
|           Method |      Mean |   Error |  StdDev | Ratio |
|----------------- |----------:|--------:|--------:|------:|
|          LinqSum | 51.529 us | 67.8 ns | 63.4 ns | 11.99 |
|           ForSum |  4.297 us |  5.1 ns |  4.7 ns |  1.00 |
|      FixedForSum |  3.274 us |  2.6 ns |  2.4 ns |  0.76 |
|   UnrolledForSum |  2.864 us |  2.5 ns |  2.4 ns |  0.66 |
| VectorizedForSum |  1.724 us |  1.6 ns |  1.5 ns |  0.40 |

### Int32[10 000]
|           Method |      Mean |   Error |  StdDev | Ratio |
|----------------- |----------:|--------:|--------:|------:|
|          LinqSum | 48.076 us | 40.7 ns | 38.1 ns |  9.57 |
|           ForSum |  5.020 us | 10.2 ns |  9.5 ns |  1.00 |
|      FixedForSum |  5.128 us |  8.8 ns |  7.8 ns |  1.02 |
|   UnrolledForSum |  2.408 us |  9.9 ns |  9.2 ns |  0.47 |
| VectorizedForSum |  1.696 us |  2.8 ns |  2.5 ns |  0.33 |
|          SimdSum |  0.880 us |  2.6 ns |  2.3 ns |  0.17 |

### Commentary
1. Awful performance as expected. The ( i => i ) Func is extremely slow and allocate.
2. Simplest algorithm I used as performance ratio base.
3. The 'fixed' statement can help by eliding some checks. But if this optimization does help performance with Byte (-24%) it degrades it with Int32. This unsafe statement cost is hard to justify by itself.
4. Manual unrolling have low code complexity, but does give large speed-up (-53%). The number of unroll should be adapted to the size of type: here I haven't so it's benefit with Byte is limited (-34%).
5. Manual vectorization leverage the multiple calculators inside each CPU core. It increases the code complexity like parallelization but without using multiple cores (so it can be added up to a real parallelization), and it does give one of the best speed-up (-6X%).
6. SIMD optimization is of course the most profitable optimization (-83%, and it can be higher) but it also introduces the biggest complexity, and it have limitation like for example overflowing that discourage its use for Byte.
