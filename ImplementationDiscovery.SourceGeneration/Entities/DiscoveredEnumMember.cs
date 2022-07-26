namespace CodeChops.ImplementationDiscovery.SourceGeneration.Entities;

public record DiscoveredEnumMember : EnumMember
{
	public DiscoverabilityMode DiscoverabilityMode { get; }
	public string FilePath { get; } 
	public LinePosition LinePosition { get; }
	public bool IsPartial { get; }
	public string? Namespace { get; }
	public string Definition { get; }
	
	/// <param name="enumIdentifier">Be aware of global namespaces!</param>
	/// <param name="namespace">Be aware of global namespaces!</param>
	public DiscoveredEnumMember(string enumIdentifier, string name, bool isPartial, string? @namespace, string definition, string? value,
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
		this.Definition = definition;
	}
}