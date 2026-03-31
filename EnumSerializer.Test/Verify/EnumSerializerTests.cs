
// (c) 2026 Kazuki Kohzuki

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Runtime.CompilerServices;

namespace EnumSerializer.Test.Verify;

public sealed class EnumSerializerTests
{
    [Theory]
    [InlineData(LanguageVersion.CSharp6)]
    [InlineData(LanguageVersion.CSharp9)]
    [InlineData(LanguageVersion.CSharp12)]
    [InlineData(LanguageVersion.CSharp14)]
    internal Task SimpleSwitch(LanguageVersion languageVersion)
    {
        // lang=C#
        const string source = """
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

        return RunTest(source, languageVersion);
    } // internal Task SimpleSwitch (LanguageVersion)

    [Theory]
    [InlineData(LanguageVersion.CSharp6)]
    [InlineData(LanguageVersion.CSharp9)]
    [InlineData(LanguageVersion.CSharp12)]
    [InlineData(LanguageVersion.CSharp14)]
    internal Task IgnoreCase(LanguageVersion languageVersion)
    {
        // lang=C#
        const string source = """
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

        return RunTest(source, languageVersion);
    } // internal Task IgnoreCase (LanguageVersion)

    [Theory]
    [InlineData(LanguageVersion.CSharp6)]
    [InlineData(LanguageVersion.CSharp9)]
    [InlineData(LanguageVersion.CSharp12)]
    [InlineData(LanguageVersion.CSharp14)]
    internal Task DifferentLength(LanguageVersion languageVersion)
    {
        // lang=C#
        const string source = """
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
        return RunTest(source, languageVersion);
    } // internal Task DifferentLength (LanguageVersion)

    [Theory]
    [InlineData(LanguageVersion.CSharp6)]
    [InlineData(LanguageVersion.CSharp9)]
    [InlineData(LanguageVersion.CSharp12)]
    [InlineData(LanguageVersion.CSharp14)]
    internal Task EmptyEnum(LanguageVersion languageVersion)
    {
        // lang=C#
        const string source = """
            using EnumSerializer;

            namespace Test;

            [EnumSerializable(typeof(DefaultSerializeValueAttribute))]
            internal enum MyEnum
            {
            }
            """;

        return RunTest(source, languageVersion);
    } // internal Task EmptyEnum (LanguageVersion)

    [Theory]
    [InlineData(LanguageVersion.CSharp6)]
    [InlineData(LanguageVersion.CSharp9)]
    [InlineData(LanguageVersion.CSharp12)]
    [InlineData(LanguageVersion.CSharp14)]
    internal Task SingleValueEnum(LanguageVersion languageVersion)
    {
        // lang=C#
        const string source = """
            using EnumSerializer;

            namespace Test;

            [EnumSerializable(typeof(DefaultSerializeValueAttribute))]
            internal enum MyEnum
            {
                [DefaultSerializeValueAttribute("val1")]
                Value1
            }
            """;
        return RunTest(source, languageVersion);
    } // internal Task SingleValueEnum (LanguageVersion)

    [Theory]
    [InlineData(LanguageVersion.CSharp6)]
    [InlineData(LanguageVersion.CSharp9)]
    [InlineData(LanguageVersion.CSharp12)]
    [InlineData(LanguageVersion.CSharp14)]
    internal Task ToStringOnly(LanguageVersion languageVersion)
    {
        // lang=C#
        const string source = """
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

        return RunTest(source, languageVersion);
    } // internal Task ToStringOnly (LanguageVersion)

    [Theory]
    [InlineData(LanguageVersion.CSharp6)]
    [InlineData(LanguageVersion.CSharp9)]
    [InlineData(LanguageVersion.CSharp12)]
    [InlineData(LanguageVersion.CSharp14)]
    internal Task TryParseOnly(LanguageVersion languageVersion)
    {
        // lang=C#
        const string source = """
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

        return RunTest(source, languageVersion);
    } // internal Task TryParseOnly (LanguageVersion)

    [Theory]
    [InlineData(LanguageVersion.CSharp6)]
    [InlineData(LanguageVersion.CSharp9)]
    [InlineData(LanguageVersion.CSharp12)]
    [InlineData(LanguageVersion.CSharp14)]
    internal Task IgnoreCaseLong(LanguageVersion languageVersion)
    {
        // lang=C#
        const string source = """
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

        return RunTest(source, languageVersion);
    } // internal Task LongLiterals (LanguageVersion)

    private static Task RunTest(string source, LanguageVersion languageVersion, [CallerMemberName] string testName = "")
    {
        var driver = GeneratorTestHelper.GetDriver(source, languageVersion);
        var results = driver.GetRunResult();
        var targetSource =
            results.Results.SelectMany(r => r.GeneratedSources)
                           .Single(s => !IgnoreRule(s))
                           .SourceText.ToString();
        return Verifier.Verify(target: targetSource, extension: "cs")
                       .UseDirectory($"Snapshots/{testName}")
                       .UseFileName(languageVersion.ToString());
    } // private static Task RunTest (string, LanguageVersion, [string])

    private static readonly string[] _ignoreFiles = [
        "ExtensionMethods.g.cs"
    ];

    private static bool IgnoreRule(GeneratedSourceResult result)
    {
        if (result.HintName.EndsWith("Attribute.g.cs", StringComparison.OrdinalIgnoreCase)) return true;
        if (_ignoreFiles.Contains(result.HintName)) return true;

        return false;
    } // private static bool IgnoreRule (GeneratedSourceResult)
} // public sealed class CSharp11Tests
