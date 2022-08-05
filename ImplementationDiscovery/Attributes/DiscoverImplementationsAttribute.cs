namespace CodeChops.ImplementationDiscovery.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
public sealed class DiscoverImplementations : Attribute
{
    public bool GenerateTypeIdsForImplementations { get; }
    public bool HasNewableImplementations { get; }
    
    public DiscoverImplementations(bool generateTypeIdsForImplementations = false, bool hasNewableImplementations = false)
    {
        this.GenerateTypeIdsForImplementations = generateTypeIdsForImplementations;
        this.HasNewableImplementations = hasNewableImplementations;
    }
}