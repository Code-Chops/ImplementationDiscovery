namespace CodeChops.ImplementationDiscovery;

public interface IDiscoverable<TBaseType> : IHasImplementationId<TBaseType>, IHasStaticImplementationId<TBaseType> 
	where TBaseType : IDiscoverable<TBaseType>
{
}