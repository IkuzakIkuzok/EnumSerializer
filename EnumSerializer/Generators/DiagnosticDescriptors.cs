// (c) 2026 Kazuki Kohzuki

namespace EnumSerializer.Generators;

/// <summary>
/// Provides factory methods for creating diagnostics.
/// </summary>
internal static class DiagnosticDescriptors
{
    #region definitions

    private static readonly DiagnosticDescriptor _invalidAttributeInheritance = new(
        id: "ES0001",
        title: "Invalid parameter inheritance",
        messageFormat: "Type '{0}' does not inherit from 'EnumSerializer.SerializeValueAttribute'",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    private static readonly DiagnosticDescriptor _noMethodToGenerate = new(
        id: "ES1001",
        title: "No methods to generate",
        messageFormat: "'ExtensionMethods.None' is specified for '{0}'. No extension methods will be generated.",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true
    );

    private static readonly DiagnosticDescriptor _multipleAttribute = new(
        id: "ES1002",
        title: "Duplicate attribute",
        messageFormat: "Multiple '{0}' attributes are applied. This will be ignored.",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true
    );

    private static readonly DiagnosticDescriptor _extensionClassNameConflict = new(
        id: "ES1003",
        title: "Extension class name conflict",
        messageFormat: "Multiple extension class names are specified. '{0}' will be ignored.",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true
    );

    #endregion definitions

    internal static Diagnostic InvalidAttributeInheritance(string typeName, Location? location)
        => Diagnostic.Create(_invalidAttributeInheritance, location, typeName);

    internal static Diagnostic NoMethodToGenerate(string typeName, Location? location)
        => Diagnostic.Create(_noMethodToGenerate, location, typeName);

    internal static Diagnostic MultipleAttribute(string typeName, Location? location)
        => Diagnostic.Create(_multipleAttribute, location, typeName);

    internal static Diagnostic ExtensionClassNameConflict(string className, Location? location)
        => Diagnostic.Create(_extensionClassNameConflict, location, className);
} // internal static class DiagnosticDefinitions
