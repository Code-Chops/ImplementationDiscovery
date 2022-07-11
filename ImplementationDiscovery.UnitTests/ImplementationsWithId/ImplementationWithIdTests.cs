using Xunit;

namespace CodeChops.ImplementationDiscovery.UnitTests.ImplementationsWithId;

public class ImplementationWithIdTests
{
    [Fact]
    public void ImplementationsExist()
    {
        Assert.Equal(nameof(ImplementationWithIdMock1), ImplementationWithIdMock1.Implementations.ImplementationWithIdMock1.Name);
        Assert.Equal(nameof(ImplementationWithIdMock2), ImplementationWithIdMock2.Implementations.ImplementationWithIdMock2.Name);
    }

    [Fact]
    public void IdsAreCreated()
    {
        Assert.Equal(nameof(ImplementationWithIdMock1), ImplementationWithIdMock1.StaticTypeId);
        Assert.Equal(nameof(ImplementationWithIdMock2), ImplementationWithIdMock2.StaticTypeId);
    }
}