using BenchmarkDotNet.Attributes;
using System.Collections;
using System.Collections.Frozen;
using System.Collections.Immutable;

namespace CollectionTests;

public class HashSet
{
    private const int N = 10000;
    private const int TARGET = N + 1;
    private readonly List<int> source;
    private readonly FrozenSet<int> frozenData;
    private readonly HashSet<int> mutableData;
    private readonly ImmutableHashSet<int> immutableData;
    private readonly ImmutableHashSetWrapper<int> immutableIEnumerableData;

    [Params(TARGET)]
    public int Target { get; set; }

    public HashSet()
    {
        source = [.. Enumerable.Range(0, N)];
        source.Add(TARGET);
        source.AddRange(Enumerable.Range(N * 2, N * 3));

        frozenData = [.. source];
        mutableData = [.. source];
        immutableData = [.. source];
        immutableIEnumerableData = new ImmutableHashSetWrapper<int>(immutableData);
    }

    [Benchmark(Baseline = true)]
    public int ImmutableHastSetWhereEvenFirst() => immutableData.Where(b => b % 2 == 1).FirstOrDefault(b => b == Target);
    [Benchmark]
    public int ImmutableHastSetWhereEvenForEach() { foreach (var b in immutableData) if (b % 2 == 1 && b == Target) return b; return 0; }
    [Benchmark]
    public int ImmutableIEnumerableWhereEvenFirst() => immutableIEnumerableData.Where(b => b % 2 == 1).FirstOrDefault(b => b == Target);

    [Benchmark]
    public int MutableHastSetWhereEvenFirst() => mutableData.Where(b => b % 2 == 1).FirstOrDefault(b => b == Target);
    [Benchmark]
    public int MutableHastSetWhereEvenForEach() { foreach (var b in mutableData) if (b % 2 == 1 && b == Target) return b; return 0; }

    [Benchmark]
    public int FrozenSetWhereEvenFirst() => frozenData.Where(b => b % 2 == 1).FirstOrDefault(b => b == Target);
    [Benchmark]
    public int FrozenSetWhereEvenForEach() { foreach (var b in frozenData) if (b % 2 == 1 && b == Target) return b; return 0; }

    [Benchmark]
    public int ListWhereEvenFirst() => source.Where(b => b % 2 == 1).FirstOrDefault(b => b == Target);
    [Benchmark]
    public int ListSetWhereEvenForEach() { foreach (var b in source) if (b % 2 == 1 && b == Target) return b; return 0; }

    private class ImmutableHashSetWrapper<T>(ImmutableHashSet<T> set) : IEnumerable<T>
    {
        public IEnumerator<T> GetEnumerator() => set.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}