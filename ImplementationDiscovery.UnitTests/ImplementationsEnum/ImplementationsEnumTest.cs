namespace CodeChops.ImplementationDiscovery.UnitTests.ImplementationsEnum;

public class ImplementationsEnumTest
{
	[Fact]
	public void ImplementationsEnum_ProvidedValue_IsCorrect()
	{
		Assert.Equal(typeof(Cat), ImplementationsEnumMock.Cat.Value.UninitializedInstance.GetType());
		Assert.Equal(typeof(Dog), ImplementationsEnumMock.Dog.Value.UninitializedInstance.GetType());
	}

	[Fact]
	public void ImplementationsEnum_Equals_IsCorrect()
	{
		Assert.Equal(ImplementationsEnumMock.Cat,		ImplementationsEnumMock.Cat);
		Assert.Equal(ImplementationsEnumMock.Cat.Value,	ImplementationsEnumMock.Cat.Value);
		Assert.Equal(ImplementationsEnumMock.Dog,		ImplementationsEnumMock.Dog);
		Assert.Equal(ImplementationsEnumMock.Dog.Value,	ImplementationsEnumMock.Dog.Value);
	}
}