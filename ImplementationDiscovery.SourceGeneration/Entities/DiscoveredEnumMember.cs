using Microsoft.CodeAnalysis.Text;

namespace CodeChops.ImplementationDiscovery.SourceGeneration.Entities;

public record DiscoveredEnumMember : EnumMember
{
	public DiscoverabilityMode DiscoverabilityMode { get; }
	public string FilePath { get; } 
	public LinePosition LinePosition { get; }
	public string? InheritanceDefinition { get; }

	public DiscoveredEnumMember(string enumIdenifier, string name, string? value, string? comment, DiscoverabilityMode discoverabilityMode, string filePath, LinePosition linePosition, string? inheritanceDefinition = null)
		: base(enumIdenifier, name, value, comment)
	{
		if (discoverabilityMode == DiscoverabilityMode.None)
		{
			throw new ArgumentException($"Member {name} of enum {enumIdenifier} should be implicitly or explicitly discovered. File path: {filePath}. Line position: {linePosition}.");
		}

		this.DiscoverabilityMode = discoverabilityMode;
		this.FilePath = filePath;
		this.LinePosition = linePosition;
		this.InheritanceDefinition = inheritanceDefinition;
	}
}