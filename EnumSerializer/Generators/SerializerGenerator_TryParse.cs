
// (c) 2025-2026 Kazuki Kohzuki

using System.Collections.Generic;
using static EnumSerializer.SymbolUtils;

namespace EnumSerializer.Generators;

internal sealed partial class SerializerGenerator
{
    private static void GenerateTryParse(StringBuilder builder, INamedTypeSymbol enumType, IEnumerable<EnumSerializationInfo> targetTypes, GenerationMode mode, out bool usePooled)
    {
        usePooled = false;
        var canUseSpan = mode >= GenerationMode.OptimizedSpanWithIfElse;

        var enumShortName = enumType.Name;
        var enumName = enumType.GetFullyQualifiedName();

        var inputType = canUseSpan ? "global::System.ReadOnlySpan<char>" : "string";

        builder.AppendLine($$"""

        /// <summary>
        /// Deserializes the specified string to a <see cref="{{enumName}}"/> value using the specified serialization attribute.
        /// </summary>
        /// <typeparam name="TAttr">The serialization attribute type.</typeparam>
        /// <param name="text">The string representation of the enum value.</param>
        /// <returns>The deserialized <see cref="{{enumName}}"/> value.</returns>
        internal static bool TryParse{{enumShortName}}<TAttr>(this {{inputType}} text, out {{enumName}} value) where TAttr : global::EnumSerializer.SerializeValueAttribute
        {
""");
        foreach (var targetType in targetTypes)
        {
            if (!targetType.GenerateTryParse) continue;

            var target = targetType.EnumType;
            if (!CheckInheritance(target, "EnumSerializer.SerializeValueAttribute"))
                return;

            var targetFullName = target.GetFullyQualifiedName();
            builder.AppendLine($$"""
            if (typeof(TAttr) == typeof({{targetFullName}}))
                return {{GetTryParseMethodName(target, enumShortName)}}(text, out value);

""");
        }

        builder.AppendLine($$"""
            // No matching attribute type found
            value = default({{enumName}});
            return false;
        }
""");

        foreach (var target in targetTypes)
        {
            if (!target.GenerateTryParse) continue;
            GenerateTryParseFromString(builder, enumName, enumType, target, mode, out var buffer);
            usePooled |= buffer;
        }

        if (mode >= GenerationMode.ExtensionMember)
            GenerateStaticExtension(builder, enumName, enumType, targetTypes);
    } // private static void GenerateTryParse (StringBuilder, INamedTypeSymbol, IEnumerable<EnumSerializationInfo>, GenerationMode, out bool)

    #region extension method

    private static void GenerateTryParseFromString(StringBuilder builder, string enumName, INamedTypeSymbol enumType, EnumSerializationInfo info, GenerationMode mode, out bool usePooled)
    {
        usePooled = false;
        var canUseSpan = mode >= GenerationMode.OptimizedSpanWithPatternMatching;

        var target = info.EnumType;
        if (!CheckInheritance(target, "EnumSerializer.SerializeValueAttribute"))
            return;

        var targetFullName = target.GetFullyQualifiedName();
        var methodName = GetTryParseMethodName(target, enumType.Name);

        var cs = info.CaseSensitive;

        var fields = enumType.GetMembers().OfType<IFieldSymbol>().Where(f => f.IsStatic);
        var cases = new Dictionary<string, string>();
        foreach (var field in fields)
        {
            var attr = field.GetAttributes().FirstOrDefault(a => a.AttributeClass?.GetFullyQualifiedName() == targetFullName);
            if (attr is null) continue;
            var args = attr.ConstructorArguments;
            if (args.Length == 0) continue;
            var serializedValue = args[0].Value?.ToString() ?? string.Empty;
            if (cases.ContainsKey(serializedValue))
                continue;
            cases.Add(serializedValue, field.Name);
        }

        var comment = cs ? "Comparison is case-sensitive." : "Comparison is case-insensitive.";

        if (cases.Count == 0)
        {
            GenerateEmptyTryParse(builder, enumName, targetFullName, methodName, canUseSpan);
            return;
        }

        if (cases.Count == 1)
        {
            var onlyCase = cases.First();
            GenerateOnlyOneTryParse(builder, onlyCase, enumName, targetFullName, methodName, cs, canUseSpan);
            return;
        }

        if (!canUseSpan)
        {
            GenerateLengthBasesSwitchTryParse(builder, cases, enumName, targetFullName, methodName, mode, cs);
            return;
        }

        var lengths = cases.Keys.Select(k => k.Length).Distinct().ToArray();
        var len_op = lengths.Length == 1 ? "!=" : ">";
        var length = lengths.Max();
        usePooled = !cs && length > MaxStackAllocLength;

        builder.AppendLine($$"""

        /// <summary>
        /// Attempts to deserialize the specified string to a <see cref="{{enumName}}"/> value using the <see cref="{{targetFullName}}"/> attribute.
        /// {{comment}}
        /// </summary>
        /// <param name="text">The string representation of the enum value.</param>
        /// <param name="value">When this method returns, contains the deserialized <see cref="{{enumName}}"/> value if the parsing succeeded, or the default value if the parsing failed.</param>
        /// <returns><see langword="true"/> if the parsing succeeded; otherwise, <see langword="false"/>.</returns>
        internal static bool {{methodName}}(this global::System.ReadOnlySpan<char> text, out {{enumName}} value)
        {
            if (text.Length {{len_op}} {{length}})
            {
                value = default({{enumName}});
                return false;
            }

""");
        if (cs)
        {
            // Case-sensitive, so we can directly switch on the input text without any transformation
            builder.AppendLine($$"""
            switch (text)
            {
""");
        }
        else if (length > MaxStackAllocLength)
        {
            // Case-insensitive and the length of the longest case exceeds the threshold for stack allocation
            builder.AppendLine($$"""
            // `text.Length` may exceed {{MaxStackAllocLength}} here, so we use pooled buffer to avoid stack overflow in case of very long input
            using global::EnumSerializer.PooledBuffer pooled = new global::EnumSerializer.PooledBuffer(text.Length);
            global::System.Span<char> lowerText = text.Length <= {{MaxStackAllocLength}} ? stackalloc char[text.Length] : pooled.GetSpan();
            global::System.MemoryExtensions.ToLowerInvariant(text, lowerText);

            switch (lowerText)
            {
""");
        }
        else
        {
            // Case-insensitive and the length of the longest case does not exceed the threshold for stack allocation
            builder.AppendLine($$"""
            // `text.Length` never exceeds {{length}} here, so we can safely use stack allocation for the lowercase buffer
            global::System.Span<char> lowerText = (stackalloc char[text.Length]);
            global::System.MemoryExtensions.ToLowerInvariant(text, lowerText);

            switch (lowerText)
            {
""");
        }

        foreach ((var key, var value) in cases)
        {
            var k = cs ? key : key.ToLowerInvariant();
            builder.AppendLine($"                case \"{k}\":"); ;
            builder.AppendLine($"                    value = {enumName}.{value};");
            builder.AppendLine($"                    return true;");
        }

        builder.AppendLine($$"""
            }

            value = default({{enumName}});
            return false;
        }
""");
    } // private static void GenerateTryParseFromString (StringBuilder, string, INamedTypeSymbol, EnumSerializationInfo, GenerationMode, out bool)

