namespace CodeChops.ImplementationDiscovery;

public interface IHasDiscoverableImplementations<out TTypeId> : IHasTypeId<TTypeId>
	where TTypeId : IId
{
}