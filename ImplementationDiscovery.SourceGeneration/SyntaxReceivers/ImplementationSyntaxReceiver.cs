using CodeChops.ImplementationDiscovery.SourceGeneration.Models;

namespace CodeChops.ImplementationDiscovery.SourceGeneration.SyntaxReceivers;

internal static class ImplementationSyntaxReceiver
{
    /// <summary>
    /// The predicate for every node that is probably a discoverable implementation.
    /// </summary>
    internal static bool CheckIfIsProbablyDiscoverableBaseOrImplementation(SyntaxNode syntaxNode, CancellationToken cancellationToken)
    {
        var isProbablyBaseType = IsProbablyDiscoverableBaseType(syntaxNode, cancellationToken);
        var isProbablyImplementation = IsProbablyDiscoverableImplementation(syntaxNode);

        return isProbablyBaseType || isProbablyImplementation;


        static bool IsProbablyDiscoverableImplementation(SyntaxNode syntaxNode)
        {
            // It should be a class, record or struct declaration.
            if (syntaxNode is not TypeDeclarationSyntax typeDeclarationSyntax || typeDeclarationSyntax is InterfaceDeclarationSyntax)
                return false;

            // It should inherit (from a base class/struct/record or interface).
            var isProbablyImplementation = typeDeclarationSyntax.BaseList is not null && typeDeclarationSyntax.BaseList.Types.Count is not 0;
            return isProbablyImplementation;
        }

        static bool IsProbablyDiscoverableBaseType(SyntaxNode syntaxNode, CancellationToken cancellationToken)
        {
            if (syntaxNode is not AttributeSyntax attribute || attribute.ArgumentList?.Arguments.Count > 3)
                return false;

            if (attribute.Parent?.Parent is not TypeDeclarationSyntax)
                return false;

            var isProbablyBaseType = attribute.Name.HasAttributeName(Constants.DiscoverableAttributeName, cancellationToken);
            return isProbablyBaseType;
        }
    }

    /// <summary>
    /// Gets the implementation details in the form of an enum member.
    /// </summary>
    /// <returns>The probably new enum member. Or null if not applicable for this node.</returns>
    internal static IEnumerable<IEnumModel> GetImplementation(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        if (context.Node is not TypeDeclarationSyntax typeDeclarationSyntax)
            return Array.Empty<IEnumModel>();

        if (ModelExtensions.GetDeclaredSymbol(context.SemanticModel, typeDeclarationSyntax, cancellationToken) is not INamedTypeSymbol type)
            return Array.Empty<IEnumModel>();

        if (type.IsStatic || type.IsAbstract)
            return Array.Empty<IEnumModel>();

        if (!TryGetBaseType(type, out var baseType, out var attribute, out var externalBaseType))
            return Array.Empty<IEnumModel>();

        var definition = GetEnumDefinition(typeDeclarationSyntax, baseType!, attribute, externalBaseType);

        var member = new DiscoveredEnumMember(
            enumIdentifier: definition!.EnumIdentifier,
            name: type.GetTypeNameWithGenericParameters(),
            isPartial: typeDeclarationSyntax.Modifiers.Any(m =>  m.IsKind(SyntaxKind.PartialKeyword)),
            @namespace: type.ContainingNamespace.IsGlobalNamespace
                ? null
                : type.ContainingNamespace.ToDisplayString(),
            declaration: type.GetObjectDeclaration(),
            value: type.GetFullTypeNameWithGenericParameters(),
            filePath: typeDeclarationSyntax.SyntaxTree.FilePath,
            linePosition: typeDeclarationSyntax.SyntaxTree.GetLineSpan(typeDeclarationSyntax.Span, cancellationToken).StartLinePosition,
            typeParameters: typeDeclarationSyntax.TypeParameterList?.ToFullString(),
            isConvertibleToConcreteType: type.IsOrInheritsClass(type => type.Equals(baseType, SymbolEqualityComparer.Default), out _),
            accessibility: type.DeclaredAccessibility.ToString().ToLowerInvariant(),
            instanceCreationMethod: GetInstanceCreationMethod(type),
            hasComments: !String.IsNullOrWhiteSpace(type.GetDocumentationCommentXml(cancellationToken: cancellationToken)));

        return [member, definition];
    }

