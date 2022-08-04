namespace CodeChops.ImplementationDiscovery.UnitTests.ImplementedRecords;

public class RecordImplementationDiscoverabilityTests
{
	[Fact]
	public void RecordImplementationHasCorrectMemberName()
	{
		Assert.True(AbstractRecordToImplement.TypeIdentities.RecordImplementationMock.Name						== nameof(RecordImplementationMock));
	}

	[Fact]
	public void RecordImplementationHasCorrectMemberValue()
	{
		Assert.True(AbstractRecordToImplement.TypeIdentities.RecordImplementationMock.Value.Instance.GetType()	== typeof(RecordImplementationMock));
	}
}