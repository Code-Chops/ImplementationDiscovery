namespace CodeChops.ImplementationDiscovery;

/// <summary>
/// Add this attribute to make the implementations of this class / interface discoverable at design time.
/// This has multiple extra advantages:
/// <list type="bullet">
/// <item>Implementations can be retrieved by name, dynamically at runtime.</item>
/// </list>
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
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