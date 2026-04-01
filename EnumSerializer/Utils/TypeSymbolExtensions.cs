
// (c) 2026 Kazuki Kohzuki

using System.Collections.Generic;

namespace EnumSerializer.Utils;

internal static class TypeSymbolExtensions
{
    extension (ITypeSymbol symbol)
    {
        #region name

        /// <summary>
        /// Gets the fully qualified name of the symbol, excluding the global namespace prefix.
        /// </summary>
        internal string FullName
            => symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted));

        /// <summary>
        /// Gets the fully qualified name of the symbol, including the global namespace.
        /// </summary>
        internal string FullyQualifiedName
            => symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Included));

        #endregion name

        #region inheritance

        internal bool InheritsFrom(string baseTypeFullName)
        {
            var baseType = symbol.BaseType;
            while (baseType is not null)
            {
                if (baseType.FullyQualifiedName == baseTypeFullName) return true;
                baseType = baseType.BaseType;
            }
            return false;
        } // internal bool InheritsFrom (string)

        #endregion inheritance

        internal IEnumerable<IFieldSymbol> StaticFields
            => symbol.GetMembers().OfType<IFieldSymbol>().Where(f => f.IsStatic);
    }
} // internal static class NamedTypeSymbolExtensions
