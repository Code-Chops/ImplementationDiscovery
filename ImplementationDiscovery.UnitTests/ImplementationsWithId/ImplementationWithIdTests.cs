using CodeChops.ImplementationDiscovery.Discovered;

namespace CodeChops.ImplementationDiscovery.UnitTests.ImplementationsWithId;

public class ImplementationWithIdTests
{
    [Fact]
    public void ImplementationsExist()
    {
        Assert.Equal(nameof(ImplementationWithIdMock1), ImplementationWithIdMockEnum.ImplementationWithIdMock1.Name);
        Assert.Equal(nameof(ImplementationWithIdMock2), ImplementationWithIdMockEnum.ImplementationWithIdMock2.Name);
    }

    [Fact]
    public void IdsAreCreated()
    {
        Assert.Equal(typeof(SimpleDiscoveredObject<ImplementationWithIdMockBase>), ImplementationWithIdMockEnum.ImplementationWithIdMock1.GetValue().GetType());
        Assert.Equal(typeof(SimpleDiscoveredObject<ImplementationWithIdMockBase>), ImplementationWithIdMockEnum.ImplementationWithIdMock2.GetValue().GetType());
    }
}