using System;
using System.Linq;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

using BenchmarkDotNet.Attributes;

namespace SumTests
{
    public class SumLong
    {
        private const int N = 10000;
        private readonly byte[] source;
        private readonly long[] data;

        public SumLong()
        {
            source = new byte[N];
            new Random(1).NextBytes(source);
            data = new long[N];
            Array.Copy(source, data, N);
        }

        [Benchmark]
        public long LinqSum()
        {
            return data.Sum(i => i);
        }

        [Benchmark(Baseline = true)]
        public long ForSum()
        {
            long result = 0;

            for (int i = 0; i < data.Length; i++)
            {
                result += data[i];
            };

            return result;
        }

        [Benchmark]
        public unsafe long FixedForSum()
        {
            long result = 0;
            var span = data.AsSpan();

            fixed (long* pSpan = span)
            {
                for (int i = 0; i < span.Length; i++)
                {
                    result += span[i];
                };
            }

            return result;
        }

        [Benchmark]
        public unsafe long UnrolledForSum()
        {
            //number of unrolling
            const int SIZE = 4;

            long result = 0;
            var span = data.AsSpan();
            int lastBlockIndex = data.Length - (data.Length % SIZE);

            fixed (long* pSpan = span)
            {
                for (int i = 0; i < lastBlockIndex; i += 4)
                {
                    result += pSpan[i + 0];
                    result += pSpan[i + 1];
                    result += pSpan[i + 2];
                    result += pSpan[i + 3];
                }

                for (int i = lastBlockIndex; i < span.Length; i++)
                {
                    result += span[i];
                }
            }

            return result;
        }

        [Benchmark]
        public long VectorizedForSum()
        {
            //number of calculators per CPU core in haswell+ architecture
            const int SIZE = 4;

            long result = 0;
            long partial1 = 0;
            long partial2 = 0;
            long partial3 = 0;
            long partial4 = 0;

            int sliceLenght = data.Length / SIZE;

            var span = data.AsSpan();
            var slice1 = span.Slice(0, sliceLenght);
            var slice2 = span.Slice(sliceLenght, sliceLenght);
            var slice3 = span.Slice(sliceLenght * 2, sliceLenght);
            var slice4 = span.Slice(sliceLenght * 3, sliceLenght);

            for (int i = 0; i < sliceLenght; i++)
            {
                partial1 += slice1[i];
                partial2 += slice2[i];
                partial3 += slice3[i];
                partial4 += slice4[i];
            };

            result += partial1;
            result += partial2;
            result += partial3;
            result += partial4;

            int lastBlockIndex = data.Length - (data.Length % SIZE);
            for (int i = lastBlockIndex; i < span.Length; i++)
            {
                result += span[i];
            }

            return result;
        }

        [Benchmark]
        public unsafe long SseSum()
        {
            const int TYPE_SIZE_PER_VECTOR = 2;

            long result = 0;
            Vector128<long> vResult = Vector128<long>.Zero;

            var span = data.AsSpan();
            int lastBlockIndex = data.Length - (data.Length % TYPE_SIZE_PER_VECTOR);

            fixed (long* pSpan = span)
            {
                for (int i = 0; i < lastBlockIndex; i += TYPE_SIZE_PER_VECTOR)
                {
                    vResult = Sse2.Add(vResult, Sse2.LoadVector128(pSpan + i));
                }
            }

            var temp = stackalloc long[TYPE_SIZE_PER_VECTOR];
            Sse2.Store(temp, vResult);

            for (int j = 0; j < TYPE_SIZE_PER_VECTOR; j++)
            {
                result += temp[j];
            }

            for (int i = lastBlockIndex; i < span.Length; i++)
            {
                result += span[i];
            }

            return result;
        }

        [Benchmark]
        public unsafe long AvxSum()
        {
            const int TYPE_SIZE_PER_VECTOR = 4;

            long result = 0;
            Vector256<long> vResult = Vector256<long>.Zero;

            var span = data.AsSpan();
            int lastBlockIndex = data.Length - (data.Length % TYPE_SIZE_PER_VECTOR);

            fixed (long* pSpan = span)
            {
                for (int i = 0; i < lastBlockIndex; i += TYPE_SIZE_PER_VECTOR)
                {
                    vResult = Avx2.Add(vResult, Avx2.LoadVector256(pSpan + i));
                }
            }

            var temp = stackalloc long[TYPE_SIZE_PER_VECTOR];
            Avx2.Store(temp, vResult);

            for (int j = 0; j < TYPE_SIZE_PER_VECTOR; j++)
            {
                result += temp[j];
            }

            for (int i = lastBlockIndex; i < span.Length; i++)
            {
                result += span[i];
            }

            return result;
        }
    }
}
