  namespace CodeChops.ImplementationDiscovery.SourceGeneration.Entities;

public record EnumDefinition : IEnumEntity
{
	public string Identifier { get; }
	public string Name { get; }
	public string? Namespace { get; }
	/// <summary>
	/// When <see cref="DiscoverabilityMode"/> is set to Implementation, the ValueTypeName is equal to name of the base class / interface with generic type parameters.
	/// </summary>
	public string? ValueTypeName { get; }
	/// <summary>
	/// When <see cref="DiscoverabilityMode"/> is set to Implementation, the ValueTypeNamespace is equal to namespace of the base class / interface.
	/// </summary>
	public string? ValueTypeNamespace { get; }
	/// <summary>
	/// AccessModifier + Type.
	/// </summary>
	public string? OuterClassDefinition { get; }
	/// <summary>
	/// Base class name.
	/// </summary>
	public string? OuterClassName { get; }
	public DiscoverabilityMode DiscoverabilityMode { get; }
	public string FilePath { get; }
	public string AccessModifier { get; }
	public List<EnumMember> MembersFromAttribute { get; }
	public bool IsStruct { get; }
	public bool GenerateIdsForImplementations { get; }
	
	/// <param name="valueTypeNamespace">Be aware of global namespaces!</param>
	public EnumDefinition(ITypeSymbol type, string valueTypeNameIncludingGenerics, string? valueTypeNamespace, DiscoverabilityMode discoverabilityMode,
		string filePath, string accessModifier, IEnumerable<EnumMember> attributeMembers, ITypeSymbol? outerClassType, bool implementationsHaveIds)
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
			outerClassDefinition: outerClassType?.GetObjectDefinition(), 
			outerClassName: outerClassType?.GetTypeNameWithGenericParameters(), 
			generateIdsForImplementations: implementationsHaveIds)
	{
	}

	/// <param name="enumNamespace">Be aware of global namespaces!</param>
	/// <param name="valueTypeNamespace">Be aware of global namespaces!</param>
	public EnumDefinition(string name, string? enumNamespace, string? valueTypeNameIncludingGenerics, string? valueTypeNamespace, DiscoverabilityMode discoverabilityMode,
		string filePath, string accessModifier, IEnumerable<EnumMember> membersFromAttribute, bool isStruct, string? outerClassDefinition, string? outerClassName, bool generateIdsForImplementations)
	{
		this.Name = name;
		this.Namespace = String.IsNullOrWhiteSpace(enumNamespace) ? null : enumNamespace;

		this.ValueTypeName = valueTypeNameIncludingGenerics;
		this.ValueTypeNamespace = valueTypeNamespace;
		this.OuterClassDefinition = outerClassDefinition;
		this.OuterClassName = outerClassName;
		
		this.DiscoverabilityMode = discoverabilityMode;
		this.FilePath = filePath;
		this.AccessModifier = accessModifier.Replace("partial ", "").Replace("static ", "").Replace("abstract ", "").Trim();

		this.MembersFromAttribute = membersFromAttribute as List<EnumMember> ?? membersFromAttribute.ToList();
		this.IsStruct = isStruct;

		var valueTypeNameWithoutGenerics = valueTypeNameIncludingGenerics is null 
			? null 
			: ClassNameHelpers.GetClassNameWithoutGenerics(valueTypeNameIncludingGenerics);
		this.Identifier = $"{(this.ValueTypeNamespace is null ? null : $"{this.ValueTypeNamespace}.")}{valueTypeNameWithoutGenerics}";

		this.GenerateIdsForImplementations = generateIdsForImplementations;
	}
}