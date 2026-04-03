
#pragma warning disable IDE0130, IDE0161

namespace EnumSerializer
{
    /// <summary>
    /// Specifies the extension methods to be generated for enum serialization.
    /// </summary>
    [global::System.Flags]
    internal enum ExtensionMethods
    {
        /// <summary>
        /// No extension methods will be generated.
        /// </summary>
        None = 0,

        /// <summary>
        /// The <c>ToString</c> extension method will be generated, allowing conversion of enum members to their serialized string representations.
        /// </summary>
        ToString = 1 << 0,

        /// <summary>
        /// The <c>TryParse</c> extension method will be generated, allowing parsing of serialized string representations back to enum members.
        /// </summary>
        TryParse = 1 << 1,

        /// <summary>
        /// Both <c>ToString</c> and <c>TryParse</c> extension methods will be generated.
        /// </summary>
        All = ToString | TryParse,
    }
}

#pragma warning restore
