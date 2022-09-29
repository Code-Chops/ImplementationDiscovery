namespace CodeChops.ImplementationDiscovery;

public interface IHasDiscoverableImplementations<out TTypeId> : IHasImplementationEnum<TTypeId>, IHasStaticImplementationEnum<TTypeId> 
	where TTypeId : IDiscoveredImplementations
{
}