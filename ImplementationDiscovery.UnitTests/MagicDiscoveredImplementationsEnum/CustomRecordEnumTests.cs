namespace CodeChops.MagicEnums.UnitTests.Custom;

public class CustomRecordEnumTests
{
	[Fact]
	public void CustomRecordEnum_HasCorrectValues()
	{
		Assert.Equal(nameof(CustomRecordEnumMock.ValueA), CustomRecordEnumMock.ValueA.Value.UninitializedInstance.Text);
		Assert.Equal(nameof(CustomRecordEnumMock.ValueB), CustomRecordEnumMock.ValueB.Value.UninitializedInstance.Text);

		Assert.Equal(nameof(CustomRecordEnumMock.ValueA), CustomRecordEnumMock.GetSingleMember(nameof(CustomRecordEnumMock.ValueA)).Value.UninitializedInstance.Text);
		Assert.Equal(nameof(CustomRecordEnumMock.ValueB), CustomRecordEnumMock.GetSingleMember(nameof(CustomRecordEnumMock.ValueB)).Value.UninitializedInstance.Text);
	}

	[Fact]
	public void CustomRecordEnum_NameCount_IsCorrect()
	{
		Assert.Equal(2, CustomRecordEnumMock.GetMemberCount());
	}

	[Fact]
	public void CustomRecordEnum_ValueCount_IsCorrect()
	{
		Assert.Equal(2, CustomRecordEnumMock.GetUniqueValueCount());
	}

	[Fact]
	public void CustomRecordEnum_Equals_IsCorrect()
	{
		Assert.Equal(CustomRecordEnumMock.ValueA,			CustomRecordEnumMock.ValueA.Value);
		Assert.NotEqual(CustomRecordEnumMock.ValueA,		CustomRecordEnumMock.ValueB);
		Assert.NotEqual(CustomRecordEnumMock.ValueA.Value,	CustomRecordEnumMock.ValueB.Value);
	}
}