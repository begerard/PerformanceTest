using System;

using BenchmarkDotNet.Running;

namespace SumTests
{
    class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            var instance1 = new SumBytes();
            instance1.ForSum();
            instance1.UnrolledForSum();
            instance1.VectorizedForSum();

            var instance2 = new SumInt();
            instance2.ForSum();
            instance2.UnrolledForSum();
            instance2.VectorizedForSum();
            instance2.SseSum();
            instance2.AvxSum();

            var instance3 = new SumLong();
            instance3.ForSum();
            instance3.UnrolledForSum();
            instance3.VectorizedForSum();
            instance3.SseSum();
            instance3.AvxSum();
#endif

#if RELEASE
            BenchmarkRunner.Run<SumBytes>();
            BenchmarkRunner.Run<SumInt>();
            BenchmarkRunner.Run<SumLong>();
#endif
        }
    }
}
