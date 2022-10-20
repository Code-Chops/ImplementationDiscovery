namespace CodeChops.ImplementationDiscovery.UnitTests.CustomName;

public class CustomNameTests
{
    [Fact]
    public void EnumShouldBeGenerated()
    {
        Assert.Equal(typeof(CustomNameTest), typeof(CustomNameTest));
    }
}