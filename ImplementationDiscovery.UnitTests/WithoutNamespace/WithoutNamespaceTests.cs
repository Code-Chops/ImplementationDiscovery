namespace CodeChops.ImplementationDiscovery.UnitTests.WithoutNamespace;

public class RecordWithoutNamespaceTests
{
	[Fact]
	public void RecordImplementationHasCorrectMemberName()
	{
		Assert.True(IInterfaceToImplementWithoutNamespaceEnum.WithoutImplementationMock.GetType().Name	== nameof(WithoutImplementationMock));
	}

	[Fact]
	public void RecordImplementationHasCorrectMemberValue()
	{
		Assert.True(IInterfaceToImplementWithoutNamespaceEnum.WithoutImplementationMock.GetType()			== typeof(WithoutImplementationMock));
	}
}