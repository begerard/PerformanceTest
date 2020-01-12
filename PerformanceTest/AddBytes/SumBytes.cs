using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;

namespace SumTests
{
    public class SumBytes
    {
        private const int N = 10000;
        private readonly Consumer consumer = new Consumer();

        private readonly byte[] data1;
        private readonly byte[] data2;

        public SumBytes()
        {
            data1 = new byte[N];
            new Random(1).NextBytes(data1);

            data2 = new byte[N];
            new Random(1).NextBytes(data2);
        }

        [Benchmark]
        public void LinqSum()
        {
            data1.Zip(data2, (i, j) => (byte)(i + j)).Consume(consumer);
        }
    }
}