
// (c) 2026 Kazuki Kohzuki

namespace EnumSerializer.Test.Diagnostics;

[DiagnosticTest]
public sealed partial class DiagnosticTests
{
    // lang=C#
    [TestSource]
    private static readonly string _noDiagnostics = """
        using EnumSerializer;
        
        namespace Test;
        
        [EnumSerializable(typeof(DefaultSerializeValueAttribute))]
        internal enum MyEnum
        {
            [DefaultSerializeValueAttribute("val1")]
            Value1,
            
            [DefaultSerializeValueAttribute("val2")]
            Value2,
            
            [DefaultSerializeValueAttribute("val3")]
            Value3
        }
        """;

    // lang=C#
    [TestSource]
    private static readonly string _invalidAttributeInheritance = """
        using EnumSerializer;
        
        namespace Test;
        
        [EnumSerializable({|ES0001:typeof(string)|})] // string does not inherit from SerializeValueAttribute
        internal enum MyEnum
        {
            [DefaultSerializeValueAttribute("val1")]
            Value1,
            
            [DefaultSerializeValueAttribute("val2")]
            Value2,
            
            [DefaultSerializeValueAttribute("val3")]
            Value3
        }
        """;

    // lang=C#
    [TestSource]
    private static readonly string _multipleAttribute = """
        using EnumSerializer;
        
        namespace Test;
        
        [EnumSerializable(typeof(DefaultSerializeValueAttribute))]
        [EnumSerializable({|ES1002:typeof(DefaultSerializeValueAttribute)|})] // Multiple EnumSerializable attributes with DefaultSerializeValueAttribute
        internal enum MyEnum
        {
            [DefaultSerializeValueAttribute("val1")]
            Value1,
            
            [DefaultSerializeValueAttribute("val2")]
            Value2,
            
            [DefaultSerializeValueAttribute("val3")]
            Value3
        }
        """;

    // lang=C#
    [TestSource]
    private static readonly string _extensionClassNameConflict = """
        using EnumSerializer;
        
        namespace Test;
        
        [EnumSerializable(typeof(DefaultSerializeValueAttribute), ExtensionClassName = "CustomClassName")]
        [EnumSerializable(typeof(CustomSerializeValueAttribute), {|ES1003:ExtensionClassName = "DifferentCustomClassName"|})] // This ExtensionClassName is ignored because it's only applicable to the first EnumSerializableAttribute.
        internal enum MyEnum
        {
            [DefaultSerializeValueAttribute("val1")]
            Value1,
        
            [DefaultSerializeValueAttribute("val2")]
            Value2,
        
            [DefaultSerializeValueAttribute("val3")]
            Value3
        }
        
        internal class CustomSerializeValueAttribute : SerializeValueAttribute
        {
            internal CustomSerializeValueAttribute(string value) : base(value) { }
        }
        """;
} // public sealed partial class DiagnosticTests
