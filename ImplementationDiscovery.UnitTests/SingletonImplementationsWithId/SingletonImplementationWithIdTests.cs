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
        Assert.Equal(typeof(ImplementationWithIdMock1), ImplementationWithIdMockEnum.ImplementationWithIdMock1.Value.Type);
        Assert.Equal(typeof(ImplementationWithIdMock2), ImplementationWithIdMockEnum.ImplementationWithIdMock2.Value.Type);
    }
}