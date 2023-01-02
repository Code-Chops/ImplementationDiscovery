using System.Text;
using CodeChops.ImplementationDiscovery.SourceGeneration.Models;
using CodeChops.ImplementationDiscovery.SourceGeneration.SyntaxReceivers;
using CodeChops.ImplementationDiscovery.SourceGeneration.SourceBuilders;

namespace CodeChops.ImplementationDiscovery.SourceGeneration;

/// <summary>
/// Generates an enum that contains members that have the name of an implementation as name and a discovered object as value.
/// This way implementations of a class, struct, or interface can easily be found statically.
/// This discovery will only work on abstract classes or interfaces with a 'DiscoverImplementations' attribute/>.
/// </summary>
[Generator]
public class ImplementationDiscoverySourceGenerator : IIncrementalGenerator
{
	public void Initialize(IncrementalGeneratorInitializationContext initializationContext)
	{
		try
		{
			var valueProvider = FindImplementations(initializationContext).Combine(initializationContext.AnalyzerConfigOptionsProvider);
	
			initializationContext.RegisterSourceOutput(
				source: valueProvider,
				action: (c, provider) => CreateSource(c, provider.Left, provider.Right!));
		}

		catch (Exception e)
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
				predicate: ImplementationSyntaxReceiver.CheckIfIsProbablyDiscoverableBaseOrImplementation,
				transform: static (context, ct)	=> ImplementationSyntaxReceiver.GetImplementation(context, ct).Append(ImplementationSyntaxReceiver.GetBaseType(context, ct)))
			.Where(static definition => definition is not null)
			.SelectMany((s, _) => s.ToList())
			.Collect();
		
		return enumEntities!;
	}

	private static void CreateSource(SourceProductionContext context, IEnumerable<IEnumModel> entities, AnalyzerConfigOptionsProvider configOptionsProvider)
	{
		entities = entities as List<IEnumModel> ?? entities.ToList();
		var allDefinitions = entities.OfType<EnumDefinition>().ToList();
		var allMembers = entities.OfType<DiscoveredEnumMember>().ToList();

		var globallyListableEnumMembers = allDefinitions.Where(definition => definition.TypeParameters is null && definition.BaseTypeNameIncludingGenerics != nameof(Object));

		var globalEnumDefinition = new GlobalEnumDefinition(configOptionsProvider);
		
		allDefinitions.Add(globalEnumDefinition);

		allMembers.AddRange(globallyListableEnumMembers
			.Select((definition, index) => new DiscoveredEnumMember(
				enumIdentifier: $"{globalEnumDefinition.Namespace}.{Constants.AllImplementationsEnumName}",
				name: definition.Name, 
				isPartial: false, 
				@namespace: definition.Namespace, 
				declaration: "public class", 
				value: $"global::{(definition.Namespace is null ? null : $"{definition.Namespace}.")}{definition.Name}",
				filePath: Constants.AllImplementationsEnumName,
				linePosition: new LinePosition(index, 0),
				typeParameters: null,
				isConvertibleToConcreteType: false,
				accessibility: definition.Accessibility,
				instanceCreationMethod: InstanceCreationMethod.Uninitialized,
				hasComments: false)));

		var allEnums = GetDiscoveredEnums(allMembers, allDefinitions)
			.OrderBy(e => e.Definition.Name)
			.ThenByDescending(e => e.Definition.Namespace)
			.ToList();

		ImplementationsEnumSourceBuilder.CreateSource(context, allEnums, configOptionsProvider);
		ImplementationIdSourceBuilder.CreateSource(context, allEnums, configOptionsProvider);
	}

	internal static IEnumerable<DiscoveredEnum> GetDiscoveredEnums(IEnumerable<DiscoveredEnumMember> allDiscoveredMembers, IEnumerable<EnumDefinition> definitions)
	{
		var definitionsByIdentifier = definitions
			.GroupBy(definition => definition.EnumIdentifier)
			.ToDictionary(group => group.Key, group => group.First());
		
		var membersByIdentifier = allDiscoveredMembers
			.GroupBy(d => d.EnumIdentifier)
			.ToDictionary(group => group.Key, group => group.ToList());
            
		var discoveredEnums = definitionsByIdentifier
			.ToDictionary(group => group.Value, group => membersByIdentifier.TryGetValue(group.Key, out var members) ? members : new List<DiscoveredEnumMember>())
			.Select(group => new DiscoveredEnum(group.Key, group.Value.OrderBy(member => member.Value).ToImmutableList()));
		
		return discoveredEnums;
	}
}