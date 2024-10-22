using System;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using Perfolizer.Horology;

namespace SumTests
{
    class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            //var instance1 = new SumBytes();
            //instance1.LinqSumCast();
            //instance1.SpanForSum();
            //instance1.ForSum();
            //instance1.UnrolledForSum();
            //instance1.VectorizedForSum();

            //var instance2 = new SumInt();
            //instance2.ForSum();
            //instance2.UnrolledForSum();
            //instance2.VectorizedForSum();
            //instance2.SseSum();
            //instance2.AvxSum();

            //var instance3 = new SumLong();
            //instance3.ForSum();
            //instance3.UnrolledForSum();
            //instance3.VectorizedForSum();
            //instance3.SseSum();
            //instance3.AvxSum();

            var instance4 = new SumBig();
            instance4.ForSum();
#endif

#if RELEASE
            IConfig config = DefaultConfig.Instance.WithSummaryStyle(SummaryStyle.Default.WithTimeUnit(TimeUnit.Microsecond));

            //BenchmarkRunner.Run<SumBytes>(config);
            //BenchmarkRunner.Run<SumInt>(config);
            //BenchmarkRunner.Run<SumLong>(config);
            BenchmarkRunner.Run<SumBig>(config);
#endif
        }
    }
}
