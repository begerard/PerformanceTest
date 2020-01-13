using System;

using BenchmarkDotNet.Running;

namespace SumTests
{
    class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            var instance = new SumBytes();
            instance.VectorizedForSum();
#endif

#if RELEASE
            BenchmarkRunner.Run<SumBytes>();
            BenchmarkRunner.Run<SumInt>();
#endif
        }
    }
}
