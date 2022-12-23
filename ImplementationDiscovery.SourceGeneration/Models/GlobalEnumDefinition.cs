namespace CodeChops.ImplementationDiscovery.SourceGeneration.Models;

internal record GlobalEnumDefinition : EnumDefinition
{
	public GlobalEnumDefinition(AnalyzerConfigOptionsProvider configOptionsProvider) 
		: base(configOptionsProvider)
	{
	}
}