namespace CodeChops.ImplementationDiscovery.UnitTests.CreationMethods;

public class CreationTests
{
	[Fact]
	public void ImplementationWithoutFactoryOrConstructor_ShouldBeUninitialized()
	{
		Assert.Null(CreationMockEnum.UninitializedCreationMock.Value.Instance.Value);
	}

	[Fact]
	public void NewableImplementation_ShouldBeInitializedCorrectly()
	{
		Assert.Equal("ValueSetInConstructor", CreationMockEnum.NewableCreationMock.Value.Instance.Value);
	}
	
	[Fact]
	public void ImplementationWithFactory_ShouldBeInitializedCorrectly()
	{
		Assert.Equal("ValueSetInFactory", CreationMockEnum.CreatableCreationMock.Value.Instance.Value);
	}
}