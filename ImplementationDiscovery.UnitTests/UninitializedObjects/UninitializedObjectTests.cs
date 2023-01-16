using CodeChops.ImplementationDiscovery.Discovered;

namespace CodeChops.ImplementationDiscovery.UnitTests.UninitializedObjects;

public class DiscoveredObjectTests
{
	[Fact]
	public void DiscoveredObjects_OfSameType_AreEqual()
	{
		Assert.Equal((SimpleDiscoveredObject<object>)typeof(DiscoveredObjectTests), new SimpleDiscoveredObject<object>(typeof(DiscoveredObjectTests)));

		var test = new DiscoveredObjectTests();
		Assert.True((SimpleDiscoveredObject<object>)typeof(DiscoveredObjectTests) == new SimpleDiscoveredObject<object>(test.GetType()));
	}
}