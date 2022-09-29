namespace CodeChops.ImplementationDiscovery;

public interface IHasDiscoverableImplementations<out TImplementationId> : IHasImplementationId<TImplementationId>, IHasStaticImplementationId<TImplementationId> 
	where TImplementationId : IDiscoveredImplementations
{
}