namespace CodeChops.MagicEnums.UnitTests.Custom;

internal partial record CustomRecordEnumMock : MagicCustomEnum<CustomRecordEnumMock, UninitializedObject<DataRecord>>
{
	public static CustomRecordEnumMock ValueA { get; } = CreateMember(new DataRecord(Text: nameof(ValueA)));
	public static CustomRecordEnumMock ValueB { get; } = CreateMember(new DataRecord(Text: nameof(ValueB)));
}

internal record DataRecord(string Text);