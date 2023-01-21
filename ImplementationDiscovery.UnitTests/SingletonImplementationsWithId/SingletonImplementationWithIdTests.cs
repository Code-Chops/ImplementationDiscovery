using CodeChops.ImplementationDiscovery.UnitTests.ImplementationsWithId;

namespace CodeChops.ImplementationDiscovery.UnitTests.SingletonImplementationsWithId;

public class ImplementationWithIdTests
{
	[Fact]
	public void ImplementationsExist()
	{
		Assert.Equal((string?)nameof(ImplementationWithIdMock1), (string?)ImplementationWithIdMockEnum.ImplementationWithIdMock1.Name);
		Assert.Equal((string?)nameof(ImplementationWithIdMock2), (string?)ImplementationWithIdMockEnum.ImplementationWithIdMock2.Name);
	}

	[Fact]
	public void IdsAreCreated()
	{
		Assert.Equal(typeof(DiscoveredObject<ImplementationWithIdMockBase>), ImplementationWithIdMockEnum.ImplementationWithIdMock1.GetValue().GetType());
		Assert.Equal(typeof(DiscoveredObject<ImplementationWithIdMockBase>), ImplementationWithIdMockEnum.ImplementationWithIdMock2.GetValue().GetType());
	}
}