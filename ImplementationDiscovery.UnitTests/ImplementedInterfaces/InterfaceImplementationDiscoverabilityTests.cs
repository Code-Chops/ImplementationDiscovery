namespace CodeChops.ImplementationDiscovery.UnitTests.ImplementedInterfaces;

public class InterfaceImplementationDiscoverabilityTests
{
	[Fact]
	public void InterfaceImplementationHasCorrectMemberName()
	{
		Assert.True(InterfaceToImplementEnum.InterfaceImplementationMock.Type.Name	== nameof(InterfaceImplementationMock));
	}

	[Fact]
	public void InterfaceImplementationHasCorrectMemberValue()
	{
		Assert.True(InterfaceToImplementEnum.InterfaceImplementationMock.Type		== typeof(InterfaceImplementationMock));
	}
}