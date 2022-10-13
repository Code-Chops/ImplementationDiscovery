namespace CodeChops.ImplementationDiscovery.UnitTests.ImplementedInterfaces;

public class InterfaceImplementationDiscoverabilityTests
{
	[Fact]
	public void InterfaceImplementationHasCorrectMemberName()
	{
		Assert.True(InterfaceToImplementEnum.InterfaceImplementationMock.GetType().Name	== nameof(InterfaceImplementationMock));
	}

	[Fact]
	public void InterfaceImplementationHasCorrectMemberValue()
	{
		Assert.True(InterfaceToImplementEnum.InterfaceImplementationMock.GetType()		== typeof(InterfaceImplementationMock));
	}
}