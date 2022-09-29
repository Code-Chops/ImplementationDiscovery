namespace CodeChops.ImplementationDiscovery;

/// <summary>
/// Contains a implementation type discriminator. 
/// </summary>
public interface IHasImplementationId<TBaseType>
	where TBaseType : class
{
	IDiscoveredImplementationsEnum<TBaseType> ImplementationId { get; }
}