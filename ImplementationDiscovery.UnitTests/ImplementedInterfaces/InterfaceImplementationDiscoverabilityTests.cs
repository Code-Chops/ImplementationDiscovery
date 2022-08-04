namespace CodeChops.ImplementationDiscovery.UnitTests.ImplementedInterfaces;

public class InterfaceImplementationDiscoverabilityTests
{
	[Fact]
	public void InterfaceImplementationHasCorrectMemberName()
	{
		Assert.True(IInterfaceToImplement.TypeIdentities.InterfaceImplementationMock.Name						== nameof(InterfaceImplementationMock));
	}

	[Fact]
	public void InterfaceImplementationHasCorrectMemberValue()
	{
		Assert.True(IInterfaceToImplement.TypeIdentities.InterfaceImplementationMock.Value.Instance.GetType()	== typeof(InterfaceImplementationMock));
	}
}