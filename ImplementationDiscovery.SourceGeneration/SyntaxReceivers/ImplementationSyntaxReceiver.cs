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
	internal static IEnumerable<IEnumModel> GetImplementation(GeneratorSyntaxContext context, CancellationToken cancellationToken)
	{
		//Debugger.Launch();
		
		if (context.Node is not TypeDeclarationSyntax typeDeclarationSyntax) 
			return Array.Empty<IEnumModel>();

		if (ModelExtensions.GetDeclaredSymbol(context.SemanticModel, typeDeclarationSyntax, cancellationToken) is not INamedTypeSymbol type) 
			return Array.Empty<IEnumModel>();
		
		if (type.IsStatic) 
			return Array.Empty<IEnumModel>();

		if (!TryGetBaseType(type, out var baseType, out var attribute, out var externalBaseType))
			return Array.Empty<IEnumModel>();
		
		var definition = GetEnumDefinition(baseType!, attribute, externalBaseType);

		var member = new DiscoveredEnumMember(
			enumIdentifier: $"{(baseType!.ContainingNamespace.IsGlobalNamespace ? null : $"{baseType.ContainingNamespace}.")}{NameHelpers.GetNameWithoutGenerics(baseType.Name)}",
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
		
		return new IEnumModel[] { member, definition!};
	}
	
	private static bool TryGetBaseType(ITypeSymbol type, out ITypeSymbol? baseType, out AttributeData? attribute, out ITypeSymbol? externalBaseType)
	{
		AttributeData? attributeData = null;
		var implementsInterface = type.IsOrImplementsInterface(t => t.HasAttribute(ImplementationDiscoverySourceGenerator.DiscoverableAttributeName, ImplementationDiscoverySourceGenerator.DiscoverableAttributeNamespace, out attributeData), out baseType);
		if (!implementsInterface)
		{
			var implementsClass = type.IsOrInheritsClass(t => t.HasAttribute(ImplementationDiscoverySourceGenerator.DiscoverableAttributeName, ImplementationDiscoverySourceGenerator.DiscoverableAttributeNamespace, out attributeData), out baseType);
			if (!implementsClass)
			{
				attribute = null;
				externalBaseType = null;
				return false;
			}
		}
		
		attribute = attributeData;
		
		// Base type is in the same assembly.
		if (baseType.ContainingAssembly.Equals(type.ContainingAssembly, SymbolEqualityComparer.Default))
		{
			externalBaseType = null;
			return true;
		}

		externalBaseType = baseType;
		if (implementsInterface) type.IsOrImplementsInterface(t => !t.BaseType!.ContainingAssembly.Equals(type.ContainingAssembly, SymbolEqualityComparer.Default), out baseType);
		if (!implementsInterface) type.IsOrInheritsClass(t => !t.BaseType!.ContainingAssembly.Equals(type.ContainingAssembly, SymbolEqualityComparer.Default), out baseType);

		return true;
	}
	
	private static EnumDefinition? GetEnumDefinition(ITypeSymbol baseType, AttributeData? discoverableAttribute, ITypeSymbol? externalBaseType)
	{
		if (baseType.IsStatic)
			return null;

		if (baseType.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() is not TypeDeclarationSyntax typeDeclarationSyntax)
			return new EnumDefinition(
				customName: discoverableAttribute!.GetArgumentOrDefault("enumName", defaultValue: (string?)null),
				name: NameHelpers.GetNameWithoutGenerics(baseType.Name),
				typeParameters: null,
				enumNamespace: baseType.ContainingNamespace.IsGlobalNamespace 
					? null 
					: baseType.ContainingNamespace.ToDisplayString(),
				baseTypeNameIncludingGenerics: baseType.ContainingNamespace.IsGlobalNamespace ? baseType.GetTypeNameWithGenericParameters() : baseType.ContainingNamespace.ToDisplayString() + "." + baseType.GetTypeNameWithGenericParameters(),
				baseTypeDeclaration: baseType.GetObjectDeclaration(),
				baseTypeGenericConstraints: null,
				baseTypeTypeKind: baseType.TypeKind,
				filePath: baseType.ContainingAssembly.GlobalNamespace.ToDisplayString(), 
				accessibility: baseType.DeclaredAccessibility.ToString().ToLowerInvariant(),
				generateImplementationIds: discoverableAttribute!.GetArgumentOrDefault("generateImplementationIds", defaultValue: false),
				hasSingletonImplementations: discoverableAttribute!.GetArgumentOrDefault("hasSingletonImplementations", defaultValue: false),
				usings: new List<string>(),
				isPartial: true,
				externalDefinition: null);

		if (discoverableAttribute is null)
		{
			var hasDiscoverableAttribute = baseType.HasAttribute(ImplementationDiscoverySourceGenerator.DiscoverableAttributeName, ImplementationDiscoverySourceGenerator.DiscoverableAttributeNamespace, out discoverableAttribute);
			if (!hasDiscoverableAttribute) 
				return null;
		}

		var filePath = typeDeclarationSyntax.SyntaxTree.FilePath;

		var externalDefinition = externalBaseType is not null ? GetEnumDefinition(externalBaseType, discoverableAttribute, externalBaseType: null) : null;
		
		var definition = new EnumDefinition(
			customName: discoverableAttribute!.GetArgumentOrDefault("enumName", defaultValue: (string?)null),
			name: NameHelpers.GetNameWithoutGenerics(baseType.Name),
			typeParameters: typeDeclarationSyntax.TypeParameterList?.ToFullString(),
			enumNamespace: baseType.ContainingNamespace.IsGlobalNamespace 
				? null 
				: baseType.ContainingNamespace.ToDisplayString(),
			baseTypeNameIncludingGenerics: externalDefinition?.BaseTypeNameIncludingGenerics ?? baseType.Name + typeDeclarationSyntax.TypeParameterList?.ToFullString(),
			baseTypeDeclaration: baseType.GetObjectDeclaration(),
			baseTypeGenericConstraints: typeDeclarationSyntax.GetClassGenericConstraints(),
			baseTypeTypeKind: baseType.TypeKind,
			filePath: filePath, 
			accessibility: baseType.DeclaredAccessibility.ToString().ToLowerInvariant(),
			generateImplementationIds: discoverableAttribute!.GetArgumentOrDefault("generateImplementationIds", defaultValue: false),
			hasSingletonImplementations: discoverableAttribute!.GetArgumentOrDefault("hasSingletonImplementations", defaultValue: false),
			usings: typeDeclarationSyntax.GetUsings().ToList(),
			isPartial: typeDeclarationSyntax.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword)),
			externalDefinition: externalDefinition);
		
		return definition;
	}

	/// <summary>
	/// Checks if the node is an enum definition which has discoverable members or attributes.
	/// </summary>
	/// <returns>The enum definition. Or null if not applicable for this node.</returns>
	internal static IEnumModel? GetBaseType(GeneratorSyntaxContext context, CancellationToken cancellationToken)
	{
		if (context.Node is not AttributeSyntax { Parent.Parent: TypeDeclarationSyntax typeDeclarationSyntax }) return null;

		if (ModelExtensions.GetDeclaredSymbol(context.SemanticModel, typeDeclarationSyntax, cancellationToken) is not INamedTypeSymbol baseType) return null;

		var definition = GetEnumDefinition(baseType, discoverableAttribute: null, externalBaseType: null);
		return definition;
	}
}