namespace CodeChops.ImplementationDiscovery.SourceGeneration.Models;

internal record DiscoveredEnum(EnumDefinition Definition, ImmutableList<DiscoveredEnumMember> Members);