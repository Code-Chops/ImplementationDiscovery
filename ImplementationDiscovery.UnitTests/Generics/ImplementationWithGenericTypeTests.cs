using Xunit;

namespace CodeChops.ImplementationDiscovery.UnitTests;

public class ImplementationWithGenericTypeTests
{
	[Fact]
	public void ClassImplementationHasCorrectMemberName()
	{
		Assert.True(ClassWithGenericTypeToImplement<int>.Implementations.ImplementationWithGenericTypeMock.Name == nameof(ImplementationWithGenericTypeMock<int>));
	}

	[Fact]
	public void ClassImplementationHasCorrectMemberValue()
	{
		Assert.True(ClassWithGenericTypeToImplement<int>.Implementations.ImplementationWithGenericTypeMock.Value.GetType()	== typeof(ImplementationWithGenericTypeMock<int>));
	}
}