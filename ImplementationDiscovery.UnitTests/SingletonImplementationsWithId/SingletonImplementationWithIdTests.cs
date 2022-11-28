using CodeChops.ImplementationDiscovery.UnitTests.ImplementationsWithId;

namespace CodeChops.ImplementationDiscovery.UnitTests.SingletonImplementationsWithId;

public class ImplementationWithIdTests
{
    [Fact]
    public void ImplementationsExist()
    {
        Assert.Equal((string?)nameof(ImplementationWithIdMock1), (string?)ImplementationWithIdMockEnum.ImplementationWithIdMock1.Name);
        Assert.Equal((string?)nameof(ImplementationWithIdMock2), (string?)ImplementationWithIdMockEnum.ImplementationWithIdMock2.Name);
    }

    [Fact]
    public void IdsAreCreated()
    {
        Assert.Equal(typeof(DiscoveredObject<ImplementationWithIdMockBase>), ImplementationWithIdMockBase.ImplementationEnum.GetValue().GetType());
        Assert.Equal(typeof(DiscoveredObject<ImplementationWithIdMockBase>), ImplementationWithIdMockBase.ImplementationEnum.GetValue().GetType());
    }
}