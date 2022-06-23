using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using CodeChops.ImplementationDiscovery.SourceGeneration.SyntaxReceivers;
using CodeChops.ImplementationDiscovery.SourceGeneration.Entities;
using CodeChops.ImplementationDiscovery.SourceGeneration.SourceBuilders;

namespace CodeChops.ImplementationDiscovery.SourceGeneration;

/// <summary>
/// Generates an enum that contains members that have the name of an implementation as value and the uninitialized object as value.
/// This way implementations of a class or struct can easily be found statically.
/// This discovery will only work on enums with the correct attribute <see cref="DiscoverableAttributeName"/>.
/// </summary>
[Generator]
public class SourceGenerator : IIncrementalGenerator
{
	internal const string GenerateMethodName				= "CreateMember";
	internal const string DiscoverableAttributeName			= "DiscoverImplementationsAttribute";
	internal const string DiscoverableAttributeNamespace	= "CodeChops.ImplementationDiscovery";

	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		context.RegisterSourceOutput(
			source: FindImplementations(context),
			action: (c, implementations) => CreateSource(c, implementations!));
	}

	/// <summary>
	/// Finds the enum definitions and stores the enum definitions in the property for later use.
	/// </summary>
	private static IncrementalValueProvider<ImmutableArray<IEnumEntity>> FindImplementations(IncrementalGeneratorInitializationContext context)
	{
		var enumEntities = context.SyntaxProvider
			.CreateSyntaxProvider(
				predicate: static (syntaxNode, ct)	=> ImplementationSyntaxReceiver.CheckIfIsProbablyDiscoverableBaseOrImplementation(syntaxNode, ct),
				transform: static (context, ct)		=> ImplementationSyntaxReceiver.GetBaseType(context, ct) ?? ImplementationSyntaxReceiver.GetEnumMemberFromImplementation(context, ct))
			.Where(static definition => definition is not null)
			.Collect();
		
		return enumEntities!;
	}

	private static void CreateSource(SourceProductionContext context, IEnumerable<IEnumEntity> entities)
	{
		entities = entities as List<IEnumEntity> ?? entities.ToList();
		var definitionsByIdentifier = entities.OfType<EnumDefinition>().ToDictionary(d => d.Identifier);
		var members = entities.OfType<DiscoveredEnumMember>();

		EnumSourceBuilder.CreateSource(context, members, definitionsByIdentifier);
	}
}