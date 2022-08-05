  namespace CodeChops.ImplementationDiscovery.SourceGeneration.Models;

internal record EnumDefinition : IEnumModel
{
	public string Identifier { get; }
	public string Name { get; }
	public string? Namespace { get; }
	public string? BaseTypeName { get; }
	public string? BaseTypeDeclaration { get; }
	public TypeKind? BaseTypeTypeKind { get; }
	public DiscoverabilityMode DiscoverabilityMode { get; }
	public string FilePath { get; }
	public string AccessModifier { get; }
	public List<EnumMember> MembersFromAttribute { get; }
	public bool GenerateTypeIdsForImplementations { get; }
	public bool HasNewableImplementations { get; }
	
	public EnumDefinition(string name, ITypeSymbol baseType, DiscoverabilityMode discoverabilityMode, string filePath, string accessModifier, 
		IEnumerable<EnumMember> membersFromAttribute, bool generateIdsForImplementations, bool hasNewableImplementations)
		: this(
			name: name,
			enumNamespace: baseType.ContainingNamespace.IsGlobalNamespace 
				? null 
				: baseType.ContainingNamespace.ToDisplayString(),
			baseTypeNameIncludingGenerics: baseType.GetTypeNameWithGenericParameters(),
			baseTypeDeclaration: baseType.GetObjectDeclaration(),
			baseTypeTypeKind: baseType.TypeKind,
			discoverabilityMode: discoverabilityMode, 
			filePath: filePath, 
			accessModifier: accessModifier, 
			membersFromAttribute: membersFromAttribute,
			generateTypeIdsForImplementations: generateIdsForImplementations,
			hasNewableImplementations: hasNewableImplementations)
	{
	}

	/// <param name="enumNamespace">Be aware of global namespaces!</param>
	public EnumDefinition(string name, string? enumNamespace, string? baseTypeNameIncludingGenerics, string? baseTypeDeclaration, TypeKind? baseTypeTypeKind, DiscoverabilityMode discoverabilityMode, 
		string filePath, string accessModifier, IEnumerable<EnumMember> membersFromAttribute, bool generateTypeIdsForImplementations, bool hasNewableImplementations)
	{
		this.Name = name;
		this.Namespace = String.IsNullOrWhiteSpace(enumNamespace) ? null : enumNamespace;
		
		var baseTypeNameWithoutGenerics = baseTypeNameIncludingGenerics is null 
			? null 
			: NameHelpers.GetNameWithoutGenerics(baseTypeNameIncludingGenerics);
		
		this.Identifier = $"{(this.Namespace is null ? null : $"{this.Namespace}.")}{baseTypeNameWithoutGenerics}";

		this.BaseTypeName = baseTypeNameIncludingGenerics;
		this.BaseTypeDeclaration = baseTypeDeclaration;
		this.BaseTypeTypeKind = baseTypeTypeKind;
		
		this.DiscoverabilityMode = discoverabilityMode;
		this.FilePath = filePath;
		this.AccessModifier = accessModifier.Replace("partial ", "").Replace("static ", "").Replace("abstract ", "").Trim();

		this.MembersFromAttribute = membersFromAttribute as List<EnumMember> ?? membersFromAttribute.ToList();

		this.GenerateTypeIdsForImplementations = generateTypeIdsForImplementations;
		this.HasNewableImplementations = hasNewableImplementations;
	}
}