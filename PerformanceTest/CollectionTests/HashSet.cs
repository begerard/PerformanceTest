using BenchmarkDotNet.Attributes;
using System.Collections;
using System.Collections.Immutable;

namespace CollectionTests;

[MemoryDiagnoser(false)]
public class HashSet
{
    private const int N = 10000;
    private const int TARGET = N + 1;
    private readonly List<int> source;
    private readonly HashSet<int> mutableData;
    private readonly HashSetWrapper<int> mutableIEnumerableData;
    private readonly ImmutableHashSet<int> immutableData;
    private readonly ImmutableHashSetWrapper<int> immutableIEnumerableData;

    [Params(TARGET)]
    public int Target { get; set; }

    public HashSet()
    {
        source = [.. Enumerable.Range(0, N)];
        source.Add(TARGET);
        source.AddRange(Enumerable.Range(N * 2, N * 3));

        mutableData = [.. source];
        mutableIEnumerableData = new HashSetWrapper<int>(mutableData);

        immutableData = [.. source];
        immutableIEnumerableData = new ImmutableHashSetWrapper<int>(immutableData);
    }

    [Benchmark(Baseline = true)]
    public int mutableHastSetFirst() => mutableData.FirstOrDefault(b => b == Target);
    [Benchmark]
    public int mutableIEnumerableFirst() => mutableIEnumerableData.FirstOrDefault(b => b == Target);

    [Benchmark]
    public int immutableHastSetFirst() => immutableData.FirstOrDefault(b => b == Target);
    [Benchmark]
    public int immutableIEnumerableFirst() => immutableIEnumerableData.FirstOrDefault(b => b == Target);

    [Benchmark]
    public int mutableHastSetWhereEvenFirst() => mutableData.Where(b => b % 2 == 1).FirstOrDefault(b => b == Target);
    [Benchmark]
    public int mutableIEnumerableWhereEvenFirst() => mutableIEnumerableData.Where(b => b % 2 == 1).FirstOrDefault(b => b == Target);
    [Benchmark]
    public int mutableIEnumerableWhereEvenForEach() { foreach (var b in mutableIEnumerableData) if (b % 2 == 1 && b == Target) return b; return 0; }

    [Benchmark]
    public int immutableHastSetWhereEvenFirst() => immutableData.Where(b => b % 2 == 1).FirstOrDefault(b => b == Target);
    [Benchmark]
    public int immutableIEnumerableWhereEvenFirst() => immutableIEnumerableData.Where(b => b % 2 == 1).FirstOrDefault(b => b == Target);
    [Benchmark]
    public int immutableIEnumerableWhereEvenForEach() { foreach (var b in immutableIEnumerableData) if (b % 2 == 1 && b == Target) return b; return 0; }

    private class HashSetWrapper<T>(HashSet<T> set) : IEnumerable<T>
    {
        public IEnumerator<T> GetEnumerator() => set.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    private class ImmutableHashSetWrapper<T>(ImmutableHashSet<T> set) : IEnumerable<T>
    {
        public IEnumerator<T> GetEnumerator() => set.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}