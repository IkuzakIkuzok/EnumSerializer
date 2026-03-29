
// (c) 2026 Kazuki Kohzuki

#pragma warning disable IDE0130
namespace EnumSerializer;
#pragma warning restore IDE0130

[Flags]
internal enum ExtensionMethods
{
    None = 0,
    ToString = 1 << 0,
    TryParse = 1 << 1,
    All = ToString | TryParse,
} // internal enum ExtensionMethods