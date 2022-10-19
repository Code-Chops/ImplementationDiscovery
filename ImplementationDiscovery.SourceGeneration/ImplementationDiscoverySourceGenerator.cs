using System.Text;
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
	internal const string DiscoverableAttributeName			= "DiscoverImplementationsAttribute";
	internal const string DiscoverableAttributeNamespace	= "CodeChops.ImplementationDiscovery";
	internal const string AllImplementationsEnumName		= "AllDiscoveredImplementations";
	internal const string ImplementationsEnumName			= "Enum";
	
	public void Initialize(IncrementalGeneratorInitializationContext initializationContext)
	{
		try
		{
			var valueProvider = FindImplementations(initializationContext).Combine(initializationContext.AnalyzerConfigOptionsProvider);
	
			initializationContext.RegisterSourceOutput(
				source: valueProvider,
				action: (c, provider) => CreateSource(c, provider.Left, provider.Right!));
		}
#pragma warning disable CS0168
		catch (Exception e)
#pragma warning restore CS0168
		{
			initializationContext.RegisterPostInitializationOutput(c => c.AddSource($"{nameof(ImplementationDiscoverySourceGenerator)}_Exception_{Guid.NewGuid()}", SourceText.From($"/*{e}*/", Encoding.UTF8)));
		}
	}

	/// <summary>
	/// Finds the enum definitions and stores the enum definitions in the property for later use.
	/// </summary>
	private static IncrementalValueProvider<ImmutableArray<IEnumModel>> FindImplementations(IncrementalGeneratorInitializationContext context)
	{
		var enumEntities = context.SyntaxProvider
			.CreateSyntaxProvider(
				predicate: ImplementationSyntaxReceiver.CheckIfIsProbablyImplementation,
				transform: static (context, ct)	=> ImplementationSyntaxReceiver.GetImplementation(context, ct))
			.Where(static definition => definition is not null)
			.SelectMany((s, _) => s.ToList())
			.Collect();
		
		return enumEntities!;
	}

	private static void CreateSource(SourceProductionContext context, IEnumerable<IEnumModel> entities, AnalyzerConfigOptionsProvider configOptionsProvider)
	{
		entities = entities as List<IEnumModel> ?? entities.ToList();
		var definitions = entities.OfType<EnumDefinition>().ToList();
		var members = entities.OfType<DiscoveredEnumMember>().ToList();

		var globallyListableEnumMembers = definitions.Where(definition => definition.TypeParameters is null && definition.BaseTypeName != nameof(Object));

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
			filePath: AllImplementationsEnumName,
			accessibility: "public",
			generateImplementationIds: false,
			hasSingletonImplementations: false,
			usings: new List<string>());
		
		definitions.Add(globalEnumDefinition);

		members.AddRange(globallyListableEnumMembers
			.Select((definition, index) => new DiscoveredEnumMember(
				enumIdentifier: $"{enumNamespace}.{AllImplementationsEnumName}",
				name: NameHelpers.GetNameWithoutGenerics(definition.Name), 
				isPartial: false, 
				@namespace: definition.Namespace, 
				declaration: "public class", 
				value: $"global::{(definition.Namespace is null ? null : $"{definition.Namespace}.")}{definition.Name}",
				filePath: AllImplementationsEnumName,
				linePosition: new LinePosition(index, 0),
				typeParameters: null,
				isConvertibleToConcreteType: false,
				accessibility: definition.Accessibility)));

		ImplementationsEnumSourceBuilder.CreateSource(context, members, definitions, configOptionsProvider);
		ImplementationIdSourceBuilder.CreateSource(context, members, definitions, configOptionsProvider);
	}
}