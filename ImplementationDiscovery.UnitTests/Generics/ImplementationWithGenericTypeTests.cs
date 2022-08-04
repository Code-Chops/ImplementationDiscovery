namespace CodeChops.ImplementationDiscovery.UnitTests.Generics;

public class ImplementationWithGenericTypeTests
{
    [Fact]
    public void ClassImplementationHasCorrectMemberName()
    {
        Assert.Equal(nameof(ImplementationWithGenericTypeMock<int>),			ClassWithGenericTypeToImplement<int>.TypeIdentities.ImplementationWithGenericTypeMock.Name);
    }

    [Fact]
    public void ClassImplementationHasCorrectMemberValue()
    {
        Assert.Equal(typeof(ImplementationWithGenericTypeMock<int>),			ClassWithGenericTypeToImplement<int>.TypeIdentities.ImplementationWithGenericTypeMock.Value.Instance.GetType());
    }
	
    [Fact]
    public void ClassWithExtraGenericTypeImplementationHasCorrectType()
    {
        Assert.Equal(typeof(ImplementationWithExtraGenericTypeMock<int>),		ClassWithExtraGenericTypeToImplement<ImplementationWithExtraGenericTypeMock<int>, int>.TypeIdentities.ImplementationWithExtraGenericTypeMock.Value.Instance.GetType());
    }
}