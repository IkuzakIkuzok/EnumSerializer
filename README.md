
# EnumSerializer

[![Test](https://github.com/IkuzakIkuzok/EnumSerializer/actions/workflows/Test.yml/badge.svg)](https://github.com/IkuzakIkuzok/EnumSerializer/actions/workflows/Test.yml)
[![Version](https://img.shields.io/nuget/v/EnumSerializer?styles=flat)](https://www.nuget.org/packages/EnumSerializer/#versions-body-tab)
[![Download](https://img.shields.io/nuget/dt/EnumSerializer?styles=flat)](https://www.nuget.org/packages/EnumSerializer/#versions-body-tab)
[![MIT License](http://img.shields.io/badge/license-MIT-blue.svg?style=flat)](https://github.com/IkuzakIkuzok/EnumSerializer/blob/main/LICENSE)

Zero-allocation serialization and deserialization of enums to and from strings enums in C#.

## Installation

You can install the EnumSerializer from [NuGet](https://www.nuget.org/packages/EnumSerializer/).

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

## Benchmarks

### Parse performance

Parse 10 enum values from their string representations using both the standard `Enum<T>.TryParse()` method and the generated try-parse method from this generator.

| Method                       | Mean      | Error    | StdDev   | Ratio | RatioSD |
|----------------------------- |----------:|---------:|---------:|------:|--------:|
| ParseStandard                | 244.33 ns | 2.347 ns | 2.195 ns |  1.00 |    0.00 |
| **ParseGenerated**           |  31.19 ns | 0.446 ns | 0.395 ns |  0.13 |    0.00 |
| ParseStandardIgnoreCase      | 337.83 ns | 3.093 ns | 2.894 ns |  1.38 |    0.02 |
| **ParseGeneratedIgnoreCase** | 121.84 ns | 0.544 ns | 0.425 ns |  0.50 |    0.00 |

## Warning for security-sensitive data

Do NOT use this generator in security-sensitive contexts,
because the generated code is optimized for performance and may be vulnerable to certain types of attacks (e.g., timing attacks).
For such scenarios, consider using a more secure serialization approach that prioritizes security over performance.
