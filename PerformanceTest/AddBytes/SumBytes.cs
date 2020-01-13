using System;
using System.Linq;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

using BenchmarkDotNet.Attributes;

namespace SumTests
{
    public class SumBytes
    {
        private const int N = 10000;
        private readonly byte[] data;

        public SumBytes()
        {
            data = new byte[N];
            new Random(1).NextBytes(data);
        }

        //[Benchmark]
        public int LinqSum()
        {
            return data.Sum(i => i);
        }

        //[Benchmark]
        public int ForSum()
        {
            int result = 0;

            for (int i = 0; i < data.Length; i++)
            {
                result += data[i];
            };

            return result;
        }

        //[Benchmark]
        public unsafe int FixedForSum()
        {
            int result = 0;
            var span = data.AsSpan();

            fixed (byte* pSpan = span)
            {
                for (int i = 0; i < span.Length; i++)
                {
                    result += span[i];
                };
            }

            return result;
        }

        //[Benchmark]
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

            Span<byte> span = data.AsSpan<byte>();
            Span<byte> slice1 = span.Slice(0, sliceLenght);
            Span<byte> slice2 = span.Slice(sliceLenght, sliceLenght);
            Span<byte> slice3 = span.Slice(sliceLenght * 2, sliceLenght);
            Span<byte> slice4 = span.Slice(sliceLenght * 3, sliceLenght);

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
            if (!Avx2.IsSupported) Console.WriteLine("No Avx2 support :(");
            //number of Byte per Vector256
            const int SIZE = 32;

            int result = 0;
            Vector256<byte> vResult = Vector256<byte>.Zero;

            var span = data.AsSpan();
            int lastBlockIndex = data.Length - (data.Length % SIZE);

            fixed (byte* pSpan = span)
            {
                for (int i = 0; i < lastBlockIndex; i += SIZE)
                {
                    vResult = Avx2.Add(vResult, Avx2.LoadVector256(pSpan + i));
                }
            }

            return result;
        }
    }
}