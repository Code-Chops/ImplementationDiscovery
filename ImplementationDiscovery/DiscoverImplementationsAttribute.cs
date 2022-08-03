namespace CodeChops.ImplementationDiscovery;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
public sealed class DiscoverImplementations : Attribute
{
    public bool GenerateIdsForImplementations { get; }

    public DiscoverImplementations(bool generateIdsForImplementations = false)
    {
        this.GenerateIdsForImplementations = generateIdsForImplementations;
    }
}