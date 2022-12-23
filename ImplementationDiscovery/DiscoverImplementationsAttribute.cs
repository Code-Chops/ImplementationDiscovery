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
    public bool GenerateProxies { get; }
    
    /// <param name="enumName">
    /// Provide a custom enum name.
    /// If not provided, the name of the base class or interface will be used without the leading 'I' (for interfaces) or trailing 'Base' for base classes.
    /// </param>
    /// <param name="generateImplementationIds">
    /// Default: false.
    /// <p>If true, all discovered implementations get an implementation ID property.</p>
    /// </param>
    /// <param name="hasSingletonImplementations">
    /// Default: false.
    /// <p>If true, the ID of all discovered implementations will be their implementation ID.</p>
    /// </param>
    /// <param name="generateProxies">
    /// Default: false.
    /// <p>If true, implementations are discovered across different assemblies by creating a proxy enum in the assembly of the implementation (under the namespace of the base class / interface).</p>
    /// </param>
    public DiscoverImplementations(string? enumName = null, bool generateImplementationIds = false, bool hasSingletonImplementations = false, bool generateProxies = false)
    {
        this.EnumName = enumName;
        this.GenerateImplementationIds = generateImplementationIds;
        this.HasSingletonImplementations = hasSingletonImplementations;
        this.GenerateProxies = generateProxies;
    }
}