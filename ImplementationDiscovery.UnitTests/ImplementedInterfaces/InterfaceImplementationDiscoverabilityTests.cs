namespace CodeChops.ImplementationDiscovery.UnitTests.ImplementedInterfaces;

public class InterfaceImplementationDiscoverabilityTests
{
	[Fact]
	public void InterfaceImplementationHasCorrectMemberName()
	{
		Assert.True(IInterfaceToImplementEnum.InterfaceImplementationMock.Name										== nameof(InterfaceImplementationMock));
	}

	[Fact]
	public void InterfaceImplementationHasCorrectMemberValue()
	{
		Assert.True(IInterfaceToImplementEnum.InterfaceImplementationMock.Value.UninitializedInstance.GetType()	== typeof(InterfaceImplementationMock));
	}
}