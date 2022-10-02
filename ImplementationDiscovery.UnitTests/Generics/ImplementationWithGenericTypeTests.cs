namespace CodeChops.ImplementationDiscovery.UnitTests.Generics;

public class ImplementationWithGenericTypeTests
{
    [Fact]
    public void ClassImplementationHasCorrectMemberName()
    {
        Assert.Equal(nameof(ImplementationWithGenericTypeMock<int>),		ClassWithGenericTypeToImplementEnum<int>.ImplementationWithGenericTypeMock.UninitializedInstance.ImplementationId.Name);
    }

    [Fact]
    public void ClassImplementationHasCorrectMemberValue()
    {
        Assert.Equal(typeof(ImplementationWithGenericTypeMock<int>),		ClassWithGenericTypeToImplementEnum<int>.ImplementationWithGenericTypeMock.UninitializedInstance.GetType());
    }
	
    [Fact]
    public void ClassWithExtraGenericTypeImplementationHasCorrectType()
    {
        Assert.Equal(typeof(ImplementationWithExtraGenericTypeMock<int>),   ClassWithExtraGenericTypeToImplementEnum<int, int>.ImplementationWithExtraGenericTypeMock.UninitializedInstance.GetType());
    }
}