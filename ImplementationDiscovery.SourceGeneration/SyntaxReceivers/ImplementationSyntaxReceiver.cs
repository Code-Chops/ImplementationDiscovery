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
			if (syntaxNode is not TypeDeclarationSyntax typeDeclarationSyntax || typeDeclarationSyntax is InterfaceDeclarationSyntax) return false;

			// It should inherit (from a base class/struct/record or interface).
			var isProbablyImplementation = typeDeclarationSyntax.BaseList is not null && typeDeclarationSyntax.BaseList.Types.Count != 0;
			return isProbablyImplementation;
		}

		static bool IsProbablyDiscoverableBaseType(SyntaxNode syntaxNode, CancellationToken cancellationToken)
		{
			if (syntaxNode is not AttributeSyntax attribute || attribute.ArgumentList?.Arguments.Count > 3) return false;
			if (attribute.Parent?.Parent is not TypeDeclarationSyntax) return false;

			var isProbablyBaseType = attribute.Name.HasAttributeName(ImplementationDiscoverySourceGenerator.DiscoverableAttributeName, cancellationToken);
			return isProbablyBaseType;
		}
	}

	/// <summary>
	/// Gets the implementation details in the form of an enum member.
	/// </summary>
	/// <returns>The probably new enum member. Or null if not applicable for this node.</returns>
	internal static IEnumModel? GetEnumMemberFromImplementation(GeneratorSyntaxContext context, CancellationToken cancellationToken)
	{
		if (context.Node is not TypeDeclarationSyntax typeDeclarationSyntax) return null;

		if (ModelExtensions.GetDeclaredSymbol(context.SemanticModel, typeDeclarationSyntax, cancellationToken) is not INamedTypeSymbol type) return null;

		if (type.IsStatic || type.IsAbstract) return null;

		AttributeData? attribute = null;
		var implementsInterface = type.IsOrImplementsInterface(t => t.HasAttribute(ImplementationDiscoverySourceGenerator.DiscoverableAttributeName, ImplementationDiscoverySourceGenerator.DiscoverableAttributeNamespace, out attribute), out var baseType);
		if (!implementsInterface)
		{
			var implementsClass = type.IsOrInheritsClass(t => t.HasAttribute(ImplementationDiscoverySourceGenerator.DiscoverableAttributeName, ImplementationDiscoverySourceGenerator.DiscoverableAttributeNamespace, out attribute), out baseType);
			if (!implementsClass) return null;
		}

		if (attribute is null) return null;

		var member = new DiscoveredEnumMember(
			enumIdentifier: $"{(baseType.ContainingNamespace.IsGlobalNamespace ? null : $"{baseType.ContainingNamespace}.")}{NameHelpers.GetNameWithoutGenerics(baseType.Name)}{ImplementationDiscoverySourceGenerator.ImplementationsEnumName}",
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
			isConvertibleToConcreteType: type.IsOrInheritsClass(type => type.Equals(baseType, SymbolEqualityComparer.Default), out _));

		return member;
	}

	/// <summary>
	/// Checks if the node is an enum definition which has discoverable members or attributes.
	/// </summary>
	/// <returns>The enum definition. Or null if not applicable for this node.</returns>
	internal static IEnumModel? GetBaseType(GeneratorSyntaxContext context, CancellationToken cancellationToken)
	{
		if (context.Node is not AttributeSyntax { Parent.Parent: TypeDeclarationSyntax typeDeclarationSyntax }) return null;

		if (ModelExtensions.GetDeclaredSymbol(context.SemanticModel, typeDeclarationSyntax, cancellationToken) is not INamedTypeSymbol baseType) return null;

		// ReSharper disable once SimplifyLinqExpressionUseAll
		if (baseType.IsStatic || !typeDeclarationSyntax.Modifiers.Any(m =>  m.IsKind(SyntaxKind.PartialKeyword))) return null;

		var hasDiscoverableAttribute = baseType.HasAttribute(ImplementationDiscoverySourceGenerator.DiscoverableAttributeName, ImplementationDiscoverySourceGenerator.DiscoverableAttributeNamespace, out var discoverableAttribute);
		if (!hasDiscoverableAttribute) return null;

		var filePath = typeDeclarationSyntax.SyntaxTree.FilePath;

		var definition = new EnumDefinition(
			customName: discoverableAttribute?.ConstructorArguments.FirstOrDefault().Value?.ToString(),
			baseTypeDeclarationSyntax: typeDeclarationSyntax,
			baseTypeSymbol: baseType,
			filePath: filePath,
			generateImplementationIds: discoverableAttribute?.ConstructorArguments.Skip(1).FirstOrDefault().Value is true,
			usings: typeDeclarationSyntax.GetUsings().ToList());

		return definition;
	}
}