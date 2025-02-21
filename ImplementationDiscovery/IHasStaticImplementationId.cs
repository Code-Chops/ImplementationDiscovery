namespace CodeChops.ImplementationDiscovery;

/// <summary>
/// Contains a static implementation enum value.
/// </summary>
public interface IHasStaticImplementationId<TBaseType>
    where TBaseType : notnull
{
    public static abstract IImplementationsEnum<TBaseType> ImplementationId { get; }
}