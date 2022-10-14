namespace CodeChops.ImplementationDiscovery.UnitTests.ImplementedInterfaces;

public class InterfaceImplementationDiscoverabilityTests
{
	[Fact]
	public void InterfaceImplementationHasCorrectMemberName()
	{
		Assert.True(InterfaceToImplementEnum.InterfaceImplementationMock.UninitializedInstance.GetType().Name	== nameof(InterfaceImplementationMock));
	}

	[Fact]
	public void InterfaceImplementationHasCorrectMemberValue()
	{
		Assert.True(InterfaceToImplementEnum.InterfaceImplementationMock.UninitializedInstance.GetType()		== typeof(InterfaceImplementationMock));
	}
}