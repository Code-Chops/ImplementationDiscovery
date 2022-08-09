using CodeChops.ImplementationDiscovery.UninitializedObjects;

namespace CodeChops.ImplementationDiscovery.UnitTests.UninitializedObjects;

public class UninitializedObjectTests
{
	[Fact]
	public void UninitializedObjects_OfSameType_AreEqual()
	{
		Assert.Equal((UninitializedObject<object>)typeof(UninitializedObjectTests), UninitializedObject<object>.Create(typeof(UninitializedObjectTests)));

		var test = new UninitializedObjectTests();
		Assert.True((UninitializedObject<object>)typeof(UninitializedObjectTests) == UninitializedObject<object>.Create(test.GetType()));
	}
}