    private static InstanceCreationMethod GetInstanceCreationMethod(INamedTypeSymbol type)
    {
        if (type.IsOrImplementsInterface(t => t.IsType(typeName: Constants.NewableInterface, containingNamespace: Constants.ICreatableNamespace, isGenericType: true), out _))
            return InstanceCreationMethod.New;

        if (type.IsOrImplementsInterface(
                predicate: t => t is INamedTypeSymbol { TypeArguments.Length: 1 } && t.IsType(Constants.CreatableInterface, Constants.ICreatableNamespace, isGenericType: true),
                targetType: out _))
            return InstanceCreationMethod.Factory;

        return InstanceCreationMethod.Uninitialized;
    }

    private static bool TryGetBaseType(ITypeSymbol type, out ITypeSymbol? baseType, out AttributeData? attribute, out ITypeSymbol? externalBaseType)
    {
        AttributeData? attributeData = null;
        var implementsInterface = type.IsOrImplementsInterface(t => t.HasAttribute(Constants.DiscoverableAttributeName, Constants.DiscoverableAttributeNamespace, out attributeData), out baseType);
        if (!implementsInterface)
        {
            var implementsClass = type.IsOrInheritsClass(t => t.HasAttribute(Constants.DiscoverableAttributeName, Constants.DiscoverableAttributeNamespace, out attributeData), out baseType);
            if (!implementsClass)
            {
                attribute = null;
                externalBaseType = null;
                return false;
            }
        }

        attribute = attributeData;

        // Base type is in another assembly.
        if (!baseType.ContainingAssembly.Equals(type.ContainingAssembly, SymbolEqualityComparer.Default))
        {
            externalBaseType = baseType;
            if (implementsInterface)
                type.IsOrImplementsInterface(t => !t.BaseType!.ContainingAssembly.Equals(type.ContainingAssembly, SymbolEqualityComparer.Default), out baseType);
            else
                type.IsOrInheritsClass(t => !t.BaseType!.ContainingAssembly.Equals(type.ContainingAssembly, SymbolEqualityComparer.Default), out baseType);

            return true;
        }

        externalBaseType = null;
        return true;
    }

    private static EnumDefinition? GetEnumDefinition(TypeDeclarationSyntax syntax, ITypeSymbol baseType, AttributeData? discoverableAttribute, ITypeSymbol? externalBaseType)
    {
        if (baseType.IsStatic)
            return null;

        if (discoverableAttribute is null)
        {
            var hasDiscoverableAttribute = baseType.HasAttribute(Constants.DiscoverableAttributeName, Constants.DiscoverableAttributeNamespace, out discoverableAttribute);
            if (!hasDiscoverableAttribute || discoverableAttribute is null)
                return null;
        }

        var filePath = syntax.SyntaxTree.FilePath;

        var generateProxies = discoverableAttribute.GetArgumentOrDefault("generateProxies", defaultValue: false);

        // Is a proxy enum
        if (generateProxies && externalBaseType is not null)
        {
            var proxyDefinition = GetEnumDefinition(syntax, baseType: externalBaseType.OriginalDefinition, discoverableAttribute, externalBaseType: null);

            return new EnumDefinition(
                baseTypeNameIncludingGenerics: proxyDefinition?.BaseTypeNameIncludingGenerics ?? baseType.Name + syntax.TypeParameterList?.ToFullString(),
                baseType: baseType,
                externalBaseType: externalBaseType.OriginalDefinition,
                syntax: syntax,
                filePath: filePath,
                attribute: discoverableAttribute,
                externalDefinition: proxyDefinition);
        }

        return new EnumDefinition(
            baseTypeNameIncludingGenerics: baseType.OriginalDefinition.GetTypeNameWithGenericParameters(),
            baseType: baseType,
            externalBaseType: null,
            syntax: syntax,
            filePath: filePath,
            attribute: discoverableAttribute,
            externalDefinition: null);
    }

    /// <summary>
    /// Checks if the node is an enum definition which has discoverable members or attributes.
    /// </summary>
    /// <returns>The enum definition. Or null if not applicable for this node.</returns>
    internal static IEnumModel? GetBaseType(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        if (context.Node is not AttributeSyntax { Parent.Parent: TypeDeclarationSyntax typeDeclarationSyntax })
            return null;

        if (ModelExtensions.GetDeclaredSymbol(context.SemanticModel, typeDeclarationSyntax, cancellationToken) is not INamedTypeSymbol baseType)
            return null;

        var definition = GetEnumDefinition(typeDeclarationSyntax, baseType, discoverableAttribute: null, externalBaseType: null);
        return definition;
    }
}