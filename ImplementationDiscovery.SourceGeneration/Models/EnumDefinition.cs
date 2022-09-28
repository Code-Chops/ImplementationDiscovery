  namespace CodeChops.ImplementationDiscovery.SourceGeneration.Models;

internal record EnumDefinition : IEnumModel
{
	public string Identifier { get; }
	public string Name { get; }
	public string? TypeParameters { get; }
	public string? Namespace { get; }
	public string? BaseTypeName { get; }
	public string? BaseTypeDeclaration { get; }
	public string? BaseTypeGenericConstraints { get; }
	public TypeKind? BaseTypeTypeKind { get; }
	public DiscoverabilityMode DiscoverabilityMode { get; }
	public string FilePath { get; }
	public string AccessModifier { get; }
	public List<EnumMember> MembersFromAttribute { get; }
	public bool GenerateTypeIdsForImplementations { get; }
	public bool HasNewableImplementations { get; }
	public List<string> Usings { get; }
	
	public EnumDefinition(string? customName, TypeDeclarationSyntax baseTypeDeclarationSyntax, ITypeSymbol baseTypeSymbol, DiscoverabilityMode discoverabilityMode, string filePath, 
		IEnumerable<EnumMember> membersFromAttribute, bool generateIdsForImplementations, bool hasNewableImplementations, List<string> usings)
		: this(
			customName: customName,
			name: $"{NameHelpers.GetNameWithoutGenerics(baseTypeSymbol.Name)}{ImplementationDiscoverySourceGenerator.ImplementationsEnumName}",
			typeParameters: baseTypeDeclarationSyntax.TypeParameterList?.ToFullString(),
			enumNamespace: baseTypeSymbol.ContainingNamespace.IsGlobalNamespace 
				? null 
				: baseTypeSymbol.ContainingNamespace.ToDisplayString(),
			baseTypeNameIncludingGenerics: baseTypeSymbol.GetTypeNameWithGenericParameters(),
			baseTypeDeclaration: baseTypeSymbol.GetObjectDeclaration(),
			baseTypeGenericConstraints: baseTypeDeclarationSyntax.GetClassGenericConstraints(),
			baseTypeTypeKind: baseTypeSymbol.TypeKind,
			discoverabilityMode: discoverabilityMode, 
			filePath: filePath, 
			accessModifier: baseTypeDeclarationSyntax.Modifiers.ToFullString(), 
			membersFromAttribute: membersFromAttribute,
			generateTypeIdsForImplementations: generateIdsForImplementations,
			hasNewableImplementations: hasNewableImplementations,
			usings: usings)
	{
	}

	public EnumDefinition(string? customName, string name, string? typeParameters, string? enumNamespace, string? baseTypeNameIncludingGenerics, string? baseTypeDeclaration, string? baseTypeGenericConstraints, TypeKind? baseTypeTypeKind, DiscoverabilityMode discoverabilityMode, 
		string filePath, string accessModifier, IEnumerable<EnumMember> membersFromAttribute, bool generateTypeIdsForImplementations, bool hasNewableImplementations, List<string> usings)
	{
		this.Name = customName ?? name;
		this.TypeParameters = typeParameters;
		this.Namespace = String.IsNullOrWhiteSpace(enumNamespace) ? null : enumNamespace;
		
		this.Identifier = $"{(this.Namespace is null ? null : $"{this.Namespace}.")}{name}";

		this.BaseTypeName = baseTypeNameIncludingGenerics;
		this.BaseTypeDeclaration = baseTypeDeclaration;
		this.BaseTypeGenericConstraints = baseTypeGenericConstraints;
		this.BaseTypeTypeKind = baseTypeTypeKind;
		
		this.DiscoverabilityMode = discoverabilityMode;
		this.FilePath = filePath;
		this.AccessModifier = accessModifier.Replace("partial ", "").Replace("static ", "").Replace("abstract ", "").Trim();

		this.MembersFromAttribute = membersFromAttribute as List<EnumMember> ?? membersFromAttribute.ToList();

		this.GenerateTypeIdsForImplementations = generateTypeIdsForImplementations;
		this.HasNewableImplementations = hasNewableImplementations;

		this.Usings = usings;
	}
}