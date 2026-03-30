
// (c) 2026 Kazuki Kohzuki

namespace EnumSerializer.Generators;

internal enum GenerationMode
{
    /// <summary>
    /// &lt; C# 8.0
    /// </summary>
    StringFallback,

    /// <summary>
    /// &gt;= C# 8.0 &amp;&amp; &lt; C# 11
    /// <list type="bullet">
    /// <item>ReadOnlySpan is available, but switch statements on spans are not supported.</item>
    /// <item>Pattern matching is supported for enum values, but not for spans.</item>
    /// <item><c>using</c> declarations for <c>ref</c> structs are available.</item>
    /// </list>
    /// </summary>
    OptimizedSpanWithIfElse,

    /// <summary>
    /// &gt;= C# 11
    /// <list type="bullet">
    /// <item>Switch statements on ReadOnlySpan are supported.</item>
    /// <item><c>file</c>-scoped types are available.</item>
    /// </list>
    /// </summary>
    OptimizedSpanWithPatternMatching,
}
