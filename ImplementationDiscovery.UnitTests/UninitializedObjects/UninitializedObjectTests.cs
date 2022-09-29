namespace CodeChops.ImplementationDiscovery.UnitTests.UninitializedObjects;

public class DiscoveredObjectTests
{
	[Fact]
	public void DiscoveredObjects_OfSameType_AreEqual()
	{
		Assert.Equal((DiscoveredObject<object>)typeof(DiscoveredObjectTests), DiscoveredObject<object>.Create(typeof(DiscoveredObjectTests)));

		var test = new DiscoveredObjectTests();
		Assert.True((DiscoveredObject<object>)typeof(DiscoveredObjectTests) == DiscoveredObject<object>.Create(test.GetType()));
	}
}