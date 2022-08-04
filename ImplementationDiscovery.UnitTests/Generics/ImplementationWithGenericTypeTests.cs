namespace CodeChops.ImplementationDiscovery.UnitTests.Generics;

public class ImplementationWithGenericTypeTests
{
    [Fact]
    public void ClassImplementationHasCorrectMemberName()
    {
        Assert.Equal(nameof(ImplementationWithGenericTypeMock<int>),			ClassWithGenericTypeToImplement<int>.TypeEnum.ImplementationWithGenericTypeMock.Name);
    }

    [Fact]
    public void ClassImplementationHasCorrectMemberValue()
    {
        Assert.Equal(typeof(ImplementationWithGenericTypeMock<int>),			ClassWithGenericTypeToImplement<int>.TypeEnum.ImplementationWithGenericTypeMock.Value.UninitializedInstance.GetType());
    }
	
    [Fact]
    public void ClassWithExtraGenericTypeImplementationHasCorrectType()
    {
        Assert.Equal(typeof(ImplementationWithExtraGenericTypeMock<int>),		ClassWithExtraGenericTypeToImplement<ImplementationWithExtraGenericTypeMock<int>, int>.TypeEnum.ImplementationWithExtraGenericTypeMock.Value.UninitializedInstance.GetType());
    }
}