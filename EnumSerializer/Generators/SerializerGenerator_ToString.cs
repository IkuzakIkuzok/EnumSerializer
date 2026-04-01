
// (c) 2025-2026 Kazuki Kohzuki

using EnumSerializer.Utils;
using System.Collections.Generic;

namespace EnumSerializer.Generators;

internal sealed partial class SerializerGenerator
{
    private static void GenerateToString(StringBuilder builder, INamedTypeSymbol enumType, IEnumerable<SerializeValueInfo> targetTypes, GenerationMode mode)
    {
        var enumName = enumType.FullyQualifiedName;

        builder.AppendLine($$"""

        /// <summary>
        /// Serializes the specified <see cref="{{enumName}}"/> value to a string using the specified serialization attribute.
        /// </summary>
        /// <typeparam name="TAttr">The serialization attribute type.</typeparam>
        /// <param name="value">The <see cref="{{enumName}}"/> value to serialize.</param>
        /// <returns>The serialized string representation of the enum value.</returns>
        internal static string ToString<TAttr>(this {{enumName}} value) where TAttr : global::EnumSerializer.SerializeValueAttribute
        {
""");

        foreach (var targetType in targetTypes)
        {
            if (!targetType.GenerateToString) continue;

            var target = targetType.AttributeType;
            var targetFullName = target.FullyQualifiedName;
            builder.AppendLine($$"""
            if (typeof(TAttr) == typeof({{targetFullName}}))
                return {{GetSpecialToStringMethodName(target)}}(value);

""");
        }

        builder.AppendLine($$"""
            // Fallback to default ToString() if no matching attribute type is found
            return value.ToString();
        } // internal static string ToString<TAttr>(this {{enumName}}) where TAttr : global::EnumSerializer.SerializeValueAttribute
""");

        var canUsePatternMatching = mode >= GenerationMode.OptimizedSpanWithIfElse;
        foreach (var target in targetTypes)
        {
            if (!target.GenerateToString) continue;
            GenerateSpecialToString(builder, enumName, enumType, target.AttributeType, canUsePatternMatching);
        }
    } // private static void GenerateToString (StringBuilder, INamedTypeSymbol, IEnumerable<EnumSerializationInfo>, bool)

    private static void GenerateSpecialToString(StringBuilder builder, string enumName, INamedTypeSymbol enumType, INamedTypeSymbol target, bool canUsePatternMatching)
    {
        var targetFullName = target.FullyQualifiedName;
        var methodName = GetSpecialToStringMethodName(target);

        builder.AppendLine($$"""

        /// <summary>
        /// Serializes the specified <see cref="{{enumName}}"/> value to a string using the <see cref="{{targetFullName}}"/> attribute.
        /// </summary>
        /// <param name="value">The <see cref="{{enumName}}"/> value to serialize.</param>
        /// <returns>The serialized string representation of the enum value.</returns>
        internal static string {{methodName}}(this {{enumName}} value)
        {
""");
        if (canUsePatternMatching)
        {
            builder.AppendLine($$"""
            return value switch
            {
""");
        }
        else
        {
            builder.AppendLine($$"""
            switch (value)
            {
""");
        }

        var nameValuePairs = GetNameValuePairs(enumType, targetFullName, out var length);

        foreach ((var name, var value) in nameValuePairs)
        {
            var s_value = string.IsNullOrEmpty(value) ? "string.Empty" : $"\"{value}\"";
            if (canUsePatternMatching)
            {
                builder.Append($"                {enumName}.{name}");
                builder.Append(' ', length - name.Length);
                builder.AppendLine($" => {s_value},");
            }
            else
            {
                builder.AppendLine($"                case {enumName}.{name}:");
                builder.AppendLine($"                    return {s_value};");
            }


        }

        if (canUsePatternMatching)
        {
            builder.AppendLine("""
                _ => value.ToString(),
            };
        }
""");
        }
        else
        {
            builder.AppendLine("""
                default:
                    return value.ToString();
            }
        }
""");
        }

    } // private static void GenerateSpecialToString (StringBuilder, string, INamedTypeSymbol, INamedTypeSymbol)
    
    private static Dictionary<string, string> GetNameValuePairs(INamedTypeSymbol enumType, string targetFullName, out int length)
    {
        var cases = new Dictionary<string, string>();
        length = 0;

        var fields = enumType.GetMembers().OfType<IFieldSymbol>().Where(f => f.IsStatic);
        foreach (var field in fields)
        {
            var attr = field.GetAttributes().FirstOrDefault(a => a.AttributeClass?.FullyQualifiedName == targetFullName);
            if (attr is null) continue;

            var args = attr.ConstructorArguments;
            if (args.Length == 0) continue;

            var key = field.Name;
            if (args[0].Value is not string serializedValue) serializedValue = string.Empty;
            cases.Add(key, serializedValue);

            if (key.Length > length)
                length = key.Length;
        }

        return cases;
    } // private static Dictionary<string, string> GetNameValuePairs (INamedTypeSymbol, string, out int)

    private static string GetSpecialToStringMethodName(INamedTypeSymbol target)
    {
        const string Suffix = "Attribute";

        var name = target.Name;
        if (name.EndsWith(Suffix))
            name = name[..^Suffix.Length];
        return $"To{name}";
    } // private static string GetSpecialToStringMethodName (INamedTypeSymbol)
} // internal sealed partial class SerializerGenerator
