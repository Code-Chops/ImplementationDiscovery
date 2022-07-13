using CodeChops.ImplementationDiscovery.SourceGeneration.Extensions;
using Microsoft.CodeAnalysis;

namespace CodeChops.ImplementationDiscovery.SourceGeneration.Entities;

public record EnumDefinition : IEnumEntity
{
	public string Identifier { get; }
	public string Name { get; }
	public string? Namespace { get; }
	/// <summary>
	/// When <see cref="DiscoverabilityMode"/> is set to Implementation, the ValueTypeName is equal to name of the base class / interface with generic type parameters.
	/// </summary>
	public string ValueTypeName { get; }
	/// <summary>
	/// When <see cref="DiscoverabilityMode"/> is set to Implementation, the ValueTypeNamespace is equal to namespace of the base class / interface.
	/// </summary>
	public string? ValueTypeNamespace { get; }
	/// <summary>
	/// AccessModifier + Type.
	/// Is NULL when <see cref="DiscoverabilityMode"/> is not set to Implementation.
	/// </summary>
	public string? BaseClassDefinition { get; }
	/// <summary>
	/// Base class name.
	/// Is NULL when <see cref="DiscoverabilityMode"/> is not set to Implementation.
	/// </summary>
	public string? BaseClassName { get; }
	public DiscoverabilityMode DiscoverabilityMode { get; }
	public string FilePath { get; }
	public string AccessModifier { get; }
	public List<EnumMember> MembersFromAttribute { get; }
	public bool IsStruct { get; }
	public bool GenerateIdsForImplementations { get; }
	
	/// <param name="valueTypeNamespace">Be aware of global namespaces!</param>
	public EnumDefinition(ITypeSymbol type, string valueTypeNameIncludingGenerics, string? valueTypeNamespace, DiscoverabilityMode discoverabilityMode,
		string filePath, string accessModifier, IEnumerable<EnumMember> attributeMembers, ITypeSymbol? baseType, bool implementationsHaveIds)
		: this(
			name: type.Name,
			enumNamespace: type.ContainingNamespace.IsGlobalNamespace 
				? null 
				: type.ContainingNamespace.ToDisplayString(),
			valueTypeNameIncludingGenerics: valueTypeNameIncludingGenerics,
			valueTypeNamespace: valueTypeNamespace,
			discoverabilityMode: discoverabilityMode, 
			filePath: filePath, 
			accessModifier: accessModifier, 
			membersFromAttribute: attributeMembers, 
			isStruct: type.TypeKind == TypeKind.Struct, 
			baseType: baseType, 
			generateIdsForImplementations: implementationsHaveIds)
	{
	}

	/// <param name="enumNamespace">Be aware of global namespaces!</param>
	/// <param name="valueTypeNamespace">Be aware of global namespaces!</param>
	public EnumDefinition(string name, string? enumNamespace, string valueTypeNameIncludingGenerics, string? valueTypeNamespace, DiscoverabilityMode discoverabilityMode,
		string filePath, string accessModifier, IEnumerable<EnumMember> membersFromAttribute, bool isStruct, ITypeSymbol? baseType, bool generateIdsForImplementations)
	{
		if (discoverabilityMode == DiscoverabilityMode.Implementation && baseType is null) throw new ArgumentException("Base type should be provided when the discoverability mode is set to implementation.");
		
		this.Name = name;
		this.Namespace = String.IsNullOrWhiteSpace(enumNamespace) ? null : enumNamespace;

		this.ValueTypeName = valueTypeNameIncludingGenerics;
		this.ValueTypeNamespace = valueTypeNamespace;
		this.BaseClassDefinition = discoverabilityMode == DiscoverabilityMode.Implementation ? baseType!.GetClassDefinition() : null;
		this.BaseClassName = baseType?.GetTypeNameWithGenericParameters();
		
		this.DiscoverabilityMode = discoverabilityMode;
		this.FilePath = filePath;
		this.AccessModifier = accessModifier.Replace("partial ", "").Replace("static ", "").Replace("abstract ", "");

		this.MembersFromAttribute = membersFromAttribute as List<EnumMember> ?? membersFromAttribute.ToList();
		this.IsStruct = isStruct;

		var genericParameterIndex = valueTypeNameIncludingGenerics.IndexOf('<');
		var valueTypeNameWithoutGenerics = genericParameterIndex <= 0 ? valueTypeNameIncludingGenerics : valueTypeNameIncludingGenerics.Substring(0, genericParameterIndex);
		this.Identifier = $"{(this.ValueTypeNamespace is null ? null : $"{this.ValueTypeNamespace}.")}{valueTypeNameWithoutGenerics}";

		this.GenerateIdsForImplementations = generateIdsForImplementations;
	}
}