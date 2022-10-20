using System.Text.RegularExpressions;

namespace CodeChops.ImplementationDiscovery.SourceGeneration.Models;

internal record EnumDefinition : IEnumModel
{
	public string EnumIdentifier { get; }
	public string Name { get; }
	public string? TypeParameters { get; }
	public string? Namespace { get; }
	public string BaseTypeNameIncludingGenerics { get; }
	public string? BaseTypeDeclaration { get; }
	public string? BaseTypeGenericConstraints { get; }
	public TypeKind? BaseTypeTypeKind { get; }
	public string FilePath { get; }
	public string Accessibility { get; }
	public bool GenerateImplementationIds { get; }
	public bool HasSingletonImplementations { get; }
	public List<string> Usings { get; }
	public bool IsPartial { get; }
	public EnumDefinition? ExternalDefinition { get; }
	
	private static Regex IsValidName { get; } = new(@"^[a-zA-Z_]\w*(\.[a-zA-Z_]\w*)*$");
	
	public EnumDefinition(string? customName, TypeDeclarationSyntax baseTypeDeclarationSyntax, ITypeSymbol baseTypeSymbol, string filePath, bool generateImplementationIds, 
		bool hasSingletonImplementations, List<string> usings, EnumDefinition? externalDefinition)
		: this(
			customName: customName,
			name: NameHelpers.GetNameWithoutGenerics(baseTypeSymbol.Name),
			typeParameters: baseTypeDeclarationSyntax.TypeParameterList?.ToFullString(),
			enumNamespace: baseTypeSymbol.ContainingNamespace.IsGlobalNamespace 
				? null 
				: baseTypeSymbol.ContainingNamespace.ToDisplayString(),
			baseTypeNameIncludingGenerics: baseTypeSymbol.Name + baseTypeDeclarationSyntax.TypeParameterList?.ToFullString(),
			baseTypeDeclaration: baseTypeSymbol.GetObjectDeclaration(),
			baseTypeGenericConstraints: baseTypeDeclarationSyntax.GetClassGenericConstraints(),
			baseTypeTypeKind: baseTypeSymbol.TypeKind,
			filePath: filePath, 
			accessibility: baseTypeSymbol.DeclaredAccessibility.ToString().ToLowerInvariant(),
			generateImplementationIds: generateImplementationIds,
			hasSingletonImplementations: hasSingletonImplementations,
			usings: usings,
			isPartial: baseTypeDeclarationSyntax.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword)),
			externalDefinition: externalDefinition)
	{
	}

	public EnumDefinition(string? customName, string name, string? typeParameters, string? enumNamespace, string baseTypeNameIncludingGenerics, 
		string? baseTypeDeclaration, string? baseTypeGenericConstraints, TypeKind? baseTypeTypeKind, string filePath, string accessibility, 
		bool generateImplementationIds, bool hasSingletonImplementations, List<string> usings, bool isPartial, EnumDefinition? externalDefinition)
	{
		this.Name = GetName(customName, name, isInterface: baseTypeDeclaration?.Contains("interface") == true, isProxy: externalDefinition is not null);
		this.TypeParameters = typeParameters?.Trim();
		this.Namespace = String.IsNullOrWhiteSpace(enumNamespace) ? null : enumNamespace;
		
		this.EnumIdentifier = $"{(this.Namespace is null ? null : $"{this.Namespace}.")}{name}";

		this.BaseTypeNameIncludingGenerics = baseTypeNameIncludingGenerics.Trim();
		this.BaseTypeDeclaration = baseTypeDeclaration?.Trim();
		this.BaseTypeGenericConstraints = baseTypeGenericConstraints?.Trim();
		this.BaseTypeTypeKind = baseTypeTypeKind;
		
		this.FilePath = filePath;
		this.Accessibility = accessibility.Replace("partial ", "").Replace("static ", "").Replace("abstract ", "").Trim();

		this.GenerateImplementationIds = generateImplementationIds;
		this.HasSingletonImplementations = hasSingletonImplementations;
		
		this.Usings = usings;
		this.IsPartial = isPartial;
		this.ExternalDefinition = externalDefinition;
	}
	
	public static string GetName(string? customName, string name, bool isInterface, bool isProxy)
	{
		var newName = $"{(customName ?? name)}";

		if (customName is not null)
			return newName;
		
		if (newName.EndsWith("Base"))
			newName = newName.Substring(0, newName.Length - 4);

		if (isInterface && newName[0] == 'I')
			newName = newName.Substring(1);

		newName = $"{newName}{ImplementationDiscoverySourceGenerator.ImplementationsEnumNameSuffix}";
		return IsValidName.IsMatch(newName) ? newName : name;
	}
}