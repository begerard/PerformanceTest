using BenchmarkDotNet.Attributes;
using System.Text;

namespace CollectionTests;

[MemoryDiagnoser(false)]
public class FormatString
{
    private static readonly CompositeFormat FORMAT = CompositeFormat.Parse(" {0:F0}fr");
    private static readonly StringBuilder SB = new();

    [ParamsSource(nameof(ValuesForTimeSpans))]
    public List<TimeSpan> _listTimeSpan;
    public static IEnumerable<List<TimeSpan>> ValuesForTimeSpans =>
    [
        [.. Enumerable.Range(0, 10).Select(i => new TimeSpan(i))],
        [.. Enumerable.Range(0, 10_000).Select(i => new TimeSpan(i))],
    ];

    [Benchmark]
    public string StringConcat()
    {
        return string.Concat(_listTimeSpan.Select(e => $" {e.TotalMinutes:F0}fr"));
    }

    [Benchmark]
    public string StringBuild()
    {
        var sb = SB.Clear();
        foreach (var column in _listTimeSpan) sb.AppendFormat(null, FORMAT, column.TotalMinutes);
        return sb.ToString();
    }
}