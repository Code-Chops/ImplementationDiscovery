namespace CodeChops.ImplementationDiscovery.UnitTests.ImplementationsEnum;

public class NewableImplementationsEnumTest
{
	[Fact]
	public void NewableImplementationsEnum_Creates_NewObject()
	{
		Assert.Equal(typeof(Blue),	NewableImplementationsEnumMock.Blue.Value.Create().GetType());
		Assert.Equal(typeof(Red),	NewableImplementationsEnumMock.Red.Value.Create().GetType());
		
		Assert.Equal(2, NewableImplementationsEnumMock.GetMemberCount());
	}
}