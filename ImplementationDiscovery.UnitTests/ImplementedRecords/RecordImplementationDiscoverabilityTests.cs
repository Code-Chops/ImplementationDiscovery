namespace CodeChops.ImplementationDiscovery.UnitTests.ImplementedRecords;

public class RecordImplementationDiscoverabilityTests
{
	[Fact]
	public void RecordImplementationHasCorrectMemberName()
	{
		Assert.True(AbstractRecordToImplementEnum.RecordImplementationMock.Name									== nameof(RecordImplementationMock));
	}

	[Fact]
	public void RecordImplementationHasCorrectMemberValue()
	{
		Assert.True(AbstractRecordToImplementEnum.RecordImplementationMock.Value.UninitializedInstance.GetType()	== typeof(RecordImplementationMock));
	}
}