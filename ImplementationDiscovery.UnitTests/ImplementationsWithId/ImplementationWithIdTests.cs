using CodeChops.ImplementationDiscovery.UninitializedObjects;

namespace CodeChops.ImplementationDiscovery.UnitTests.ImplementationsWithId;

public class ImplementationWithIdTests
{
    [Fact]
    public void ImplementationsExist()
    {
        Assert.Equal(nameof(ImplementationWithIdMock1), ImplementationWithIdMockBase.TypeEnum.ImplementationWithIdMock1.Name);
        Assert.Equal(nameof(ImplementationWithIdMock2), ImplementationWithIdMockBase.TypeEnum.ImplementationWithIdMock2.Name);
    }

    [Fact]
    public void IdsAreCreated()
    {
        Assert.Equal(typeof(UninitializedObject<ImplementationWithIdMockBase>), ImplementationWithIdMock1.StaticTypeId.GetValue().GetType());
        Assert.Equal(typeof(UninitializedObject<ImplementationWithIdMockBase>), ImplementationWithIdMock2.StaticTypeId.GetValue().GetType());
    }
}