namespace CodeChops.ImplementationDiscovery;

public interface IHasDiscoverableImplementations<TBaseType> : IHasImplementationId<TBaseType>, IHasStaticImplementationId<TBaseType> 
	where TBaseType : class
{
}