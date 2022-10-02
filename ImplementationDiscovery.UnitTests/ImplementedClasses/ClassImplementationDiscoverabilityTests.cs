namespace CodeChops.ImplementationDiscovery.UnitTests.ImplementedClasses;

public class ClassImplementationDiscoverabilityTests
{
	[Fact]
	public void ClassImplementationHasCorrectMemberName()
	{
		Assert.True(AbstractClassToImplementEnum.ClassImplementationMock.GetType().Name	== nameof(ClassImplementationMock));
	}

	[Fact]
	public void ClassImplementationHasCorrectMemberValue()
	{
		Assert.True(AbstractClassToImplementEnum.ClassImplementationMock.GetType()		== typeof(ClassImplementationMock));
	}
}