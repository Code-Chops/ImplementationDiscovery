namespace CodeChops.ImplementationDiscovery.UnitTests.Generics;

public class ImplementationWithGenericTypeTests
{
    [Fact]
    public void ClassImplementationHasCorrectMemberName()
    {
        Assert.Equal(nameof(ImplementationWithGenericTypeMock<int>), ClassWithGenericTypeToImplementEnum<int>.ImplementationWithGenericTypeMock.Name);
    }

    [Fact]
    public void ClassImplementationHasCorrectMemberValue()
    {
        Assert.Equal(typeof(ImplementationWithGenericTypeMock<int>), ClassWithGenericTypeToImplementEnum<int>.ImplementationWithGenericTypeMock.Type);
    }

    [Fact]
    public void ClassWithExtraGenericTypeImplementationHasCorrectType()
    {
        Assert.Equal(typeof(ImplementationWithExtraGenericTypeMock<int>), ClassWithExtraGenericTypeToImplementEnum<int, int>.ImplementationWithExtraGenericTypeMock.Type);
    }
}