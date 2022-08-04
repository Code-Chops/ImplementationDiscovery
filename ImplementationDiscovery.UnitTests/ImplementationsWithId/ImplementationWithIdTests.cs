namespace CodeChops.ImplementationDiscovery.UnitTests.ImplementationsWithId;

public class ImplementationWithIdTests
{
    [Fact]
    public void ImplementationsExist()
    {
        Assert.Equal(nameof(ImplementationWithIdMock1), ImplementationWithIdMock1.TypeIdentities.ImplementationWithIdMock1.Name);
        Assert.Equal(nameof(ImplementationWithIdMock2), ImplementationWithIdMock2.TypeIdentities.ImplementationWithIdMock2.Name);
    }

    [Fact]
    public void IdsAreCreated()
    {
        Assert.Equal(nameof(ImplementationWithIdMock1), ImplementationWithIdMock1.StaticTypeId.Value.Instance.TypeId.Name);
        Assert.Equal(nameof(ImplementationWithIdMock2), ImplementationWithIdMock2.StaticTypeId.Value.Instance.TypeId.Name);
    }
}