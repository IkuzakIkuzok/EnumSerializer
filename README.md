
# EnumSerializer

A source generator for serializing enums in C#.

## Installation

You can install the EnumSerializer from [NuGet](https://www.nuget.org/packages/EnumSerializer/):

## Usage

Extension methods for enum serialization are generated in the same namespace as the enum.
Simply mark your enum with the `[EnumSerializable]` attribute with the appropriate attributes to generate the serialization code.

```C#
using EnumSerializer;

namespace Test;

[EnumSerializable(typeof(DefaultSerializeValueAttribute))]
internal enum MyEnum
{
    [DefaultSerializeValue("val1")]
    Value1,

    [DefaultSerializeValue("val2")]
    Value2,

    [DefaultSerializeValue("val3")]
    Value3
}
```

This will generate the following extension methods for the `MyEnum` enum:
```C#
namespace Test
{
    internal static class MyEnumSerializationExtensions
    {
        internal static string ToString<TAttr>(this MyEnum value) where TAttr : SerializeValueAttribute
        {
            if (typeof(TAttr) == typeof(DefaultSerializeValueAttribute))
                return ToDefaultSerializeValue(value);

            // Fallback to default ToString() if no matching attribute type is found
            return value.ToString();
        }

        internal static string ToDefaultSerializeValue(this MyEnum value)
        {
            return value switch
            {
                MyEnum.Value1 => "val1",
                MyEnum.Value2 => "val2",
                MyEnum.Value3 => "val3",
                _ => value.ToString(),
            };
        }

        internal static bool TryParseMyEnum<TAttr>(this ReadOnlySpan<char> text, out MyEnum value) where TAttr : SerializeValueAttribute
        {
            if (typeof(TAttr) == typeof(DefaultSerializeValueAttribute))
                return TryParseMyEnumFromDefaultSerializeValue(text, out value);

            // No matching attribute type found
            value = default;
            return false;
        }

        internal static bool TryParseMyEnumFromDefaultSerializeValue(this ReadOnlySpan<char> text, out MyEnum value)
        {
            switch (text)
            {
                case "val1":
                    value = MyEnum.Value1;
                    return true;
                case "val2":
                    value = MyEnum.Value2;
                    return true;
                case "val3":
                    value = MyEnum.Value3;
                    return true;
            }

            value = default;
            return false;
        }
    }
}
```

Note that this example is simplified for demonstration purposes.
The actual generated code may include additional features such as optimizations.

Multiple attribute types can be usesd for the same enum to generate different serialization methods.

```C#
using EnumSerializer;

namespace Test;

[EnumSerializable(typeof(DefaultSerializeValueAttribute))]
[EnumSerializable(typeof(UIValueAttribute))]
internal enum MyEnum
{
    [DefaultSerializeValue("val1")]
    [UIValue("Value 1")]
    Value1,

    [DefaultSerializeValue("val2")]
    [UIValue("Value 2")]
    Value2,

    [DefaultSerializeValue("val3")]
    [UIValue("Value 3")]
    Value3
}
```

### Options

#### Case sensitivity for parsing

To enable case-insensitive parsing, set `CaseSensitive` property to `false` in the `EnumSerializableAttribute`.

```C#
using EnumSerializer;

namespace Test;

[EnumSerializable(typeof(DefaultSerializeValueAttribute), CaseSensitive = false)]
internal enum MyEnum
{
    [DefaultSerializeValue("val1")]
    Value1,

    [DefaultSerializeValue("val2")]
    Value2,

    [DefaultSerializeValue("val3")]
    Value3
}
```

This will generate parsing methods that ignore case when matching input strings to enum values.
The default value for `CaseSensitive` is `true`, meaning that parsing will be case-sensitive unless explicitly set to `false`.

#### Target methods

By default, the generator creates `ToString()` and `TryParse()` methods for each enum.
To customize the generated method names, you can set the `Methods` property in the `EnumSerializableAttribute`.
The `Methods` property accepts a combination of flags from the `ExtensionMethods` enum, which includes the following options:

- `ToString`: Generates a `ToString()` extension method for the enum.
- `TryParse`: Generates a `TryParse()` extension method for the enum.
- `All`: Generates both `ToString()` and `TryParse()` extension methods for the enum.

(`ExtensionMethods` enum also has a `None` value which can be used to disable generation of all methods, but this is not a common use case since it would defeat the purpose of using the generator.)

For example, following code will generate only `ToString()` methods for the `MyEnum` enum:
```C#
using EnumSerializer;

namespace Test;

[EnumSerializable(typeof(DefaultSerializeValueAttribute), Methods = ExtensionMethods.ToString)]
internal enum MyEnum
{
    [DefaultSerializeValue("val1")]
    Value1,

    [DefaultSerializeValue("val2")]
    Value2,

    [DefaultSerializeValue("val3")]
    Value3
}
```