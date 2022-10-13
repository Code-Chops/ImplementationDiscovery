namespace CodeChops.ImplementationDiscovery.SourceGeneration.Models;

internal record DiscoveredEnumMember : EnumMember
{
	public string FilePath { get; } 
	public LinePosition LinePosition { get; }
	public bool IsPartial { get; }
	public string? Namespace { get; }
	public string Declaration { get; }
	public string? TypeParameters { get; }
	public bool IsConvertibleToConcreteType { get; }
	public string Accessibility { get; }

	public DiscoveredEnumMember(string enumIdentifier, string name, bool isPartial, string? @namespace, string declaration, string? value, string filePath, LinePosition linePosition, string? typeParameters, bool isConvertibleToConcreteType, string accessibility)
		: base(enumIdentifier, name, value)
	{
		this.FilePath = filePath;
		this.LinePosition = linePosition;
		this.IsPartial = isPartial;
		this.Namespace = @namespace;
		this.Declaration = declaration;
		this.TypeParameters = typeParameters;
		this.IsConvertibleToConcreteType = isConvertibleToConcreteType;
		this.Accessibility = accessibility;
	}

	public string GetConcreteTypeName(EnumDefinition definition) => this.TypeParameters is null && !this.IsConvertibleToConcreteType
		? $"global::{this.Namespace}.{this.GetSimpleName(definition)}"
		: $"global::{definition.Namespace}.{definition.BaseTypeName}";
}