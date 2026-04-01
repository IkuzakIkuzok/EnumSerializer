
// (c) 2026 Kazuki Kohzuki

#pragma warning disable IDE0130
namespace System.Diagnostics.CodeAnalysis;
#pragma warning restore IDE0130

/// <summary>
/// Specifies that when a method returns <see cref="ReturnValue"/>, the parameter may be null even if the corresponding type disallows it.
/// </summary>
/// <remarks>Initializes the attribute with the specified return value condition.</remarks>
/// <param name="returnValue">
/// The return value condition. If the method returns this value, the associated parameter may be null.
/// </param>
[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
internal sealed class MaybeNullWhenAttribute(bool returnValue) : Attribute
{
    /// <summary>
    /// Gets the return value condition.
    /// </summary>
    public bool ReturnValue { get; } = returnValue;
}

/// <summary>
/// Specifies that when a method returns <see cref="ReturnValue"/>, the parameter will not be null even if the corresponding type allows it.
/// </summary>
/// <remarks>Initializes the attribute with the specified return value condition.</remarks>
/// <param name="returnValue">
/// The return value condition. If the method returns this value, the associated parameter will not be null.
/// </param>
[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
internal sealed class NotNullWhenAttribute(bool returnValue) : Attribute
{
    /// <summary>
    /// Gets the return value condition.
    /// </summary>
    public bool ReturnValue { get; } = returnValue;
}

/// <summary>
/// Specifies that the output will be non-null if the named parameter is non-null.
/// </summary>
/// <remarks>Initializes the attribute with the associated parameter name.</remarks>
/// <param name="parameterName">
/// The associated parameter name.  The output will be non-null if the argument to the parameter specified is non-null.
/// </param>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, AllowMultiple = true, Inherited = false)]
internal sealed class NotNullIfNotNullAttribute(string parameterName) : Attribute
{
    /// <summary>
    /// Gets the associated parameter name.
    /// </summary>
    public string ParameterName { get; } = parameterName;
}
