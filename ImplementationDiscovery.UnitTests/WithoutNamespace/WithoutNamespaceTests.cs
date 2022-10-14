namespace CodeChops.ImplementationDiscovery.UnitTests.WithoutNamespace;

public class RecordWithoutNamespaceTests
{
	[Fact]
	public void RecordImplementationHasCorrectMemberName()
	{
		Assert.True(InterfaceToImplementWithoutNamespaceEnum.WithoutImplementationMock.UninitializedInstance.GetType().Name	== nameof(WithoutImplementationMock));
	}

	[Fact]
	public void RecordImplementationHasCorrectMemberValue()
	{
		Assert.True(InterfaceToImplementWithoutNamespaceEnum.WithoutImplementationMock.UninitializedInstance.GetType()		== typeof(WithoutImplementationMock));
	}
}