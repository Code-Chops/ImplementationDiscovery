namespace CodeChops.ImplementationDiscovery.UnitTests.SingletonImplementationsWithId;

[DiscoverImplementations(generateImplementationIds: true, hasSingletonImplementations: true)]
public abstract partial class SingletonImplementationWithIdMockBase
{
}

public partial class SingletonImplementationWithIdMock1 : SingletonImplementationWithIdMockBase
{
}

public partial class SingletonImplementationWithIdMock2 : SingletonImplementationWithIdMockBase
{
}