namespace CodeChops.ImplementationDiscovery.UnitTests.MagicDiscoveredImplementationsEnum;

public class MagicDiscoveredImplementationsEnumTest
{
	[Fact]
	public void MagicDiscoveredImplementationsEnum_ProvidedValue_IsCorrect()
	{
		Assert.Equal(typeof(Cat), MagicDiscoveredImplementationsEnumMock.Cat.Value.UninitializedInstance.GetType());
		Assert.Equal(typeof(Dog), MagicDiscoveredImplementationsEnumMock.Dog.Value.UninitializedInstance.GetType());
	}

	[Fact]
	public void MagicDiscoveredImplementationsEnum_Equals_IsCorrect()
	{
		Assert.Equal(MagicDiscoveredImplementationsEnumMock.Cat,		MagicDiscoveredImplementationsEnumMock.Cat);
		Assert.Equal(MagicDiscoveredImplementationsEnumMock.Cat.Value,	MagicDiscoveredImplementationsEnumMock.Cat.Value);
		Assert.Equal(MagicDiscoveredImplementationsEnumMock.Dog,		MagicDiscoveredImplementationsEnumMock.Dog);
		Assert.Equal(MagicDiscoveredImplementationsEnumMock.Dog.Value,	MagicDiscoveredImplementationsEnumMock.Dog.Value);
	}
}