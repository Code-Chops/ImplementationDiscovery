namespace CodeChops.ImplementationDiscovery.UnitTests.WithoutNamespace;

public class RecordWithoutNamespaceTests
{
	[Fact]
	public void RecordImplementationHasCorrectMemberName()
	{
		Assert.True(InterfaceToImplementWithoutNamespaceEnum.WithoutImplementationMock.GetType().Name	== nameof(WithoutImplementationMock));
	}

	[Fact]
	public void RecordImplementationHasCorrectMemberValue()
	{
		Assert.True(InterfaceToImplementWithoutNamespaceEnum.WithoutImplementationMock.GetType()		== typeof(WithoutImplementationMock));
	}
}