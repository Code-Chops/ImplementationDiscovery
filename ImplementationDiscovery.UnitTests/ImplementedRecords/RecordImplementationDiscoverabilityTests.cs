namespace CodeChops.ImplementationDiscovery.UnitTests.ImplementedRecords;

public class RecordImplementationDiscoverabilityTests
{
	[Fact]
	public void RecordImplementationHasCorrectMemberName()
	{
		Assert.True(AbstractRecordToImplement.TypeEnum.RecordImplementationMock.Name									== nameof(RecordImplementationMock));
	}

	[Fact]
	public void RecordImplementationHasCorrectMemberValue()
	{
		Assert.True(AbstractRecordToImplement.TypeEnum.RecordImplementationMock.Value.UninitializedInstance.GetType()	== typeof(RecordImplementationMock));
	}
}