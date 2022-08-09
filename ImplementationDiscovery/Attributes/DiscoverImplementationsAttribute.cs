namespace CodeChops.ImplementationDiscovery.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
public sealed class DiscoverImplementations : Attribute
{
    public string? EnumName { get; }
    public bool GenerateTypeIdsForImplementations { get; }
    public bool HasNewableImplementations { get; }
    
    public DiscoverImplementations(string? enumName = null, bool generateTypeIdsForImplementations = false, bool hasNewableImplementations = false)
    {
        this.EnumName = enumName;
        this.GenerateTypeIdsForImplementations = generateTypeIdsForImplementations;
        this.HasNewableImplementations = hasNewableImplementations;
    }
}