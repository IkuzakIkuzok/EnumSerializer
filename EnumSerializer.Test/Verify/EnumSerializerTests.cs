
// (c) 2026 Kazuki Kohzuki

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace EnumSerializer.Test.Verify;

[VerifyTest(LanguageVersion.CSharp6, LanguageVersion.CSharp9, LanguageVersion.CSharp12, LanguageVersion.CSharp14)]
public sealed partial class EnumSerializerTests
{
    // lang=C#
    [TestSource]
    private static readonly string simpleSwitch = """
        using EnumSerializer;

        namespace Test;

        [EnumSerializable(typeof(DefaultSerializeValueAttribute))]
        internal enum MyEnum
        {
            [DefaultSerializeValueAttribute("val1")]
            Value1,

            [DefaultSerializeValueAttribute("val2")]
            Value2,

            [DefaultSerializeValueAttribute("val3")]
            Value3
        }
        """;

    // lang=C#
    [TestSource]
    private static readonly string ignoreCase = """
        using EnumSerializer;
        
        namespace Test;
        
        [EnumSerializable(typeof(DefaultSerializeValueAttribute), CaseSensitive = false)]
        internal enum MyEnum
        {
            [DefaultSerializeValueAttribute("val1")]
            Value1,
        
            [DefaultSerializeValueAttribute("val2")]
            Value2,
        
            [DefaultSerializeValueAttribute("val3")]
            Value3
        }
        """;

    // lang=C#
    [TestSource]
    private static readonly string differentLength = """
        using EnumSerializer;

        namespace Test;

        [EnumSerializable(typeof(DefaultSerializeValueAttribute))]
        internal enum MyEnum
        {
            [DefaultSerializeValueAttribute("v1")]
            Value1,

            [DefaultSerializeValueAttribute("val2")]
            Value2,

            [DefaultSerializeValueAttribute("value3")]
            Value3
        }
        """;

    // lang=C#
    [TestSource]
    private static readonly string emptyEnum = """
        using EnumSerializer;

        namespace Test;

        [EnumSerializable(typeof(DefaultSerializeValueAttribute))]
        internal enum MyEnum
        {
        }
        """;

    // lang=C#
    [TestSource]
    private static readonly string singleValueEnum = """
        using EnumSerializer;

        namespace Test;

        [EnumSerializable(typeof(DefaultSerializeValueAttribute))]
        internal enum MyEnum
        {
            [DefaultSerializeValueAttribute("val1")]
            Value1
        }
        """;

    // lang=C#
    [TestSource]
    private static readonly string toStringOnly = """
        using EnumSerializer;

        namespace Test;

        [EnumSerializable(typeof(DefaultSerializeValueAttribute), Methods = ExtensionMethods.ToString)]
        internal enum MyEnum
        {
            [DefaultSerializeValueAttribute("val1")]
            Value1,

            [DefaultSerializeValueAttribute("val2")]
            Value2,

            [DefaultSerializeValueAttribute("val3")]
            Value3
        }
        """;

    // lang=C#
    [TestSource]
    private static readonly string tryParseOnly = """
        using EnumSerializer;

        namespace Test;

        [EnumSerializable(typeof(DefaultSerializeValueAttribute), Methods = ExtensionMethods.TryParse)]
        internal enum MyEnum
        {
            [DefaultSerializeValueAttribute("val1")]
            Value1,

            [DefaultSerializeValueAttribute("val2")]
            Value2,

            [DefaultSerializeValueAttribute("val3")]
            Value3
        }
        """;

    // lang=C#
    [TestSource]
    private static readonly string ignoreCaseLong = """
        using EnumSerializer;

        namespace Test;

        [EnumSerializable(typeof(DefaultSerializeValueAttribute), CaseSensitive = false)]
        internal enum MyEnum
        {
            [DefaultSerializeValueAttribute("Short literal")]
            Short,

            // 774 characters
            [DefaultSerializeValueAttribute("An exceptionally elongated string literal meticulously constructed for the explicit purpose of rigorously evaluating the boundary conditions and threshold capacities of high-performance zero-allocation serialization algorithms relying heavily on raw memory manipulation techniques such as ref byte references and span slicing within custom-built Source Generator outputs, specifically intended to trigger dynamic buffer reallocation pathways, aggressively expose potential stack overflow vulnerabilities, and thoroughly validate the robust handling of continuous contiguous memory blocks significantly exceeding the standard two-hundred-and-fifty-six-byte stack-allocated threshold limit without compromising execution speed or introducing hidden garbage collection overhead")]
            VeryLong,
        }
        """;

    // lang=C#
    [TestSource]
    private static readonly string invalidArguments = """
        using EnumSerializer;

        namespace Test;

        [EnumSerializable(typeof(DefaultSerializeValueAttribute))]
        [EnumSerializable(typeof(string))] // Invalid attribute type
        internal enum MyEnum
        {
            [DefaultSerializeValueAttribute("val1")]
            Value1,

            [DefaultSerializeValueAttribute("val2")]
            Value2,

            [DefaultSerializeValueAttribute("val3")]
            Value3
        }
        """;

    // lang=C#
    [TestSource]
    private readonly string nameCollision = """
        using EnumSerializer;
        
        namespace Test;
        
        [EnumSerializable(typeof(DefaultSerializeValueAttribute))]
        internal enum MyEnum
        {
            [DefaultSerializeValueAttribute("val1")]
            Value1,
        
            [DefaultSerializeValueAttribute("val2")]
            Value2,
        
            [DefaultSerializeValueAttribute("val3")]
            Value3
        }

        // Following classes intentionally collide with the generated class name "MyEnumSerializationExtensions"
        // to test the generator's ability to resolve name conflicts by generating unique class names.
        internal class MyEnumSerializationExtensions {}
        internal class MyEnumSerializationExtensions_1 {}
        """;

    // lang=C#
    [TestSource]
    private static readonly string specifiedClassName = """
        using EnumSerializer;

        namespace Test;

        [EnumSerializable(typeof(DefaultSerializeValueAttribute), ExtensionClassName = "CustomClassName")]
        [EnumSerializable(typeof(CustomSerializeValueAttribute), ExtensionClassName = "DifferentCustomClassName")] // This ExtensionClassName is ignored because it's only applicable to the first EnumSerializableAttribute.
        internal enum MyEnum
        {
            [DefaultSerializeValueAttribute("val1")]
            Value1,
        
            [DefaultSerializeValueAttribute("val2")]
            Value2,
        
            [DefaultSerializeValueAttribute("val3")]
            Value3
        }

        internal class CustomSerializeValueAttribute : SerializeValueAttribute
        {
            internal CustomSerializeValueAttribute(string value) : base(value) { }
        }

        // The user-specified class name is always used without modification and the generator does not check for name conflicts.
        internal class CustomClassName {}
        """;

    private static readonly string[] _ignoreFiles = [
        "ExtensionMethods.g.cs",
        "Microsoft.CodeAnalysis.EmbeddedAttribute.cs",
    ];

    private static partial bool IgnoreRule(GeneratedSourceResult result)
    {
        if (result.HintName.EndsWith("Attribute.g.cs", StringComparison.OrdinalIgnoreCase)) return true;
        if (_ignoreFiles.Contains(result.HintName)) return true;

        return false;
    } // private static bool IgnoreRule (GeneratedSourceResult)
} // public sealed class CSharp11Tests
