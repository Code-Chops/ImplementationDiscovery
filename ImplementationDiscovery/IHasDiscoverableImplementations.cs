namespace CodeChops.ImplementationDiscovery;

public interface IHasDiscoverableImplementations<out TTypeId> : IHasTypeId<TTypeId>, IHasStaticDiscoveredTypeEnum<TTypeId> 
	where TTypeId : IDiscoveredImplementations
{
}