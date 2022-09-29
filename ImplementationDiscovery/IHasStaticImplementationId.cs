namespace CodeChops.ImplementationDiscovery;

/// <summary>
/// Contains a static type discriminator. 
/// </summary>
public interface IHasStaticImplementationId<TBaseType>
	where TBaseType : IDiscoverable<TBaseType>
{
	public static abstract IImplementationsEnum<TBaseType> StaticImplementationId { get; }
}