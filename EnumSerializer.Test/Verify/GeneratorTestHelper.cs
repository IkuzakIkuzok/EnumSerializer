
// (c) 2026 Kazuki Kohzuki

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Runtime.CompilerServices;

namespace EnumSerializer.Test.Verify;

internal static class GeneratorTestHelper
{
    [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
    [return: UnsafeAccessorType("EnumSerializer.Generators.SerializerGenerator, EnumSerializer")]
    private static extern object CreateSerializerGeneratorImpl();

    [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
    [return: UnsafeAccessorType("EnumSerializer.Generators.AttributesGenerator, EnumSerializer")]
    private static extern object CreateAttributesGeneratorImpl();

    internal static IIncrementalGenerator CreateSerializerGenerator() => (IIncrementalGenerator)CreateSerializerGeneratorImpl();

    internal static IIncrementalGenerator CreateAttributesGenerator() => (IIncrementalGenerator)CreateAttributesGeneratorImpl();

    internal static GeneratorDriver GetDriver(string source, LanguageVersion languageVersion)
    {
        var options = new CSharpParseOptions(languageVersion);

        var serializerGenerator = CreateSerializerGenerator();
        var attributesGenerator = CreateAttributesGenerator();

        var syntaxTree = CSharpSyntaxTree.ParseText(source, options, cancellationToken: TestContext.Current.CancellationToken);
        var compilation = CSharpCompilation.Create(
            assemblyName: "TestComp",
            options: new(OutputKind.DynamicallyLinkedLibrary),
            syntaxTrees: [syntaxTree]
        )
            .AddReferences(
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(serializerGenerator.GetType().Assembly.Location)
            );

        return CSharpGeneratorDriver.Create(attributesGenerator, serializerGenerator)
                                    .WithUpdatedParseOptions(options)
                                    .RunGenerators(compilation, cancellationToken: TestContext.Current.CancellationToken);
    } // internal static GeneratorDriver GetDriver (string, LanguageVersion)
} // internal static class IncrementalGeneratorHelper
