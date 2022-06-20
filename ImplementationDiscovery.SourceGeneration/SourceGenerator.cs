using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using CodeChops.ImplementationDiscovery.SourceGeneration.SyntaxReceivers;
using CodeChops.ImplementationDiscovery.SourceGeneration.Entities;
using System.Diagnostics;
using CodeChops.ImplementationDiscovery.SourceGeneration.SourceBuilders;
using System.Linq;
using CodeChops.ImplementationDiscovery.SourceGeneration.Extensions;

namespace CodeChops.ImplementationDiscovery.SourceGeneration;

/// <summary>
/// Generates an enum that contains members that have the name of an implementation as value and the unitialized object as value.
/// This way implementations of a class or struct can easily be found statically.
/// This discovery will only work on enums with the correct attribute <see cref="DiscoverableAttributeName"/>.
/// </summary>
[Generator]
public class SourceGenerator : IIncrementalGenerator
{
	internal const string GenerateMethodName				= "CreateMember";
	internal const string DiscoverableAttributeName			= "DiscoverImplementationsAttribute";
	internal const string DiscoverableAttributeNamespace	= "CodeChops.ImplementationDiscovery";

	private static ImmutableArray<DiscoveredEnumMember> DiscoveredEnumMembers { get; } = Array.Empty<DiscoveredEnumMember>().ToImmutableArray();

	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		context.RegisterSourceOutput(
			source: this.FindImplementations(context),
			action: (context, implementations) => this.CreateSource(context, implementations!));
	}

	/// <summary>
	/// Finds the enum definitions and stores the enum definitions in the property for later use.
	/// </summary>
	private IncrementalValueProvider<ImmutableArray<ImplementationData>> FindImplementations(IncrementalGeneratorInitializationContext context)
	{
		// Get the enum members.
		var implementations = context.SyntaxProvider
			.CreateSyntaxProvider(
				predicate: static (syntaxNode, ct)	=> ImplementationSyntaxReceiver.CheckIfIsProbablyDiscoverableImplementation(syntaxNode),
				transform: static (context, ct)		=> ImplementationSyntaxReceiver.GetEnumMemberFromImplementation(context, ct))
			.Where(static definition => definition is not null)
			.Collect();
		
		return implementations!;
	}

	private void CreateSource(SourceProductionContext context, IEnumerable<ImplementationData> implementations)
	{
		var implementationsByName = implementations
			.OrderByDescending(i => i.Type.IsAbstract)
			.ThenByDescending(i => i.Type.IsGeneric())
			.GroupBy(i => $"{i.BaseType.Name}{i.Member.EnumName}");

		var enumsByName = implementationsByName.ToDictionary(i => i.Key, i => new EnumDefinition(
				name: i.First().Member.EnumName,
				enumNamespace: i.First().EnumNamespace,
				valueTypeName: i.First().BaseType.GetNameWithGenericParameters(),
				valueTypeNamespace: i.First().BaseType.ContainingNamespace.ToDisplayString(),
				discoverabilityMode: DiscoverabilityMode.Implementation,
				filePath: "",
				accessModifier: "public ",
				attributeMembers: i.Select(m => new EnumMember(m.Member.Name, m.Member.Value)),
				isStruct: false,
				valueTypeDefinition: GetDefinition(i.First().BaseType)));

		EnumSourceBuilder.CreateSource(context, DiscoveredEnumMembers, enumsByName);


		static string GetDefinition(ITypeSymbol baseType)
		{
			var accessibility = baseType.DeclaredAccessibility.ToString().ToLowerInvariant();
			var abstractOrEmpty = baseType.IsAbstract && baseType.TypeKind != TypeKind.Interface ? "abstract " : "";
			return $"{accessibility} {abstractOrEmpty}partial {baseType.GetClassTypeName()} {baseType.GetNameWithGenericParameters()}";
		}
	}
}