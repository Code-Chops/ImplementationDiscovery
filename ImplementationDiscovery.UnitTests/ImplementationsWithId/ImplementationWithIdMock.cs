namespace CodeChops.ImplementationDiscovery.UnitTests.ImplementationsWithId;

[DiscoverImplementations(generateIdsForImplementations: true)]
public abstract partial class ImplementationWithIdMockBase
{
}

public partial class ImplementationWithIdMock1 : ImplementationWithIdMockBase
{
}

public partial class ImplementationWithIdMock2 : ImplementationWithIdMockBase
{
}