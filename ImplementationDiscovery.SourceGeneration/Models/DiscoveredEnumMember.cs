namespace CodeChops.ImplementationDiscovery.SourceGeneration.Models;

internal record DiscoveredEnumMember : EnumMember
{
	public DiscoverabilityMode DiscoverabilityMode { get; }
	public string FilePath { get; } 
	public LinePosition LinePosition { get; }
	public bool IsPartial { get; }
	public string? Namespace { get; }
	public string Declaration { get; }
	
	public DiscoveredEnumMember(string enumIdentifier, string name, bool isPartial, string? @namespace, string declaration, string? value,
		string? comment, DiscoverabilityMode discoverabilityMode, string filePath, LinePosition linePosition)
		: base(enumIdentifier, name, value, comment)
	{
		if (discoverabilityMode == DiscoverabilityMode.None)
		{
			throw new ArgumentException($"Member {name} of enum {enumIdentifier} should be implicitly or explicitly discovered. File path: {filePath}. Line position: {linePosition}.");
		}

		this.DiscoverabilityMode = discoverabilityMode;
		this.FilePath = filePath;
		this.LinePosition = linePosition;
		this.IsPartial = isPartial;
		this.Namespace = @namespace;
		this.Declaration = declaration;
	}
}