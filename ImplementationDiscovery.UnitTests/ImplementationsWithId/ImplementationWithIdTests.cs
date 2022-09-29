using CodeChops.ImplementationDiscovery.UninitializedObjects;

namespace CodeChops.ImplementationDiscovery.UnitTests.ImplementationsWithId;

public class ImplementationWithIdTests
{
    [Fact]
    public void ImplementationsExist()
    {
        Assert.Equal(nameof(ImplementationWithIdMock1), ImplementationWithIdMockBaseEnum.ImplementationWithIdMock1.Name);
        Assert.Equal(nameof(ImplementationWithIdMock2), ImplementationWithIdMockBaseEnum.ImplementationWithIdMock2.Name);
    }

    [Fact]
    public void IdsAreCreated()
    {
        Assert.Equal(typeof(UninitializedObject<ImplementationWithIdMockBase>), ImplementationWithIdMock1.StaticImplementationEnum.GetValue().GetType());
        Assert.Equal(typeof(UninitializedObject<ImplementationWithIdMockBase>), ImplementationWithIdMock2.StaticImplementationEnum.GetValue().GetType());
    }
}