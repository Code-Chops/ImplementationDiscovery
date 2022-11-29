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
    public bool GenerateUninitializedObjects { get; }
    
    /// <param name="enumName">Provide a custom enum name. If not provided, a friendly enum name will be generated automatically.</param>
    /// <param name="generateImplementationIds">Default: false.
    /// <p>If true, all discovered implementations get an implementation ID property.</p>
    /// </param>
    /// <param name="hasSingletonImplementations">Default: false.
    /// <p>If true, the ID of all discovered implementations will be their implementation ID.</p>
    /// </param>
    /// <param name="generateUninitializedObjects">Default: false.
    /// <p>If true, it will generate uninitialized objects (this will avoid possible circular references).</p>
    /// <p>If false, it take the following subsequent steps:</p>
    /// <list type="number">
    /// <item>Try to find the <see cref="CodeChops.DomainDrivenDesign.DomainModeling.Factories.ICreatable{TObject}.Create(Validator)"/> factory method and invoke it.</item>
    /// <item>If not found, try to find a parameterless constructor and invoke it.</item>
    /// <item>If not found, throw an InvalidOperationException.</item>
    /// </list>
    /// </param>
    public DiscoverImplementations(string? enumName = null, bool generateImplementationIds = false, bool hasSingletonImplementations = false, bool generateUninitializedObjects = true)
    {
        this.EnumName = enumName;
        this.GenerateImplementationIds = generateImplementationIds;
        this.HasSingletonImplementations = hasSingletonImplementations;
        this.GenerateUninitializedObjects = generateUninitializedObjects;
    }
}