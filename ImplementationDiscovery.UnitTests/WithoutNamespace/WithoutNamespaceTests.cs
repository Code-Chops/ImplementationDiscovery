namespace CodeChops.ImplementationDiscovery.UnitTests.WithoutNamespace;

public class RecordWithoutNamespaceTests
{
	[Fact]
	public void RecordImplementationHasCorrectMemberName()
	{
		Assert.True(IInterfaceToImplementWithoutNamespace.TypeEnum.WithoutImplementationMock.Name									== nameof(WithoutImplementationMock));
	}

	[Fact]
	public void RecordImplementationHasCorrectMemberValue()
	{
		Assert.True(IInterfaceToImplementWithoutNamespace.TypeEnum.WithoutImplementationMock.Value.UninitializedInstance.GetType()	== typeof(WithoutImplementationMock));
	}
}