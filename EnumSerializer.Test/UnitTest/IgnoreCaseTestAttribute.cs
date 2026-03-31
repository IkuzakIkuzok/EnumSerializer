
// (c) 2026 Kazuki Kohzuki

namespace EnumSerializer.Test.UnitTest;

internal sealed class IgnoreCaseTestAttribute : SerializeValueAttribute
{
    internal IgnoreCaseTestAttribute(string value) : base(value) { }
} // internal sealed class IgnoreCaseTestAttribute
