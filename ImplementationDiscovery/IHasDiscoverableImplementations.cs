using CodeChops.DomainDrivenDesign.DomainModeling.Identities;

namespace CodeChops.ImplementationDiscovery;

public interface IHasDiscoverableImplementations<out TTypeId> : IHasTypeId<TTypeId> 
	where TTypeId : Id
{
}