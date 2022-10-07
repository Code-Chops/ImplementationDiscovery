namespace CodeChops.ImplementationDiscovery.UnitTests.EmptyImplementations;

public class EmptyImplementationsTests
{
    [Fact]
    public void EnumShouldBeGenerated()
    {
        Assert.Equal(typeof(CodeChops.ImplementationDiscovery.UnitTests.EmptyImplementations.EmptyEnum), typeof(CodeChops.ImplementationDiscovery.UnitTests.EmptyImplementations.EmptyEnum));
    }
}