namespace CodeChops.ImplementationDiscovery.UnitTests.WithoutNamespace;

public class RecordWithoutNamespaceTests
{
	[Fact]
	public void RecordImplementationHasCorrectMemberName()
	{
		Assert.True(InterfaceToImplementWithoutNamespaceEnum.WithoutImplementationMock.Type.Name	== nameof(WithoutImplementationMock));
	}

	[Fact]
	public void RecordImplementationHasCorrectMemberValue()
	{
		Assert.True(InterfaceToImplementWithoutNamespaceEnum.WithoutImplementationMock.Type			== typeof(WithoutImplementationMock));
	}
}