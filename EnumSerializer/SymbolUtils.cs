
// (c) 2025 Kazuki Kohzuki

//using SourceGeneratorUtils;

namespace EnumSerializer;

/// <summary>
/// Provides utility methods for working with Roslyn symbols.
/// </summary>
internal static class SymbolUtils
{
    #region Symbol name

    /// <summary>
    /// Gets the fully qualified name of the attribute.
    /// </summary>
    /// <param name="attribute">The attribute.</param>
    /// <param name="context">The context.</param>
    /// <returns>The fully qualified name.</returns>
    internal static string GetGetFullyQualifiedName(this global::Microsoft.CodeAnalysis.CSharp.Syntax.AttributeSyntax attribute, global::Microsoft.CodeAnalysis.Diagnostics.SyntaxNodeAnalysisContext context)
    {
        var symbol = context.SemanticModel.GetSymbolInfo(attribute).Symbol;
        return symbol?.GetFullName() ?? string.Empty;
    } // internal static string GetGetFullyQualifiedName (AttributeSyntax, SyntaxNodeAnalysisContext)

    /// <summary>
    /// Gets the full name of a named type symbol.
    /// </summary>
    /// <param name="symbol">The named type symbol.</param>
    /// <returns>The full name of the symbol without 'global::'.</returns>
    internal static string? GetFullName(this global::Microsoft.CodeAnalysis.ISymbol? symbol)
        => symbol?.ToDisplayString(global::Microsoft.CodeAnalysis.SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(global::Microsoft.CodeAnalysis.SymbolDisplayGlobalNamespaceStyle.Omitted));

    /// <summary>
    /// Obtains the fully qualified name of the class, including 'global::' prefix.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <returns>The fully qualified name.</returns>
    [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull(nameof(symbol))]
    internal static string? GetFullyQualifiedName(this global::Microsoft.CodeAnalysis.ISymbol? symbol)
        => symbol?.ToDisplayString(global::Microsoft.CodeAnalysis.SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(global::Microsoft.CodeAnalysis.SymbolDisplayGlobalNamespaceStyle.Included));

    #endregion Symbol name

    #region Attribute argument

    /// <summary>
    /// Tries to get the value of a named argument from an attribute.
    /// </summary>
    /// <typeparam name="T">The expected type of the argument value.</typeparam>
    /// <param name="attribute">The attribute data.</param>
    /// <param name="argumentName">The name of the argument to retrieve.</param>
    /// <param name="value">When this method returns, contains the value of the named argument if found and successfully cast to the expected type; otherwise, the default value of <typeparamref name="T"/>.</param>
    /// <returns><see langword="true"/> if the named argument is found and its value can be cast to the expected type; otherwise, <see langword="false"/>.</returns>
    internal static bool TryGetNamedArgumentValue<T>(this global::Microsoft.CodeAnalysis.AttributeData attribute, string argumentName, out T? value)
    {
        foreach (var namedArg in attribute.NamedArguments)
        {
            if (namedArg.Key != argumentName) continue;
            if (namedArg.Value.Value is not T typedValue) break; // If the argument exists but cannot be cast to the expected type, treat it as not found.
            value = typedValue;
            return true;
        }
        value = default;
        return false;
    } // internal static bool TryGetNamedArgumentValue<T> (this AttributeData, string, out T)

    internal static bool TryGetNamedArgumentEnumValue<TEnum>(this global::Microsoft.CodeAnalysis.AttributeData attribute, string argumentName, out TEnum value) where TEnum : struct, global::System.Enum
    {
        foreach (var namedArg in attribute.NamedArguments)
        {
            if (namedArg.Key != argumentName) continue;
            if (namedArg.Value.Value is not int intValue) break; // Enum values are represented as integers in attribute data.
            if (!EnumCache<TEnum>.ValidValues.Contains(intValue)) break; // Check if the integer value corresponds to a defined enum member.
            value = (TEnum)(object)intValue; // Cast the integer to the enum type.
            return true;
        }
        value = default;
        return false;
    } // internal static bool TryGetNamedArgumentEnumValue<TEnum> (this AttributeData, string, out TEnum)

    private static class EnumCache<T> where T : struct, global::System.Enum
    {
        public static readonly global::System.Collections.Generic.HashSet<int> ValidValues =
            new(global::System.Linq.Enumerable.Cast<int>(global::System.Enum.GetValues(typeof(T))));
    }

    #endregion Attribute argument

    /// <summary>
    /// Checks if a named type symbol inherits from a specified base type by its full name.
    /// </summary>
    /// <param name="symbol">The named type symbol to check.</param>
    /// <param name="baseFullName">The full name of the base type to check against.</param>
    /// <returns><see langword="true"/> if the symbol inherits from the specified base type; otherwise, <see langword="false"/>.</returns>
    /// <remarks>
    /// This method returns <see langword="false"/> if the specified <paramref name="symbol"/> is equal to the base type itself.
    /// </remarks>
    internal static bool CheckInheritance(INamedTypeSymbol symbol, string baseFullName)
    {
        var current = symbol;
        while (current.BaseType is not null)
        {
            if (current.BaseType.GetFullName() == baseFullName) return true;
            current = current.BaseType;
        }
        return false;
    } // internal static bool CheckInheritance (INamedTypeSymbol, string)
} // internal static class RoslynUtils
