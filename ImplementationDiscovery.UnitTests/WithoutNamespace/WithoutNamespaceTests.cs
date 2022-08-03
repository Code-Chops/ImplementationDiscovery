namespace CodeChops.ImplementationDiscovery.UnitTests.WithoutNamespace;

public class RecordWithoutNamespaceTests
{
	[Fact]
	public void RecordImplementationHasCorrectMemberName()
	{
		Assert.True(IInterfaceToImplementWithoutNamespace.Implementations.WithoutImplementationMock.Name						== nameof(WithoutImplementationMock));
	}

	[Fact]
	public void RecordImplementationHasCorrectMemberValue()
	{
		Assert.True(IInterfaceToImplementWithoutNamespace.Implementations.WithoutImplementationMock.Value.Instance.GetType()	== typeof(WithoutImplementationMock));
	}
}