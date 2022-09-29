namespace CodeChops.ImplementationDiscovery.UnitTests.ImplementationsEnum;

public class NewableImplementationsEnumTest
{
	[Fact]
	public void NewableImplementationsEnum_Creates_NewObject()
	{
		Assert.Equal(typeof(Blue),	NewableImplementationsEnumMock.Blue.Value.CreateNewInstance().GetType());
		Assert.Equal(typeof(Red),	NewableImplementationsEnumMock.Red.Value.CreateNewInstance().GetType());
	}
}