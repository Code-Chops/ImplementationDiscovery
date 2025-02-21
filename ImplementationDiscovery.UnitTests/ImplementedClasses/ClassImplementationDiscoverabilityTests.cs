namespace CodeChops.ImplementationDiscovery.UnitTests.ImplementedClasses;

public class ClassImplementationDiscoverabilityTests
{
    [Fact]
    public void ClassImplementationHasCorrectMemberName()
    {
        Assert.True(AbstractClassToImplementEnum.ClassImplementationMock.Type.Name is nameof(ClassImplementationMock));
    }

    [Fact]
    public void ClassImplementationHasCorrectMemberValue()
    {
        Assert.True(AbstractClassToImplementEnum.ClassImplementationMock.Type == typeof(ClassImplementationMock));
    }
}