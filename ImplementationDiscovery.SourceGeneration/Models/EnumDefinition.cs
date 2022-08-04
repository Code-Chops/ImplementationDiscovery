  namespace CodeChops.ImplementationDiscovery.SourceGeneration.Models;

internal record EnumDefinition : IEnumModel
{
	public string Identifier { get; }
	public string Name { get; }
	public string? Namespace { get; }
	public string? ValueTypeName { get; }
	public string? ValueTypeDeclaration { get; }
	public TypeKind? ValueTypeTypeKind { get; }
	public DiscoverabilityMode DiscoverabilityMode { get; }
	public string FilePath { get; }
	public string AccessModifier { get; }
	public List<EnumMember> MembersFromAttribute { get; }
	public bool GenerateIdsForImplementations { get; }
	
	/// <param name="valueTypeNamespace">Be aware of global namespaces!</param>
	public EnumDefinition(string name, ITypeSymbol valueType, DiscoverabilityMode discoverabilityMode, string filePath, string accessModifier, 
		IEnumerable<EnumMember> membersFromAttribute, bool generateIdsForImplementations)
		: this(
			name: name,
			enumNamespace: valueType.ContainingNamespace.IsGlobalNamespace 
				? null 
				: valueType.ContainingNamespace.ToDisplayString(),
			valueTypeNameIncludingGenerics: valueType.GetTypeNameWithGenericParameters(),
			valueTypeDeclaration: valueType.GetObjectDeclaration(),
			valueTypeTypeKind: valueType?.TypeKind,
			discoverabilityMode: discoverabilityMode, 
			filePath: filePath, 
			accessModifier: accessModifier, 
			membersFromAttribute: membersFromAttribute,
			generateIdsForImplementations: generateIdsForImplementations)
	{
	}

	/// <param name="enumNamespace">Be aware of global namespaces!</param>
	/// <param name="valueTypeNamespace">Be aware of global namespaces!</param>
	public EnumDefinition(string name, string? enumNamespace, string? valueTypeNameIncludingGenerics, string? valueTypeDeclaration, TypeKind? valueTypeTypeKind,
		DiscoverabilityMode discoverabilityMode, string filePath, string accessModifier, IEnumerable<EnumMember> membersFromAttribute, bool generateIdsForImplementations)
	{
		this.Name = name;
		this.Namespace = String.IsNullOrWhiteSpace(enumNamespace) ? null : enumNamespace;
		
		var valueTypeNameWithoutGenerics = valueTypeNameIncludingGenerics is null 
			? null 
			: NameHelpers.GetNameWithoutGenerics(valueTypeNameIncludingGenerics);
		
		this.Identifier = $"{(this.Namespace is null ? null : $"{this.Namespace}.")}{valueTypeNameWithoutGenerics}";

		this.ValueTypeName = valueTypeNameIncludingGenerics;
		this.ValueTypeDeclaration = valueTypeDeclaration;
		this.ValueTypeTypeKind = valueTypeTypeKind;
		
		this.DiscoverabilityMode = discoverabilityMode;
		this.FilePath = filePath;
		this.AccessModifier = accessModifier.Replace("partial ", "").Replace("static ", "").Replace("abstract ", "").Trim();

		this.MembersFromAttribute = membersFromAttribute as List<EnumMember> ?? membersFromAttribute.ToList();

		this.GenerateIdsForImplementations = generateIdsForImplementations;
	}
}