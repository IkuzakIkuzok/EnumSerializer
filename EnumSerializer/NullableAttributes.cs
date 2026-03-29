
// (c) 2026 Kazuki Kohzuki

namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Specifies that when a method returns <see cref="ReturnValue"/>, the parameter may be null even if the corresponding type disallows it.
/// </summary>
[global::System.AttributeUsage(global::System.AttributeTargets.Parameter, Inherited = false)]
internal sealed class MaybeNullWhenAttribute : global::System.Attribute
{
    /// <summary>
    /// Gets the return value condition.
    /// </summary>
    public bool ReturnValue { get; }

    /// <summary>Initializes the attribute with the specified return value condition.</summary>
    /// <param name="returnValue">
    /// The return value condition. If the method returns this value, the associated parameter may be null.
    /// </param>
    public MaybeNullWhenAttribute(bool returnValue)
    {
        this.ReturnValue = returnValue;
    }
}

/// <summary>
/// Specifies that when a method returns <see cref="ReturnValue"/>, the parameter will not be null even if the corresponding type allows it.
/// </summary>
[global::System.AttributeUsage(global::System.AttributeTargets.Parameter, Inherited = false)]
internal sealed class NotNullWhenAttribute : global::System.Attribute
{
    /// <summary>
    /// Gets the return value condition.
    /// </summary>
    public bool ReturnValue { get; }

    /// <summary>Initializes the attribute with the specified return value condition.</summary>
    /// <param name="returnValue">
    /// The return value condition. If the method returns this value, the associated parameter will not be null.
    /// </param>
    public NotNullWhenAttribute(bool returnValue)
    {
        this.ReturnValue = returnValue;
    }
}

/// <summary>
/// Specifies that the output will be non-null if the named parameter is non-null.
/// </summary>
[global::System.AttributeUsage(AttributeTargets.Parameter | global::System.AttributeTargets.Property | global::System.AttributeTargets.ReturnValue, AllowMultiple = true, Inherited = false)]
internal sealed class NotNullIfNotNullAttribute : global::System.Attribute
{
    /// <summary>
    /// Gets the associated parameter name.
    /// </summary>
    public string ParameterName { get; }

    /// <summary>Initializes the attribute with the associated parameter name.</summary>
    /// <param name="parameterName">
    /// The associated parameter name.  The output will be non-null if the argument to the parameter specified is non-null.
    /// </param>
    public NotNullIfNotNullAttribute(string parameterName)
    {
        this.ParameterName = parameterName;
    }
}
