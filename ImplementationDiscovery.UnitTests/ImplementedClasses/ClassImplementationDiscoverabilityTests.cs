namespace CodeChops.ImplementationDiscovery.UnitTests.ImplementedClasses;

public class ClassImplementationDiscoverabilityTests
{
	[Fact]
	public void ClassImplementationHasCorrectMemberName()
	{
		Assert.True(AbstractClassToImplementEnum.ClassImplementationMock.Name									== nameof(ClassImplementationMock));
	}

	[Fact]
	public void ClassImplementationHasCorrectMemberValue()
	{
		Assert.True(AbstractClassToImplementEnum.ClassImplementationMock.Value.UninitializedInstance.GetType()	== typeof(ClassImplementationMock));
	}
}