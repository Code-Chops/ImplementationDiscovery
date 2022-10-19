using System.Diagnostics;
using CodeChops.ImplementationDiscovery.SourceGeneration.Models;

namespace CodeChops.ImplementationDiscovery.SourceGeneration.SyntaxReceivers;

internal static class ImplementationSyntaxReceiver
{
	/// <summary>
	/// The predicate for every node that is probably a discoverable implementation.
	/// </summary>
	internal static bool CheckIfIsProbablyImplementation(SyntaxNode syntaxNode, CancellationToken cancellationToken)
	{
		// It should be a class, record or struct declaration.
		if (syntaxNode is not TypeDeclarationSyntax typeDeclarationSyntax || typeDeclarationSyntax is InterfaceDeclarationSyntax) return false;

		// It should inherit (from a base class/struct/record or interface).
		var isProbablyImplementation = typeDeclarationSyntax.BaseList is not null && typeDeclarationSyntax.BaseList.Types.Count != 0;
		return isProbablyImplementation;
	}

	/// <summary>
	/// Gets the implementation details in the form of an enum member.
	/// </summary>
	/// <returns>The probably new enum member. Or null if not applicable for this node.</returns>
	internal static IEnumModel[] GetImplementation(GeneratorSyntaxContext context, CancellationToken cancellationToken)
	{
		if (context.Node is not TypeDeclarationSyntax typeDeclarationSyntax) 
			return Array.Empty<IEnumModel>();

		if (ModelExtensions.GetDeclaredSymbol(context.SemanticModel, typeDeclarationSyntax, cancellationToken) is not INamedTypeSymbol type) 
			return Array.Empty<IEnumModel>();
		
		if (type.IsStatic || type.IsAbstract) 
			return Array.Empty<IEnumModel>();
		
		AttributeData? attribute = null;
		var implementsInterface = type.IsOrImplementsInterface(t => t.HasAttribute(ImplementationDiscoverySourceGenerator.DiscoverableAttributeName, ImplementationDiscoverySourceGenerator.DiscoverableAttributeNamespace, out attribute), out var baseType);
		if (!implementsInterface)
		{
			var implementsClass = type.IsOrInheritsClass(t => t.HasAttribute(ImplementationDiscoverySourceGenerator.DiscoverableAttributeName, ImplementationDiscoverySourceGenerator.DiscoverableAttributeNamespace, out attribute), out baseType);
			if (!implementsClass) 
				return Array.Empty<IEnumModel>();
		}
		
		if (attribute is null) 
			return Array.Empty<IEnumModel>();
	
		if (!baseType.ContainingAssembly.Equals(type.ContainingAssembly, SymbolEqualityComparer.Default))
		{
			if (!type.BaseType!.ContainingAssembly.Equals(type.ContainingAssembly, SymbolEqualityComparer.Default))
				baseType = type;
			else
			{
				if (implementsInterface) type.IsOrImplementsInterface(t => !t.BaseType!.ContainingAssembly.Equals(type.ContainingAssembly, SymbolEqualityComparer.Default), out baseType);
				if (!implementsInterface) type.IsOrInheritsClass(t => !t.BaseType!.ContainingAssembly.Equals(type.ContainingAssembly, SymbolEqualityComparer.Default), out baseType);
			}
		}

		var member = new DiscoveredEnumMember(
			enumIdentifier: $"{(baseType.ContainingNamespace.IsGlobalNamespace ? null : $"{baseType.ContainingNamespace}.")}{NameHelpers.GetNameWithoutGenerics(baseType.Name)}",
			name: type.Name,
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
			accessibility: type.DeclaredAccessibility.ToString().ToLowerInvariant());

		var definition = GetEnumDefinition(baseType, attribute);
		
		return new IEnumModel[] { member, definition!};
	}

	private static EnumDefinition? GetEnumDefinition(ITypeSymbol baseType, AttributeData? discoverableAttribute = null)
	{
		if (baseType.DeclaringSyntaxReferences.First().GetSyntax() is not TypeDeclarationSyntax typeDeclarationSyntax)
			return null;

		// ReSharper disable once SimplifyLinqExpressionUseAll
		if (baseType.IsStatic || !typeDeclarationSyntax.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword)))
			return null;

		if (discoverableAttribute is null)
		{
			var hasDiscoverableAttribute = baseType.HasAttribute(ImplementationDiscoverySourceGenerator.DiscoverableAttributeName, ImplementationDiscoverySourceGenerator.DiscoverableAttributeNamespace, out discoverableAttribute);
			if (!hasDiscoverableAttribute) 
				return null;
		}

		var filePath = typeDeclarationSyntax.SyntaxTree.FilePath;

		var definition = new EnumDefinition(
			customName: discoverableAttribute!.GetArgumentOrDefault("enumName", defaultValue: (string?)null),
			baseTypeDeclarationSyntax: typeDeclarationSyntax,
			baseTypeSymbol: baseType,
			filePath: filePath,
			generateImplementationIds: discoverableAttribute!.GetArgumentOrDefault("generateImplementationIds", defaultValue: false),
			hasSingletonImplementations: discoverableAttribute!.GetArgumentOrDefault("hasSingletonImplementations", defaultValue: false),
			usings: typeDeclarationSyntax.GetUsings().ToList());
		
		return definition;
	}
}