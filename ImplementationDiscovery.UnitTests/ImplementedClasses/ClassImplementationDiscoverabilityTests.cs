namespace CodeChops.ImplementationDiscovery.UnitTests.ImplementedClasses;

public class ClassImplementationDiscoverabilityTests
{
	[Fact]
	public void ClassImplementationHasCorrectMemberName()
	{
		Assert.True(AbstractClassToImplement.TypeIdentities.ClassImplementationMock.Name						== nameof(ClassImplementationMock));
	}

	[Fact]
	public void ClassImplementationHasCorrectMemberValue()
	{
		Assert.True(AbstractClassToImplement.TypeIdentities.ClassImplementationMock.Value.Instance.GetType()	== typeof(ClassImplementationMock));
	}
}