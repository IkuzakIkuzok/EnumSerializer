
// (c) 2026 Kazuki Kohzuki

using EnumSerializer.Utils;
using System.Collections.Generic;

namespace EnumSerializer.Generators;

internal record EnumInfo(INamedTypeSymbol EnumType, IEnumerable<SerializeValueInfo> SerializeValues, string ExtensionClassName)
{
    internal static EnumInfo Create(GeneratorAttributeSyntaxContext context)
    {
        var enumType = (INamedTypeSymbol)context.TargetSymbol;
        var attrs =
                context.Attributes.Where(a => a.AttributeClass?.FullName == SerializerGenerator.AttributeFullName)
                               .Select(SerializeValueInfo.Create)
                               .OfType<SerializeValueInfo>();

        var className = $"{enumType.Name}SerializationExtensions";

        return new(enumType, [.. attrs], ToUniqueName(className, enumType.ContainingNamespace.ToDisplayString(), context.SemanticModel.Compilation));
    } // internal static EnumInfo Create(GeneratorAttributeSyntaxContext context)

    private static string ToUniqueName(string name, string @namespace, Compilation compilation)
    {
        if (IsUniqueName(name, @namespace, compilation)) return name;

        var suffix = 1;
        string uniqueName;
        while (!(IsUniqueName(uniqueName = $"{name}_{suffix}", @namespace, compilation)))
            ++suffix;
        return uniqueName;
    } // private static string ToUniqueName (string, string, Compilation)

    private static bool IsUniqueName(string name, string @namespace, Compilation compilation)
    {
        var fullName = string.IsNullOrWhiteSpace(@namespace) ? name : $"{@namespace}.{name}";
        return compilation.GetTypeByMetadataName(fullName) is null;
    } // private static bool IsUniqueName (string, string, Compilation)
} // internal record EnumInfo (INamedTypeSymbol, IEnumerable<SerializeValueInfo>, string)
