using System;
using System.Linq;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

using BenchmarkDotNet.Attributes;

namespace SumTests
{
    public class SumInt
    {
        private const int N = 10000;
        private readonly byte[] source;
        private readonly int[] data;

        public SumInt()
        {
            source = new byte[N];
            new Random(1).NextBytes(source);
            data = new int[N];
            Array.Copy(source, data, N);
        }

        [Benchmark]
        public int LinqSum()
        {
            return data.Sum(i => i);
        }

        [Benchmark]
        public int ForSum()
        {
            int result = 0;

            for (int i = 0; i < data.Length; i++)
            {
                result += data[i];
            };

            return result;
        }

        [Benchmark]
        public unsafe int FixedForSum()
        {
            int result = 0;
            var span = data.AsSpan();

            fixed (int* pSpan = span)
            {
                for (int i = 0; i < span.Length; i++)
                {
                    result += span[i];
                };
            }

            return result;
        }

        [Benchmark]
        public unsafe int UnrolledForSum()
        {
            //number of unrolling
            const int SIZE = 4;

            int result = 0;
            var span = data.AsSpan();
            int lastBlockIndex = data.Length - (data.Length % SIZE);

            fixed (int* pSpan = span)
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
        public int VectorizedForSum()
        {
            //number of calculators per CPU core in haswell+ architecture
            const int SIZE = 4;

            int result = 0;
            int partial1 = 0;
            int partial2 = 0;
            int partial3 = 0;
            int partial4 = 0;

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
        public unsafe int SimdSum()
        {
            int SIZE = 4;

            int result = 0;
            Vector128<int> vResult = Vector128<int>.Zero;

            var span = data.AsSpan();
            int lastBlockIndex = data.Length - (data.Length % SIZE);

            fixed (int* pSpan = span)
            {
                for (int i = 0; i < lastBlockIndex; i += SIZE)
                {
                    vResult = Avx2.Add(vResult, Avx2.LoadVector128(pSpan + i));
                }
            }

            vResult = Avx2.HorizontalAdd(vResult, vResult);
            vResult = Avx2.HorizontalAdd(vResult, vResult);
            result = vResult.ToScalar();

            for (int i = lastBlockIndex; i < span.Length; i++)
            {
                result += span[i];
            }

            return result;
        }
    }
}
