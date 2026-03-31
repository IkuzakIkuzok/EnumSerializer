
// (c) 2026 Kazuki Kohzuki

using BenchmarkDotNet.Attributes;

namespace EnumSerializer.Benchmarks;

[MemoryDiagnoser]
public class ParseBenchmark
{
    private static readonly string[] _values = [
        .. Enum.GetValues<SimpleEnum>().Select(f => f.ToDefaultSerializeValue())
    ];

#pragma warning disable CA1822

    [Benchmark(Baseline = true)]
    public void ParseStandard()
    {
        foreach (var value in _values)
            _ = Enum.TryParse<SimpleEnum>(value, ignoreCase: false, out _);
    } // public void ParseStandard()

    [Benchmark]
    public void ParseGenerated()
    {
        foreach (var value in _values)
            _ = value.TryParseSimpleEnumFromDefaultSerializeValue(out _);
    } // public void ParseGenerated()

    [Benchmark]
    public void ParseStandardIgnoreCase()
    {
        foreach (var value in _values)
            _ = Enum.TryParse<SimpleEnum>(value, ignoreCase: true, out _);
    } // public void ParseStandardIgnoreCase()

    [Benchmark]
    public void ParseGeneratedIgnoreCase()
    {
        foreach (var value in _values)
            _ = value.TryParseSimpleEnumFromIgnoreCaseTest(out _);
    } // public void ParseGeneratedIgnoreCase()

#pragma warning restore CA1822
} // public class ParseBenchmark
