namespace CodeChops.ImplementationDiscovery.UnitTests.WithoutNamespace;

public class RecordWithoutNamespaceTests
{
	[Fact]
	public void RecordImplementationHasCorrectMemberName()
	{
		Assert.True(IInterfaceToImplementWithoutNamespace.TypeIdentities.WithoutImplementationMock.Name						== nameof(WithoutImplementationMock));
	}

	[Fact]
	public void RecordImplementationHasCorrectMemberValue()
	{
		Assert.True(IInterfaceToImplementWithoutNamespace.TypeIdentities.WithoutImplementationMock.Value.Instance.GetType()	== typeof(WithoutImplementationMock));
	}
}