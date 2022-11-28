namespace CodeChops.ImplementationDiscovery;

/// <summary>
/// Contains a static type discriminator. 
/// </summary>
public interface IHasStaticImplementationId<TBaseType>
	where TBaseType : notnull
{
	public static abstract IImplementationsEnum<TBaseType> ImplementationId { get; }
}