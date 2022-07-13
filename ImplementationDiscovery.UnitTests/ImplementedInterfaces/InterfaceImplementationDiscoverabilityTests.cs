namespace CodeChops.ImplementationDiscovery.UnitTests.ImplementedInterfaces;

public class InterfaceImplementationDiscoverabilityTests
{
	[Fact]
	public void InterfaceImplementationHasCorrectMemberName()
	{
		Assert.True(IInterfaceToImplement.Implementations.InterfaceImplementationMock.Name				== nameof(InterfaceImplementationMock));
	}

	[Fact]
	public void InterfaceImplementationHasCorrectMemberValue()
	{
		Assert.True(IInterfaceToImplement.Implementations.InterfaceImplementationMock.Value.GetType()	== typeof(InterfaceImplementationMock));
	}
}