
// (c) 2026 Kazuki Kohzuki

using System.Collections.Generic;

namespace EnumSerializer.Generators;

internal sealed class EnumSerializationInfo
{
    internal INamedTypeSymbol EnumType { get; }

    internal bool CaseSensitive { get; }

    internal ExtensionMethods ExtensionMethods { get; }

    internal bool GenerateToString { get; }

    internal bool GenerateTryParse { get; }

    internal EnumSerializationInfo(INamedTypeSymbol enumType, bool caseSensitive, ExtensionMethods methods)
    {
        this.EnumType = enumType;
        this.ExtensionMethods = methods;
        this.CaseSensitive = caseSensitive;
        this.GenerateToString = methods.HasFlag(ExtensionMethods.ToString);
        this.GenerateTryParse = methods.HasFlag(ExtensionMethods.TryParse);
    } // ctor (INamedTypeSymbol, bool, ExtensionMethods)

    internal sealed class EqualityComparer : IEqualityComparer<EnumSerializationInfo>
    {
        public bool Equals(EnumSerializationInfo? x, EnumSerializationInfo? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null || y is null) return false;
            return SymbolEqualityComparer.Default.Equals(x.EnumType, y.EnumType);
        } // public bool Equals (EnumSerializationInfo?, EnumSerializationInfo?)

        public int GetHashCode(EnumSerializationInfo obj)
            => SymbolEqualityComparer.Default.GetHashCode(obj.EnumType);
    } // internal sealed class EqualityComparer : IEqualityComparer<EnumSerializationInfo>
} // internal sealed class EnumSerializationInfo
