  using System.Text.RegularExpressions;

  namespace CodeChops.ImplementationDiscovery.SourceGeneration.Models;

internal record EnumDefinition : IEnumModel
{
	public string EnumIdentifier { get; }
	public string Name { get; }
	public string? TypeParameters { get; }
	public string? Namespace { get; }
	public string BaseTypeName { get; }
	public string? BaseTypeDeclaration { get; }
	public string? BaseTypeGenericConstraints { get; }
	public TypeKind? BaseTypeTypeKind { get; }
	public string FilePath { get; }
	public string Accessibility { get; }
	public bool GenerateImplementationIds { get; }
	public bool HasSingletonImplementations { get; }
	public List<string> Usings { get; }

	private static Regex IsValidName { get; } = new(@"^[a-zA-Z_]\w*(\.[a-zA-Z_]\w*)*$");
	
	public EnumDefinition(string? customName, TypeDeclarationSyntax baseTypeDeclarationSyntax, ITypeSymbol baseTypeSymbol, string filePath, 
		bool generateImplementationIds, bool hasSingletonImplementations, List<string> usings)
		: this(
			customName: customName,
			name: NameHelpers.GetNameWithoutGenerics(baseTypeSymbol.Name),
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
			hasSingletonImplementations: hasSingletonImplementations,
			usings: usings)
	{
	}

	public EnumDefinition(string? customName, string name, string? typeParameters, string? enumNamespace, string baseTypeNameIncludingGenerics, 
		string? baseTypeDeclaration, string? baseTypeGenericConstraints, TypeKind? baseTypeTypeKind, string filePath, string accessibility, 
		bool generateImplementationIds, bool hasSingletonImplementations, List<string> usings)
	{
		this.Name = customName ?? $"{GetName()}{ImplementationDiscoverySourceGenerator.ImplementationsEnumName}";
		this.TypeParameters = typeParameters?.Trim();
		this.Namespace = String.IsNullOrWhiteSpace(enumNamespace) ? null : enumNamespace;
		
		this.EnumIdentifier = $"{(this.Namespace is null ? null : $"{this.Namespace}.")}{name}";

		this.BaseTypeName = baseTypeNameIncludingGenerics.Trim();
		this.BaseTypeDeclaration = baseTypeDeclaration?.Trim();
		this.BaseTypeGenericConstraints = baseTypeGenericConstraints?.Trim();
		this.BaseTypeTypeKind = baseTypeTypeKind;
		
		this.FilePath = filePath;
		this.Accessibility = accessibility.Replace("partial ", "").Replace("static ", "").Replace("abstract ", "").Trim();

		this.GenerateImplementationIds = generateImplementationIds;
		this.HasSingletonImplementations = hasSingletonImplementations;
		
		this.Usings = usings;


		string GetName()
		{
			var newName = name;
			
			if (newName.EndsWith("Base"))
				newName = newName.Substring(0, newName.Length - 4);

			if (baseTypeDeclaration?.Contains("interface") == true && newName[0] == 'I')
				newName = newName.Substring(1);
			
			return IsValidName.IsMatch(newName) ? newName : name;
		}
	}
}