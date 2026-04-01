
// (c) 2026 Kazuki Kohzuki

using EnumSerializer.Utils;
using System.Collections.Generic;

namespace EnumSerializer.Generators;

internal sealed class SerializeValueInfo
{
    internal INamedTypeSymbol AttributeType { get; }

    internal bool CaseSensitive { get; }

    internal ExtensionMethods ExtensionMethods { get; }

    internal bool GenerateToString { get; }

    internal bool GenerateTryParse { get; }

    /// <summary>
    /// Gets the location of the whole attribute application.
    /// </summary>
    /// <remarks>
    /// For <c>[EnumSerializable(typeof(DefaultSerializeValueAttribute))]</c>,
    /// this property locates <c>EnumSerializable(typeof(DefaultSerializeValueAttribute))</c>.
    /// </remarks>
    internal Location? Location { get; }

    /// <summary>
    /// Gets the location of the attribute specifed as the argument of <c>EnumSerializableAttribute</c>.
    /// </summary>
    /// <remarks>
    /// For <c>[EnumSerializable(typeof(DefaultSerializeValueAttribute))]</c>,
    /// this property locates <c>typeof(DefaultSerializeValueAttribute)</c>.
    /// </remarks>
    internal Location? AttributeLocation { get; }

    internal Location? ExtensionMethodsLocation { get; }

    private SerializeValueInfo(INamedTypeSymbol enumType, bool caseSensitive, ExtensionMethods methods, Location? location, Location? attrLocation, Location? extensionMethodsLocation)
    {
        this.AttributeType = enumType;
        this.ExtensionMethods = methods;
        this.CaseSensitive = caseSensitive;
        this.GenerateToString = methods.HasFlag(ExtensionMethods.ToString);
        this.GenerateTryParse = methods.HasFlag(ExtensionMethods.TryParse);
        this.Location = location;
        this.AttributeLocation = attrLocation;
        this.ExtensionMethodsLocation = extensionMethodsLocation;
    } // ctor (INamedTypeSymbol, bool, ExtensionMethods, Location?, Location?)

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

        var location = attribute.ApplicationSyntaxReference?.GetSyntax().GetLocation();
        var attrLocation = argSyntaxList?[0]?.GetLocation();

        return new(enumType, caseSensitive, methods, location, attrLocation, methodLocation);
    } // internal static SerializeValueInfo? Create (AttributeData)

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
