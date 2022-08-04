namespace CodeChops.ImplementationDiscovery.UnitTests.ImplementationsWithId;

public class ImplementationWithIdTests
{
    [Fact]
    public void ImplementationsExist()
    {
        Assert.Equal(nameof(ImplementationWithIdMock1), ImplementationWithIdMock1.TypeEnum.ImplementationWithIdMock1.Name);
        Assert.Equal(nameof(ImplementationWithIdMock2), ImplementationWithIdMock2.TypeEnum.ImplementationWithIdMock2.Name);
    }

    [Fact]
    public void IdsAreCreated()
    {
        Assert.Equal(nameof(ImplementationWithIdMock1), ImplementationWithIdMock1.StaticTypeId.Value.UninitializedInstance.TypeId.Name);
        Assert.Equal(nameof(ImplementationWithIdMock2), ImplementationWithIdMock2.StaticTypeId.Value.UninitializedInstance.TypeId.Name);
    }
}