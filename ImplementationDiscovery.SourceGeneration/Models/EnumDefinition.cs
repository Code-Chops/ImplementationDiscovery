  namespace CodeChops.ImplementationDiscovery.SourceGeneration.Models;

internal record EnumDefinition : IEnumModel
{
	public string EnumIdentifier { get; }
	public string Name { get; }
	public string? TypeParameters { get; }
	public string? Namespace { get; }
	public string? BaseTypeName { get; }
	public string? BaseTypeDeclaration { get; }
	public string? BaseTypeGenericConstraints { get; }
	public TypeKind? BaseTypeTypeKind { get; }
	public string FilePath { get; }
	public string Accessibility { get; }
	public bool GenerateImplementationIds { get; }
	public List<string> Usings { get; }
	
	public EnumDefinition(string? customName, TypeDeclarationSyntax baseTypeDeclarationSyntax, ITypeSymbol baseTypeSymbol, string filePath, bool generateImplementationIds, List<string> usings)
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
			filePath: filePath, 
			accessibility: baseTypeSymbol.DeclaredAccessibility.ToString().ToLowerInvariant(),
			generateImplementationIds: generateImplementationIds,
			usings: usings)
	{
	}

	public EnumDefinition(string? customName, string name, string? typeParameters, string? enumNamespace, string? baseTypeNameIncludingGenerics, string? baseTypeDeclaration, 
		string? baseTypeGenericConstraints, TypeKind? baseTypeTypeKind, string filePath, string accessibility, bool generateImplementationIds, List<string> usings)
	{
		this.Name = customName ?? name.Trim();
		this.TypeParameters = typeParameters?.Trim();
		this.Namespace = String.IsNullOrWhiteSpace(enumNamespace) ? null : enumNamespace;
		
		this.EnumIdentifier = $"{(this.Namespace is null ? null : $"{this.Namespace}.")}{name}";

		this.BaseTypeName = baseTypeNameIncludingGenerics?.Trim();
		this.BaseTypeDeclaration = baseTypeDeclaration?.Trim();
		this.BaseTypeGenericConstraints = baseTypeGenericConstraints?.Trim();
		this.BaseTypeTypeKind = baseTypeTypeKind;
		
		this.FilePath = filePath;
		this.Accessibility = accessibility.Replace("partial ", "").Replace("static ", "").Replace("abstract ", "").Trim();

		this.GenerateImplementationIds = generateImplementationIds;

		this.Usings = usings;
	}
}