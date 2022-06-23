using Xunit;

namespace CodeChops.ImplementationDiscovery.UnitTests.Generics;

public class ImplementationWithGenericTypeTests
{
	[Fact]
	public void ClassImplementationHasCorrectMemberName()
	{
		Assert.Equal(nameof(ImplementationWithGenericTypeMock<int>),			ClassWithGenericTypeToImplement<int>.Implementations.ImplementationWithGenericTypeMock.Name);
	}

	[Fact]
	public void ClassImplementationHasCorrectMemberValue()
	{
		Assert.Equal(typeof(ImplementationWithGenericTypeMock<int>),			ClassWithGenericTypeToImplement<int>.Implementations.ImplementationWithGenericTypeMock.Value.GetType());
	}
	
	[Fact]
	public void ClassWithExtraGenericTypeImplementationHasCorrectType()
	{
		Assert.Equal(typeof(ImplementationWithExtraGenericTypeMock<int>),		ClassWithExtraGenericTypeToImplement<ImplementationWithExtraGenericTypeMock<int>, int>.Implementations.ImplementationWithExtraGenericTypeMock.Value.GetType());
	}
}