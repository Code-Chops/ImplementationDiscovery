using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using CodeChops.ImplementationDiscovery.SourceGeneration.Entities;
using CodeChops.ImplementationDiscovery.SourceGeneration.Extensions;
using System.Diagnostics;

namespace CodeChops.ImplementationDiscovery.SourceGeneration.SyntaxReceivers;

internal class ImplementationSyntaxReceiver
{
	/// <summary>
	/// The predicate for every node that is probably a discoverable implementation.
	/// </summary>
	internal static bool CheckIfIsProbablyDiscoverableImplementation(SyntaxNode syntaxNode)
	{
		// It should be a class, record or struct declaration.
		if (syntaxNode is not TypeDeclarationSyntax typeDeclarationSyntax || typeDeclarationSyntax is InterfaceDeclarationSyntax) return false;

		// It should inherit (from a base class/struct/record or interface).
		if (typeDeclarationSyntax.BaseList is null || typeDeclarationSyntax.BaseList.Types.Count == 0) return false;

		return true;
	}

	/// <summary>
	/// Gets the implementation details in the form of an enum member.
	/// </summary>
	/// <returns>The probably new enum member. Or null if not applicable for this node.</returns>
	internal static ImplementationData? GetEnumMemberFromImplementation(GeneratorSyntaxContext context, CancellationToken cancellationToken)
	{
		var typeDeclarationSyntax = (TypeDeclarationSyntax)context.Node;
		if (context.SemanticModel.GetDeclaredSymbol(typeDeclarationSyntax, cancellationToken) is not INamedTypeSymbol type) return null;

		if (type is null || type.IsStatic || type.BaseType is null || type.IsAbstract) return null;

		AttributeData? attribute = null;
		var implementsInterface = type.IsOrImplementsInterface(type => type.HasAttribute(SourceGenerator.DiscoverableAttributeName, SourceGenerator.DiscoverableAttributeNamespace, out attribute), out var baseType);		
		if (!implementsInterface)
		{
			var implementsClass = type.IsOrInheritsClass(type => type.HasAttribute(SourceGenerator.DiscoverableAttributeName, SourceGenerator.DiscoverableAttributeNamespace, out attribute), out baseType);
			if (!implementsClass) return null;
		}

		if (attribute is null) return null;

		var member = new DiscoveredEnumMember(
			enumName: $"Implementations",
			name: type.Name, 
			value: $"typeof({type.GetTypeFullName()})", 
			comment: null, 
			discoverabilityMode: DiscoverabilityMode.Implementation,
			filePath: typeDeclarationSyntax.SyntaxTree.FilePath, 
			linePosition: typeDeclarationSyntax.SyntaxTree.GetLineSpan(typeDeclarationSyntax.Span, cancellationToken).StartLinePosition);

		var implementedMember = new ImplementationData(
			Member: member, 
			Type: type,
			BaseType: baseType,
			EnumNamespace: type.ContainingNamespace.ToDisplayString());

		return implementedMember;
	}
}