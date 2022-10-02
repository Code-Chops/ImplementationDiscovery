namespace CodeChops.ImplementationDiscovery.UnitTests.ImplementationsWithId;

public class ImplementationWithIdTests
{
    [Fact]
    public void ImplementationsExist()
    {
        Assert.Equal(nameof(ImplementationWithIdMock1), ImplementationWithIdMockBaseEnum.ImplementationWithIdMock1.UninitializedInstance.ImplementationId.Name);
        Assert.Equal(nameof(ImplementationWithIdMock2), ImplementationWithIdMockBaseEnum.ImplementationWithIdMock2.UninitializedInstance.ImplementationId.Name);
    }

    [Fact]
    public void IdsAreCreated()
    {
        Assert.Equal(typeof(DiscoveredObject<ImplementationWithIdMockBase>), ImplementationWithIdMock1.GetImplementationId().GetValue().GetType());
        Assert.Equal(typeof(DiscoveredObject<ImplementationWithIdMockBase>), ImplementationWithIdMock2.GetImplementationId().GetValue().GetType());
    }
}