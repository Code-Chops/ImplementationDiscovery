namespace CodeChops.ImplementationDiscovery.UnitTests.ImplementedInterfaces;

public class InterfaceImplementationDiscoverabilityTests
{
	[Fact]
	public void InterfaceImplementationHasCorrectMemberName()
	{
		Assert.True(IInterfaceToImplement.TypeEnum.InterfaceImplementationMock.Name										== nameof(InterfaceImplementationMock));
	}

	[Fact]
	public void InterfaceImplementationHasCorrectMemberValue()
	{
		Assert.True(IInterfaceToImplement.TypeEnum.InterfaceImplementationMock.Value.UninitializedInstance.GetType()	== typeof(InterfaceImplementationMock));
	}
}