using CodeChops.ImplementationDiscovery.UnitTests.ImplementationsWithId;

namespace CodeChops.ImplementationDiscovery.UnitTests.SingletonImplementationsWithId;

public class ImplementationWithIdTests
{
    [Fact]
    public void ImplementationsExist()
    {
        Assert.Equal((string?)nameof(ImplementationWithIdMock1), (string?)ImplementationWithIdMockEnum.ImplementationWithIdMock1.UninitializedInstance.GetImplementationId().Name);
        Assert.Equal((string?)nameof(ImplementationWithIdMock2), (string?)ImplementationWithIdMockEnum.ImplementationWithIdMock2.UninitializedInstance.GetImplementationId().Name);
    }

    [Fact]
    public void IdsAreCreated()
    {
        Assert.Equal(typeof(DiscoveredObject<ImplementationWithIdMockBase>), ImplementationWithIdMock1.ImplementationId.GetValue().GetType());
        Assert.Equal(typeof(DiscoveredObject<ImplementationWithIdMockBase>), ImplementationWithIdMock2.ImplementationId.GetValue().GetType());
    }
}