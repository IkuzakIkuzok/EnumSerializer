
// (c) 2026 Kazuki Kohzuki

namespace EnumSerializer.Test.UnitTest;

[EnumSerializable(typeof(DefaultSerializeValueAttribute))]
[EnumSerializable(typeof(IgnoreCaseTestAttribute), CaseSensitive = false)]
internal enum SimpleEnum
{
    [DefaultSerializeValue("val1")]
    [IgnoreCaseTest("VAL1")]
    Value1,

    [DefaultSerializeValue("val2")]
    [IgnoreCaseTest("VAL2")]
    Value2,

    [DefaultSerializeValue("val3")]
    [IgnoreCaseTest("VAL3")]
    Value3,
} // internal enum SimpleEnum
