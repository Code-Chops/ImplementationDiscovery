using CodeChops.ImplementationDiscovery.SourceGeneration.Models;
using CodeChops.ImplementationDiscovery.SourceGeneration.SyntaxReceivers;
using CodeChops.ImplementationDiscovery.SourceGeneration.SourceBuilders;

namespace CodeChops.ImplementationDiscovery.SourceGeneration;

/// <summary>
/// Generates an enum that contains members that have the name of an implementation as value and the uninitialized object as value.
/// This way implementations of a class or struct can easily be found statically.
/// This discovery will only work on enums with the correct attribute <see cref="DiscoverableAttributeName"/>.
/// </summary>
[Generator]
public class ImplementationDiscoverySourceGenerator : IIncrementalGenerator
{
	internal const string GenerateMethodName				= "CreateMember";
	internal const string DiscoverableAttributeName			= "DiscoverImplementationsAttribute";
	internal const string DiscoverableAttributeNamespace	= "CodeChops.ImplementationDiscovery.Attributes";
	internal const string AllImplementationsEnumName		= "AllDiscoveredImplementations";
	internal const string ImplementationsEnumName			= "Enum";
	
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
		var members = entities.OfType<DiscoveredEnumMember>().ToList();

		var globallyListableEnumMembers = definitionsByIdentifier.Values.Where(definition => definition.BaseTypeName is null || !NameHelpers.HasGenericParameter(definition.BaseTypeName));

		configOptionsProvider.GlobalOptions.TryGetValue("build_property.RootNamespace", out var enumNamespace);

		var globalEnumDefinition = new EnumDefinition(
			customName: null,
			name: AllImplementationsEnumName,
			typeParameters: null,
			enumNamespace: enumNamespace,
			baseTypeNameIncludingGenerics: nameof(Object),
			baseTypeDeclaration: null,
			baseTypeGenericConstraints: null,
			baseTypeTypeKind: null,
			discoverabilityMode: DiscoverabilityMode.Implementation,
			filePath: AllImplementationsEnumName,
			accessModifier: "public",
			membersFromAttribute: globallyListableEnumMembers
				.Select(definition => new DiscoveredEnumMember(
					enumIdentifier: $"{enumNamespace}.{AllImplementationsEnumName}", 
					name: NameHelpers.GetNameWithoutGenerics(definition.BaseTypeName!), 
					isPartial: false, 
					@namespace: definition.Namespace, 
					declaration: "public class", 
					value: $"global::{(definition.Namespace is null ? null : $"{definition.Namespace}.")}{definition.Name}",
					comment: null,
					discoverabilityMode: DiscoverabilityMode.Implementation,
					filePath: AllImplementationsEnumName,
					linePosition: new LinePosition())),
			generateImplementationIds: false,
			hasNewableImplementations: false,
			usings: new List<string>());
		
		definitionsByIdentifier.Add(AllImplementationsEnumName, globalEnumDefinition);
		
		EnumSourceBuilder.CreateSource(context, members, definitionsByIdentifier, configOptionsProvider);
		ImplementationIdSourceBuilder.CreateSource(context, members, definitionsByIdentifier, configOptionsProvider);
	}
}