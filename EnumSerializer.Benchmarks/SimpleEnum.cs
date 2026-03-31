
// (c) 2026 Kazuki Kohzuki

namespace EnumSerializer.Benchmarks;

[EnumSerializable(typeof(DefaultSerializeValueAttribute))]
[EnumSerializable(typeof(IgnoreCaseTestAttribute), CaseSensitive = false)]
internal enum SimpleEnum
{
    [DefaultSerializeValue("Value1")]
    [IgnoreCaseTest("Value1")]
    Value1,

    [DefaultSerializeValue("Value2")]
    [IgnoreCaseTest("Value2")]
    Value2,

    [DefaultSerializeValue("Value3")]
    [IgnoreCaseTest("Value3")]
    Value3,

    [DefaultSerializeValue("Value4")]
    [IgnoreCaseTest("Value4")]
    Value4,

    [DefaultSerializeValue("Value5")]
    [IgnoreCaseTest("Value5")]
    Value5,

    [DefaultSerializeValue("Value6")]
    [IgnoreCaseTest("Value6")]
    Value6,

    [DefaultSerializeValue("Value7")]
    [IgnoreCaseTest("Value7")]
    Value7,

    [DefaultSerializeValue("Value8")]
    [IgnoreCaseTest("Value8")]
    Value8,

    [DefaultSerializeValue("Value9")]
    [IgnoreCaseTest("Value9")]
    Value9,

    [DefaultSerializeValue("Value10")]
    [IgnoreCaseTest("Value10")]
    Value10,
} // internal enum SimpleEnum
