namespace CodeChops.ImplementationDiscovery.UnitTests.MagicDiscoveredImplementationsEnum;

public class MagicNewableDiscoveredImplementationsEnumTest
{
	[Fact]
	public void MagicNewableDiscoveredImplementationsEnum_Creates_NewObject()
	{
		Assert.Equal(typeof(Blue),	MagicNewlyDiscoveredImplementationsEnumMock.Blue.Value.CreateNewInstance().GetType());
		Assert.Equal(typeof(Red),	MagicNewlyDiscoveredImplementationsEnumMock.Red.Value.CreateNewInstance().GetType());
	}
}