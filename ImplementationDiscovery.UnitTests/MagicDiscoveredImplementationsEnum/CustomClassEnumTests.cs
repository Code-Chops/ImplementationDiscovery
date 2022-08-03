namespace CodeChops.MagicEnums.UnitTests.Custom;

public class CustomClassEnumTests
{
	[Fact]
	public void CustomClassEnum_HasCorrectValues()
	{
		Assert.Equal(nameof(CustomClassEnumMock.ValueA), CustomClassEnumMock.ValueA.Value.Instance.Text);
		Assert.Equal(nameof(CustomClassEnumMock.ValueB), CustomClassEnumMock.ValueB.Value.Instance.Text);

		Assert.Equal(nameof(CustomClassEnumMock.ValueA), CustomClassEnumMock.GetSingleMember(nameof(CustomClassEnumMock.ValueA)).Value.Instance.Text);
		Assert.Equal(nameof(CustomClassEnumMock.ValueB), CustomClassEnumMock.GetSingleMember(nameof(CustomClassEnumMock.ValueB)).Value.Instance.Text);
	}

	[Fact]
	public void CustomClassEnum_NameCount_IsCorrect()
	{
		Assert.Equal(2, CustomClassEnumMock.GetMemberCount());
	}

	[Fact]
	public void CustomClassEnum_ValueCount_IsCorrect()
	{
		Assert.Equal(2, CustomClassEnumMock.GetUniqueValueCount());
	}

	[Fact]
	public void CustomClassEnum_Equals_IsCorrect()
	{
		Assert.Equal(CustomClassEnumMock.ValueA,			CustomClassEnumMock.ValueA.Value);
		Assert.NotEqual(CustomClassEnumMock.ValueA,			CustomClassEnumMock.ValueB);
		Assert.NotEqual(CustomClassEnumMock.ValueA.Value,	CustomClassEnumMock.ValueB.Value);
	}
}