    private static void GenerateEmptyTryParse(StringBuilder builder, string enumName, string targetFullName, string methodName, bool canUseSpan)
    {
        var inputType = canUseSpan ? "global::System.ReadOnlySpan<char>" : "string";

        builder.AppendLine($$"""

        /// <summary>
        /// Attempts to deserialize the specified string to a <see cref="{{enumName}}"/> value using the <see cref="{{targetFullName}}"/> attribute.
        /// </summary>
        /// <param name="text">The string representation of the enum value.</param>
        /// <param name="value">The default value of {{enumName}}.</param>
        /// <returns><see langword="false"/>.</returns>
        /// <remarks>Since there are no enum members decorated with the <see cref="{{targetFullName}}"/> attribute,
        /// this method always returns <see langword="false"/>.</remarks>
        internal static bool {{methodName}}(this {{inputType}} text, out {{enumName}} value)
        {
            value = default({{enumName}});
            return false;
        }
""");
    } // private static void GenerateEmptyTryParse (StringBuilder, string, string, string)

    private static void GenerateOnlyOneTryParse(StringBuilder builder, KeyValuePair<string, string> onlyCase, string enumName, string targetFullName, string methodName, bool cs, bool canUseSpan)
    {
        var inputType = canUseSpan ? "global::System.ReadOnlySpan<char>" : "string";
        var comparison = canUseSpan ? "global::System.MemoryExtensions.Equals" : "global::System.String.Equals";
        var option = cs ? "Ordinal" : "OrdinalIgnoreCase";
        var comment = cs ? "Comparison is case-sensitive." : "Comparison is case-insensitive.";

        builder.AppendLine($$"""

        /// <summary>
        /// Attempts to deserialize the specified string to a <see cref="{{enumName}}"/> value using the <see cref="{{targetFullName}}"/> attribute.
        /// {{comment}}
        /// </summary>
        /// <param name="text">The string representation of the enum value.</param>
        /// <param name="value">When this method returns, contains the deserialized <see cref="{{enumName}}"/> value if the parsing succeeded, or the default value if the parsing failed.</param>
        /// <returns><see langword="true"/> if the parsing succeeded; otherwise, <see langword="false"/>.</returns>
        internal static bool {{methodName}}(this {{inputType}} text, out {{enumName}} value)
        {
            if ({{comparison}}(text, "{{onlyCase.Key}}", global::System.StringComparison.{{option}}))
            {
                value = {{enumName}}.{{onlyCase.Value}};
                return true;
            }

            value = default({{enumName}});
            return false;
        }
""");
    } // private static void GenerateOnlyOneTryParse (StringBuilder, KeyValuePair<string, string>, string, string, string, bool)

