
// (c) 2026 Kazuki Kohzuki

using DiffEngine;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Runtime.CompilerServices;

namespace EnumSerializer.Test;

public sealed class CSharp11Tests
{
    private static readonly CSharpParseOptions _options = new(LanguageVersion.CSharp11);

    [ModuleInitializer]
    internal static void Init()
    {
        VerifySourceGenerators.Initialize();
        DiffTools.UseOrder(DiffTool.VisualStudio, DiffTool.VisualStudioCode);
    } // internal static void Init ()

    [Fact]
    internal Task GenerateSimpleSwitchCaseSensitive()
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

        var serializerGenerator = GeneratorTestHelper.CreateSerializerGenerator();
        var attributesGenerator = GeneratorTestHelper.CreateAttributesGenerator();

        var syntaxTree = CSharpSyntaxTree.ParseText(source, _options, cancellationToken: TestContext.Current.CancellationToken);
        var compilation = CSharpCompilation.Create(
            assemblyName: "TestComp",
            options: new(OutputKind.DynamicallyLinkedLibrary),
            syntaxTrees: [syntaxTree]
        )
            .AddReferences(
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(serializerGenerator.GetType().Assembly.Location)
            );

        var driver =
            CSharpGeneratorDriver.Create(attributesGenerator, serializerGenerator)
                                 .WithUpdatedParseOptions(_options)
                                 .RunGenerators(compilation, cancellationToken: TestContext.Current.CancellationToken);

        return Verify(driver).UseDirectory("Snapshots/CSharp11");
    } // internal Task GenerateSimpleSwitchCaseSensitive ()
} // public sealed class CSharp11Tests
