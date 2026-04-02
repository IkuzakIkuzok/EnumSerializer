
// (c) 2026 Kazuki Kohzuki

using EnumSerializer.Utils;
using System.Collections.Generic;

namespace EnumSerializer.Generators;

internal sealed class SerializeValueInfo
{
    required internal INamedTypeSymbol AttributeType { get; init; }

    required internal bool CaseSensitive { get; init; }

    required internal string? ExtensionClassName { get; init; }

    required internal ExtensionMethods ExtensionMethods { get; init; }

    required internal bool GenerateToString { get; init; }

    required internal bool GenerateTryParse { get; init; }

    /// <summary>
    /// Gets the location of the whole attribute application.
    /// </summary>
    /// <remarks>
    /// For <c>[EnumSerializable(typeof(DefaultSerializeValueAttribute))]</c>,
    /// this property locates <c>EnumSerializable(typeof(DefaultSerializeValueAttribute))</c>.
    /// </remarks>
    required internal Location? Location { get; init; }

    /// <summary>
    /// Gets the location of the attribute specifed as the argument of <c>EnumSerializableAttribute</c>.
    /// </summary>
    /// <remarks>
    /// For <c>[EnumSerializable(typeof(DefaultSerializeValueAttribute))]</c>,
    /// this property locates <c>typeof(DefaultSerializeValueAttribute)</c>.
    /// </remarks>
    required internal Location? AttributeLocation { get; init; }

    required internal Location? ClassNameLocation { get; init; }

    required internal Location? ExtensionMethodsLocation { get; init; }

    private SerializeValueInfo() { }

    internal static SerializeValueInfo? Create(AttributeData attribute)
    {
        var args = attribute.ConstructorArguments;
        if (args.Length == 0) return default;
        if (args[0].Value is not INamedTypeSymbol enumType) return default;

        var argSyntaxList = attribute.ArgumentSyntaxList;

        var caseSensitive = true;
        if (attribute.TryGetNamedArgumentValue("CaseSensitive", out bool cs))
            caseSensitive = cs;

        var methods = ExtensionMethods.All;
        Location? methodLocation = null;
        if (attribute.TryGetNamedArgumentEnumValue<ExtensionMethods>("Methods", out var m))
        {
            methods = m;
            methodLocation = argSyntaxList?["Methods"]?.GetLocation();
        }

        Location? classNameLocation = null;
        if (attribute.TryGetNamedArgumentValue("ExtensionClassName", out string? className))
            classNameLocation = argSyntaxList?["ExtensionClassName"]?.GetLocation();
        else
            className = null;


        var location = attribute.ApplicationSyntaxReference?.GetSyntax().GetLocation();
        var attrLocation = argSyntaxList?[0]?.GetLocation();

        return new()
        {
            AttributeType = enumType,
            CaseSensitive = caseSensitive,
            ExtensionClassName = className,
            ExtensionMethods = methods,
            GenerateToString = methods.HasFlag(ExtensionMethods.ToString),
            GenerateTryParse = methods.HasFlag(ExtensionMethods.TryParse),
            Location = location,
            AttributeLocation = attrLocation,
            ClassNameLocation = classNameLocation,
            ExtensionMethodsLocation = methodLocation
        };
    } // internal static SerializeValueInfo? Create (AttributeData, Compilation)

    internal sealed class EqualityComparer : IEqualityComparer<SerializeValueInfo>
    {
        internal static EqualityComparer Default => field ??= new();

        private EqualityComparer() { }

        public bool Equals(SerializeValueInfo? x, SerializeValueInfo? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null || y is null) return false;
            return SymbolEqualityComparer.Default.Equals(x.AttributeType, y.AttributeType);
        } // public bool Equals (SerializeValueInfo?, SerializeValueInfo?)

        public int GetHashCode(SerializeValueInfo obj)
            => SymbolEqualityComparer.Default.GetHashCode(obj.AttributeType);
    } // internal sealed class EqualityComparer : IEqualityComparer<SerializeValueInfo>
} // internal sealed class SerializeValueInfo
