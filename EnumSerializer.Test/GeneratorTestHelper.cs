
// (c) 2026 Kazuki Kohzuki

using Microsoft.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace EnumSerializer.Test;

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
} // internal static class IncrementalGeneratorHelper
