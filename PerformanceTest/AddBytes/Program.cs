using BenchmarkDotNet.Running;

namespace SumTests
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<SumBytes>();
        }
    }
}
