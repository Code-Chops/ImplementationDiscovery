namespace CodeChops.ImplementationDiscovery.UnitTests.EmptyEnum;

public class EmptyImplementationsTests
{
    [Fact]
    public void EnumShouldBeGenerated()
    {
        Assert.Equal(typeof(EmptyEnum), typeof(EmptyEnum));
    }
}