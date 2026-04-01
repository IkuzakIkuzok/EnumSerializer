
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

        var caseSensitive = true;
        if (attribute.TryGetNamedArgumentValue("CaseSensitive", out bool cs))
            caseSensitive = cs;

        var methods = ExtensionMethods.All;
        Location? methodLocation = null;
        if (attribute.TryGetNamedArgumentEnumValue<ExtensionMethods>("Methods", out var m))
        {
            methods = m;
            methodLocation = GetAttributeNamedArgumentSyntax(attribute, "Methods")?.GetLocation();
        }

        var location = attribute.ApplicationSyntaxReference?.GetSyntax().GetLocation();
        var attrLocation = GetAttributeArgumentSyntax(attribute, 0)?.GetLocation();

        return new(enumType, caseSensitive, methods, location, attrLocation, methodLocation);
    } // internal static SerializeValueInfo? Create (AttributeData)

    private static AttributeArgumentSyntax? GetAttributeArgumentSyntax(AttributeData attribute, int index)
    {
        if (attribute.ApplicationSyntaxReference?.GetSyntax() is not AttributeSyntax attributeSyntax) return null;
        if (attributeSyntax.ArgumentList is not AttributeArgumentListSyntax argumentList) return null;
        if ((uint)index >= (uint)argumentList.Arguments.Count) return null;
        return argumentList.Arguments[index];
    } // private static AttributeArgumentSyntax? GetAttributeArgumentSyntax (AttributeData, int)

    private static AttributeArgumentSyntax? GetAttributeNamedArgumentSyntax(AttributeData attribute, string name)
    {
        if (attribute.ApplicationSyntaxReference?.GetSyntax() is not AttributeSyntax attributeSyntax) return null;
        if (attributeSyntax.ArgumentList is not AttributeArgumentListSyntax argumentList) return null;

        foreach (var argument in argumentList.Arguments)
        {
            if (argument.NameEquals?.Name.Identifier.Text == name)
                return argument;
        }

        return null;
    } // private static AttributeArgumentSyntax? GetAttributeNamedArgumentSyntax (AttributeData, string)

    internal sealed class EqualityComparer : IEqualityComparer<SerializeValueInfo>
    {
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
