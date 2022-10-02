namespace CodeChops.ImplementationDiscovery.UnitTests.ImplementedClasses;

public class ClassImplementationDiscoverabilityTests
{
	[Fact]
	public void ClassImplementationHasCorrectMemberName()
	{
		Assert.True(AbstractClassToImplementEnum.ClassImplementationMock.UninitializedInstance.GetType().Name	== nameof(ClassImplementationMock));
	}

	[Fact]
	public void ClassImplementationHasCorrectMemberValue()
	{
		Assert.True(AbstractClassToImplementEnum.ClassImplementationMock.UninitializedInstance.GetType()		== typeof(ClassImplementationMock));
	}
}