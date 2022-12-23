﻿using System.Text.RegularExpressions;

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
	public bool GenerateProxies { get; }
	public List<string> Usings { get; }
	public bool IsPartial { get; }
	public EnumDefinition? ExternalDefinition { get; }

	public bool IsProxy => this.ExternalDefinition is not null;
	
	private static Regex IsValidName { get; } = new(@"^[a-zA-Z_]\w*(\.[a-zA-Z_]\w*)*$");
	
	protected EnumDefinition(AnalyzerConfigOptionsProvider configOptionsProvider)
	{
		var baseTypeName = nameof(Object);
		var name = GetName(Constants.AllImplementationsEnumName, baseTypeName, isProxy: false);
		
		this.Name = name;
		this.TypeParameters = null;
		
		configOptionsProvider.GlobalOptions.TryGetValue("build_property.RootNamespace", out var enumNamespace);
		this.Namespace = String.IsNullOrWhiteSpace(enumNamespace) ? null : enumNamespace;

		this.EnumIdentifier = $"{(this.Namespace is null ? null : $"{this.Namespace}.")}{name}";

		this.BaseTypeNameIncludingGenerics = baseTypeName.Trim();

		this.FilePath = Constants.AllImplementationsEnumName;
		this.Accessibility = "internal";

		this.GenerateImplementationIds = false;
		this.HasSingletonImplementations = false;
		this.GenerateProxies = false;
		
		this.Usings = new List<string>();
		this.IsPartial = true;
		this.ExternalDefinition = null;
	}

	public EnumDefinition(string baseTypeNameIncludingGenerics, ITypeSymbol baseType, ITypeSymbol? externalBaseType, TypeDeclarationSyntax syntax, 
		string filePath, AttributeData attribute, EnumDefinition? externalDefinition)
	{
		var typeParameters = NameHelpers.GetGenericsParameters(baseTypeNameIncludingGenerics);
		var isProxy = externalDefinition is not null;
		var generateProxies = attribute.GetArgumentOrDefault("generateProxies", defaultValue: false);
		
		var customName = attribute.GetArgumentOrDefault("enumName", defaultValue: (string?)null);
		var name = GetName(customName, baseTypeNameIncludingGenerics, isProxy: isProxy) + typeParameters;
		
		this.Name = name;
		this.TypeParameters = typeParameters;
		
		var enumNamespace = baseType.ContainingNamespace.IsGlobalNamespace 
			? null 
			: baseType.ContainingNamespace.ToDisplayString();
		this.Namespace = String.IsNullOrWhiteSpace(enumNamespace) ? null : enumNamespace;

		this.EnumIdentifier = $"{(this.Namespace is null ? null : $"{this.Namespace}.")}{name}";

		this.BaseTypeNameIncludingGenerics = baseTypeNameIncludingGenerics.Trim();
		this.BaseTypeDeclaration = baseType.GetObjectDeclaration().Trim();
		this.BaseTypeGenericConstraints = syntax.GetClassGenericConstraints()?.Trim();
		this.BaseTypeTypeKind = (externalBaseType ?? baseType).TypeKind;
		
		this.FilePath = filePath;
		this.Accessibility = generateProxies ? "public" : "internal";

		this.GenerateImplementationIds = attribute.GetArgumentOrDefault("generateImplementationIds", defaultValue: false);
		this.HasSingletonImplementations = attribute.GetArgumentOrDefault("hasSingletonImplementations", defaultValue: false);
		this.GenerateProxies = generateProxies;
		
		this.Usings = syntax
			.GetUsings()
			.Append($"using {baseType.ContainingNamespace?.ToDisplayString() ?? "System"};")
			.Append($"using {externalBaseType?.ContainingNamespace?.ToDisplayString() ?? "System"};")
			.ToList();
		
		this.IsPartial = syntax.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword));
		this.ExternalDefinition = externalDefinition;
	}

	public static string GetName(string? customName, string name, bool isProxy)
	{
		var newName = NameHelpers.GetNameWithoutGenerics(customName ?? name);

		if (customName is null)
		{
			if (newName.EndsWith("Base"))
				newName = newName.Substring(0, newName.Length - "Base".Length);

			if (newName.Length >=2 && newName[0] == 'I' && Char.IsUpper(newName[1]) && Char.IsLower(newName[2]))
				newName = newName.Substring(1);
			
			if (!newName.EndsWith(Constants.ImplementationsEnumNameSuffix))
				newName = $"{newName}{Constants.ImplementationsEnumNameSuffix}";
		}

		if (isProxy)
		{
			if (newName.EndsWith(Constants.ImplementationsEnumNameSuffix))
				newName = newName.Substring(0, newName.Length - Constants.ImplementationsEnumNameSuffix.Length);

			newName = $"{newName}{Constants.ProxyEnumSuffix}{Constants.ImplementationsEnumNameSuffix}";
		}

		return IsValidName.IsMatch(newName) ? newName : name;
	}
}