    private static void GenerateLengthBasesSwitchTryParse(StringBuilder builder, Dictionary<string, string> cases, string enumName, string targetFullName, string methodName, GenerationMode mode, bool cs)
    {
        var canUseSpan = mode >= GenerationMode.OptimizedSpanWithIfElse;
        var inputType = canUseSpan ? "global::System.ReadOnlySpan<char>" : "string";
        var comparison = canUseSpan ? "global::System.MemoryExtensions.Equals" : "global::System.String.Equals";
        var option = cs ? "Ordinal" : "OrdinalIgnoreCase";
        var comment = cs ? "Comparison is case-sensitive." : "Comparison is case-insensitive.";

        var byLen =
            cases.GroupBy(c => c.Key.Length)
                 .ToDictionary(g => g.Key, g => g.ToList());

        builder.AppendLine($$"""

        /// <summary>
        /// Attempts to deserialize the specified string to a <see cref="{{enumName}}"/> value using the <see cref="{{targetFullName}}"/> attribute.
        /// {{comment}}
        /// </summary>
        /// <param name="text">The string representation of the enum value.</param>
        /// <param name="value">When this method returns, contains the deserialized <see cref="{{enumName}}"/> value if the parsing succeeded, or the default value if the parsing failed.</param>
        /// <returns><see langword="true"/> if the parsing succeeded; otherwise, <see langword="false"/>.</returns>
        internal static bool {{methodName}}(this {{inputType}} text, out {{enumName}} value)
        {
            switch (text.Length)
            {
""");
        foreach (var (len, items) in byLen)
        {
            builder.AppendLine($"                case {len}:");

            foreach (var item in items)
            {
                builder.AppendLine($$"""
                                        if ({{comparison}}(text, "{{item.Key}}", global::System.StringComparison.{{option}}))
                                        {
                                            value = {{enumName}}.{{item.Value}};
                                            return true;
                                        }
                    """);
            }

            builder.AppendLine($"                    break;");
        }

        builder.AppendLine($$"""
            }

            value = default({{enumName}});
            return false;
        }
""");
    } // private static void GenerateLengthBasesSwitchTryParse (StringBuilder, Dictionary<string, string>, string, string, string, GenerationMode, bool)

    private static string GetTryParseMethodName(INamedTypeSymbol target, string enumName)
    {
        var name = target.Name;
        if (name.EndsWith("Attribute"))
            name = name[..^"Attribute".Length];
        return $"TryParse{enumName}From{name}";
    } // private static string GetTryParseMethodName (INamedTypeSymbol, string)

    #endregion extension method

    #region static extension

    private static void GenerateStaticExtension(StringBuilder builder, string enumName, INamedTypeSymbol enumType, IEnumerable<EnumSerializationInfo> targetTypes)
    {
        builder.AppendLine($$"""

                    extension ({{enumName}})
                    {
                        /// <summary>
                        /// Deserializes the specified string to a <see cref="{{enumName}}"/> value using the specified serialization attribute.
                        /// </summary>
                        /// <typeparam name="TAttr">The serialization attribute type.</typeparam>
                        /// <param name="text">The string representation of the enum value.</param>
                        /// <param name="value">When this method returns, contains the deserialized <see cref="{{enumName}}"/> value if the parsing succeeded, or the default value if the parsing failed.</param>
                        /// <returns>The deserialized <see cref="{{enumName}}"/> value.</returns>
                        internal static bool TryParse<TAttr>(global::System.ReadOnlySpan<char> text, out {{enumName}} value) where TAttr : global::EnumSerializer.SerializeValueAttribute
                        {
                            return TryParse{{enumType.Name}}<TAttr>(text, out value);
                        }
            """);

        foreach (var targetType in targetTypes)
        {
            if (!targetType.GenerateTryParse) continue;

            var target = targetType.EnumType;
            if (!CheckInheritance(target, "EnumSerializer.SerializeValueAttribute"))
                return;

            var methodName = GetStaticTryParseMethodName(target);
            builder.AppendLine($$"""

                            /// <summary>
                            /// Deserializes the specified string to a <see cref="{{enumName}}"/> value using the <see cref="{{target.GetFullyQualifiedName()}}"/> attribute.
                            /// </summary>
                            /// <param name="text">The string representation of the enum value.</param>
                            /// <param name="value">When this method returns, contains the deserialized <see cref="{{enumName}}"/> value if the parsing succeeded, or the default value if the parsing failed.</param>
                            /// <returns><see langword="true"/> if the parsing succeeded; otherwise, <see langword="false"/>.</returns>
                            internal static bool {{methodName}}(global::System.ReadOnlySpan<char> text, out {{enumName}} value)
                            {
                                return {{GetTryParseMethodName(target, enumType.Name)}}(text, out value);
                            }
                """);
        }

        builder.AppendLine($$"""
                    }
            """);
    } // private static void GenerateStaticExtension (StringBuilder, string, INamedTypeSymbol, IEnumerable<EnumSerializationInfo>)

    private static string GetStaticTryParseMethodName(INamedTypeSymbol target)
    {
        var name = target.Name;
        if (name.EndsWith("Attribute"))
            name = name[..^"Attribute".Length];
        return $"TryParse{name}";
    } // private static string GetTryParseMethodName (INamedTypeSymbol)

    #endregion static extension
} // internal sealed partial class SerializerGenerator
