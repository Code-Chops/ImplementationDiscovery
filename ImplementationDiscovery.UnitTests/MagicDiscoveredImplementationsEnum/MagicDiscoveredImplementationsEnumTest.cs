namespace CodeChops.ImplementationDiscovery.UnitTests.MagicDiscoveredImplementationsEnum;

public class UninitializedObjectEnumTest
{
	[Fact]
	public void ObjectEnum_ProvidedValue_IsCorrect()
	{
		Assert.Equal(typeof(Cat), UninitializedObjectEnumMock.Cat.Value.UninitializedInstance.GetType());
		Assert.Equal(typeof(Dog), UninitializedObjectEnumMock.Dog.Value.UninitializedInstance.GetType());

		Assert.Equal(typeof(Cat), UninitializedObjectWithBaseTypeEnumMock.Cat.Value.UninitializedInstance.GetType());
		Assert.Equal(typeof(Dog), UninitializedObjectWithBaseTypeEnumMock.Dog.Value.UninitializedInstance.GetType());
	}

	[Fact]
	public void ObjectEnum_Equals_IsCorrect()
	{
		Assert.Equal(UninitializedObjectEnumMock.Cat,					UninitializedObjectEnumMock.Cat);
		Assert.Equal(UninitializedObjectEnumMock.Cat.Value,				UninitializedObjectEnumMock.Cat.Value);
		Assert.Equal(UninitializedObjectEnumMock.Dog,					UninitializedObjectEnumMock.Dog);
		Assert.Equal(UninitializedObjectEnumMock.Dog.Value,				UninitializedObjectEnumMock.Dog.Value);

		Assert.Equal(UninitializedObjectWithBaseTypeEnumMock.Cat,		UninitializedObjectWithBaseTypeEnumMock.Cat);
		Assert.Equal(UninitializedObjectWithBaseTypeEnumMock.Cat.Value,	UninitializedObjectWithBaseTypeEnumMock.Cat.Value);
		Assert.Equal(UninitializedObjectWithBaseTypeEnumMock.Dog,		UninitializedObjectWithBaseTypeEnumMock.Dog);
		Assert.Equal(UninitializedObjectWithBaseTypeEnumMock.Dog.Value,	UninitializedObjectWithBaseTypeEnumMock.Dog.Value);
	}
}