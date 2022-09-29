namespace CodeChops.ImplementationDiscovery;

/// <summary>
/// Contains a static type discriminator. 
/// </summary>
public interface IHasStaticImplementationId<TBaseType>
	where TBaseType : class
{
	public static abstract IDiscoveredImplementationsEnum<TBaseType> StaticImplementationId { get; }
}