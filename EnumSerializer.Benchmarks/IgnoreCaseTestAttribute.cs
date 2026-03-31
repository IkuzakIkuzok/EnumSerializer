
// (c) 2026 Kazuki Kohzuki

namespace EnumSerializer.Benchmarks;

internal sealed class IgnoreCaseTestAttribute : SerializeValueAttribute
{
    internal IgnoreCaseTestAttribute(string value) : base(value) { }
} // internal sealed class IgnoreCaseTestAttribute
