namespace CodeChops.ImplementationDiscovery.UnitTests.ImplementedClasses;

public class ClassImplementationDiscoverabilityTests
{
	[Fact]
	public void ClassImplementationHasCorrectMemberName()
	{
		Assert.True(AbstractClassToImplement.Implementations.ClassImplementationMock.Name				== nameof(ClassImplementationMock));
	}

	[Fact]
	public void ClassImplementationHasCorrectMemberValue()
	{
		Assert.True(AbstractClassToImplement.Implementations.ClassImplementationMock.Value.GetType()	== typeof(ClassImplementationMock));
	}
}