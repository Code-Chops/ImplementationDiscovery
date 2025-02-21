using Architect.DomainModeling;

namespace CodeChops.ImplementationDiscovery;

public interface INewable<out TSelf>
    where TSelf : INewable<TSelf>, IDomainObject, new();