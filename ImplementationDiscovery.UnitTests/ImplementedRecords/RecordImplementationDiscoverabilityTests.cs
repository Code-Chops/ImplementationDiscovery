namespace CodeChops.ImplementationDiscovery.UnitTests.ImplementedRecords;

public class RecordImplementationDiscoverabilityTests
{
	[Fact]
	public void RecordImplementationHasCorrectMemberName()
	{
		Assert.True(AbstractRecordToImplementEnum.RecordImplementationMock.UninitializedInstance.GetType().Name	== nameof(RecordImplementationMock));
	}

	[Fact]
	public void RecordImplementationHasCorrectMemberValue()
	{
		Assert.True(AbstractRecordToImplementEnum.RecordImplementationMock.UninitializedInstance.GetType()		== typeof(RecordImplementationMock));
	}
}