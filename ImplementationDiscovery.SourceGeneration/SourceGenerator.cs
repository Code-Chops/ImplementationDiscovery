﻿using CodeChops.ImplementationDiscovery.SourceGeneration.Models;
using CodeChops.ImplementationDiscovery.SourceGeneration.SyntaxReceivers;
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
	internal const string AllImplementationsEnumName		= "AllDiscoveredImplementations";
	internal const string ImplementationsEnumName			= "TypeIdentities";
	
	public void Initialize(IncrementalGeneratorInitializationContext initializationContext)
	{		
		var valueProvider = FindImplementations(initializationContext).Combine(initializationContext.AnalyzerConfigOptionsProvider);

		initializationContext.RegisterSourceOutput(
			source: valueProvider,
			action: (c, provider) => CreateSource(c, provider.Left, provider.Right!));
	}

	/// <summary>
	/// Finds the enum definitions and stores the enum definitions in the property for later use.
	/// </summary>
	private static IncrementalValueProvider<ImmutableArray<IEnumModel>> FindImplementations(IncrementalGeneratorInitializationContext context)
	{
		var enumEntities = context.SyntaxProvider
			.CreateSyntaxProvider(
				predicate: ImplementationSyntaxReceiver.CheckIfIsProbablyDiscoverableBaseOrImplementation,
				transform: static (context, ct)		=> ImplementationSyntaxReceiver.GetBaseType(context, ct) ?? ImplementationSyntaxReceiver.GetEnumMemberFromImplementation(context, ct))
			.Where(static definition => definition is not null)
			.Collect();
		
		return enumEntities!;
	}

	private static void CreateSource(SourceProductionContext context, IEnumerable<IEnumModel> entities, AnalyzerConfigOptionsProvider configOptionsProvider)
	{
		entities = entities as List<IEnumModel> ?? entities.ToList();
		var definitionsByIdentifier = entities.OfType<EnumDefinition>().ToDictionary(d => d.Identifier);
		var members = entities.OfType<DiscoveredEnumMember>();

		var globallyListableEnumMembers = definitionsByIdentifier.Values.Where(definition => definition.ValueTypeName is null || !NameHelpers.HasGenericParameter(definition.ValueTypeName));

		configOptionsProvider.GlobalOptions.TryGetValue("build_property.RootNamespace", out var enumNamespace);

		var globalEnumDefinition = new EnumDefinition(
			name: SourceGenerator.AllImplementationsEnumName,
			enumNamespace: enumNamespace,
			valueTypeNameIncludingGenerics: AllImplementationsEnumName,
			valueTypeDeclaration: null,
			valueTypeTypeKind: null,
			discoverabilityMode: DiscoverabilityMode.Implementation,
			filePath: AllImplementationsEnumName,
			accessModifier: "public",
			membersFromAttribute: globallyListableEnumMembers
				.Select(definition => new DiscoveredEnumMember(
					enumIdentifier: AllImplementationsEnumName, 
					name: NameHelpers.GetNameWithoutGenerics(definition.ValueTypeName!), 
					isPartial: false, 
					@namespace: definition.Namespace, 
					declaration: "public class", 
					value: $"typeof(global::{(definition.Namespace is null ? null : $"{definition.Namespace}.")}{(definition.ValueTypeName is null ? null : $"{definition.ValueTypeName}.")}{definition.Name})",
					comment: null,
					discoverabilityMode: DiscoverabilityMode.Implementation,
					filePath: AllImplementationsEnumName,
					linePosition: new LinePosition())),
			generateIdsForImplementations: false);
		
		definitionsByIdentifier.Add(AllImplementationsEnumName, globalEnumDefinition);
		
		EnumSourceBuilder.CreateSource(context, members, definitionsByIdentifier);
		ImplementationIdSourceBuilder.CreateSource(context, members, definitionsByIdentifier);
	}
}