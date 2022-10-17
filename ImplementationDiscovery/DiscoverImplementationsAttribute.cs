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
    public bool HasSingletonImplementations { get; }
    
    /// <param name="enumName">Provide a custom enum name. If not provided, a friendly enum name will be generated automatically.</param>
    /// <param name="generateImplementationIds">If true, all discovered implementations get an implementation ID property.</param>
    /// <param name="hasSingletonImplementations">If true, the ID of all discovered implementations will be their implementation ID.</param>
    public DiscoverImplementations(string? enumName = null, bool generateImplementationIds = false, bool hasSingletonImplementations = false)
    {
        this.EnumName = enumName;
        this.GenerateImplementationIds = generateImplementationIds;
        this.HasSingletonImplementations = hasSingletonImplementations;
    }
}