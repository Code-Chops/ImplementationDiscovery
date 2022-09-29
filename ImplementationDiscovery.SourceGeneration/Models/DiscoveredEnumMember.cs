namespace CodeChops.ImplementationDiscovery.SourceGeneration.Models;

internal record DiscoveredEnumMember : EnumMember
{
	public string FilePath { get; } 
	public LinePosition LinePosition { get; }
	public bool IsPartial { get; }
	public string? Namespace { get; }
	public string Declaration { get; }
	
	public DiscoveredEnumMember(string enumIdentifier, string name, bool isPartial, string? @namespace, string declaration, string? value, string filePath, LinePosition linePosition)
		: base(enumIdentifier, name, value)
	{
		this.FilePath = filePath;
		this.LinePosition = linePosition;
		this.IsPartial = isPartial;
		this.Namespace = @namespace;
		this.Declaration = declaration;
	}
}