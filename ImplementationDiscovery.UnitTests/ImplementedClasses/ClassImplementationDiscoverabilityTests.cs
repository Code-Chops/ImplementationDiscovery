namespace CodeChops.ImplementationDiscovery.UnitTests.ImplementedClasses;

public class ClassImplementationDiscoverabilityTests
{
	[Fact]
	public void ClassImplementationHasCorrectMemberName()
	{
		Assert.True(AbstractClassToImplement.TypeEnum.ClassImplementationMock.Name									== nameof(ClassImplementationMock));
	}

	[Fact]
	public void ClassImplementationHasCorrectMemberValue()
	{
		Assert.True(AbstractClassToImplement.TypeEnum.ClassImplementationMock.Value.UninitializedInstance.GetType()	== typeof(ClassImplementationMock));
	}
}