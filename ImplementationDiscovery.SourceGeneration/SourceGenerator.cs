using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using CodeChops.ImplementationDiscovery.SourceGeneration.SyntaxReceivers;
using CodeChops.ImplementationDiscovery.SourceGeneration.Entities;
using CodeChops.ImplementationDiscovery.SourceGeneration.Helpers;
using CodeChops.ImplementationDiscovery.SourceGeneration.SourceBuilders;
using Microsoft.CodeAnalysis.Text;

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
	internal const string ImplementationsEnumName			= "Implementations";
	private const string AllImplementationsEnumName			= "AllImplementations";
	private const string GlobalEnumNamespace				= $"{nameof(CodeChops)}.{nameof(ImplementationDiscovery)}";

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

		var globallyListableEnumMembers = definitionsByIdentifier.Values.Where(definition => !definition.OuterClassName?.HasGenericParameter() ?? true);
		
		var globalEnumDefinition = new EnumDefinition(
			name: SourceGenerator.AllImplementationsEnumName,
			enumNamespace: GlobalEnumNamespace,
			valueTypeNameIncludingGenerics: null,
			valueTypeNamespace: null,
			discoverabilityMode: DiscoverabilityMode.Implementation,
			filePath: AllImplementationsEnumName,
			accessModifier: "public",
			membersFromAttribute: globallyListableEnumMembers
				.Select(definition => new DiscoveredEnumMember(
					enumIdentifier: AllImplementationsEnumName, 
					name: definition.OuterClassName!.GetClassNameWithoutGenerics(), 
					isPartial: false, 
					@namespace: definition.Namespace, 
					definition: "public class", 
					value: $"typeof(global::{(definition.Namespace is null ? null : $"{definition.Namespace}.")}{(definition.OuterClassName is null ? null : $"{definition.OuterClassName}.")}{definition.Name})",
					comment: null,
					discoverabilityMode: DiscoverabilityMode.Implementation,
					filePath: AllImplementationsEnumName,
					linePosition: new LinePosition())),
			isStruct: false,
			outerClassDefinition: null,
			outerClassName: null,
			generateIdsForImplementations: false);
		
		definitionsByIdentifier.Add(AllImplementationsEnumName, globalEnumDefinition);
		
		EnumSourceBuilder.CreateSource(context, members, definitionsByIdentifier);
		ImplementationIdSourceBuilder.CreateSource(context, members, definitionsByIdentifier);
	}
}