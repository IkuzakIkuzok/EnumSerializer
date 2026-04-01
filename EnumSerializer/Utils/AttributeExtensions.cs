
// (c) 2026 Kazuki Kohzuki

namespace EnumSerializer.Utils;

internal static class AttributeExtensions
{
    extension (AttributeData attribute)
    {
        /// <summary>
        /// Attempts to retrieve the value of a named argument from the attribute and cast it to the specified type.
        /// </summary>
        /// <typeparam name="T">The expected type of the named argument value.</typeparam>
        /// <param name="argumentName">The name of the argument to retrieve from the attribute.</param>
        /// <param name="value">When this method returns, contains the value of the named argument cast to type <typeparamref name="T"/> if found and of the correct type;
        /// otherwise, the default value for type <typeparamref name="T"/>.</param>
        /// <returns><see langword="true"/> if the named argument exists and can be cast to type <typeparamref name="T"/>; otherwise, <see langword="false"/>.</returns>
        internal bool TryGetNamedArgumentValue<T>(string argumentName, out T? value)
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
        } // internal bool TryGetNamedArgumentValue<T> (string, out T?)

        /// <summary>
        /// Attempts to retrieve the value of a named argument as an enumeration value of the specified type.
        /// </summary>
        /// <typeparam name="TEnum">The enumeration type to which the argument value should be converted.</typeparam>
        /// <param name="argumentName">The name of the argument to retrieve from the attribute's named arguments.</param>
        /// <param name="value">When this method returns, contains the enumeration value associated with the specified argument name, if found and valid;
        /// otherwise, the default value for the enumeration type.</param>
        /// <returns><see langword="true"/> if the named argument exists and its value corresponds to a valid member of the specified enumeration type;
        /// otherwise, <see langword="false"/>.</returns>
        internal bool TryGetNamedArgumentEnumValue<TEnum>(string argumentName, out TEnum value) where TEnum : struct, Enum
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
        } // internal bool TryGetNamedArgumentEnumValue<TEnum> (string, out TEnum) where TEnum : struct, Enum
    }

    private static class EnumCache<T> where T : struct, Enum
    {
        public static readonly System.Collections.Generic.HashSet<int> ValidValues = [.. Enumerable.Cast<int>(Enum.GetValues(typeof(T)))];
    } // private static class EnumCache<T> where T : struct, Enum
} // internal static class AttributeExtensions
