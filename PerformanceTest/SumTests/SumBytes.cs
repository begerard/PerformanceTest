using BenchmarkDotNet.Attributes;

namespace SumTests;

public class SumBytes
{
    private const int N = 10000;
    private readonly byte[] data;

    public SumBytes()
    {
        data = new byte[N];
        new Random(1).NextBytes(data);
    }

    [Benchmark]
    public int LinqSumCast() => data.Sum(i => i);

    [Benchmark(Baseline = true)]
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
    public int SpanForSum()
    {
        int result = 0;
        var span = data.AsSpan();

        for (int i = 0; i < span.Length; i++)
        {
            result += span[i];
        };

        return result;
    }

    [Benchmark]
    public int UnrolledForSum()
    {
        //number of unrolling
        const int SIZE = 4;

        int result = 0;
        var span = data.AsSpan();
        int lastBlockIndex = data.Length - (data.Length % SIZE);

        for (int i = 0; i < lastBlockIndex; i += 4)
        {
            result += span[i + 0];
            result += span[i + 1];
            result += span[i + 2];
            result += span[i + 3];
        }

        for (int i = lastBlockIndex; i < span.Length; i++)
        {
            result += span[i];
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
        var slice1 = span[..sliceLenght];
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
}
