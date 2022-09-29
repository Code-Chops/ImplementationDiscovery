namespace CodeChops.ImplementationDiscovery.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
public sealed class DiscoverImplementations : Attribute
{
    public string? EnumName { get; }
    public bool GenerateImplementationIds { get; }

    public DiscoverImplementations(string? enumName = null, bool generateImplementationIds = false)
    {
        this.EnumName = enumName;
        this.GenerateImplementationIds = generateImplementationIds;
    }
}