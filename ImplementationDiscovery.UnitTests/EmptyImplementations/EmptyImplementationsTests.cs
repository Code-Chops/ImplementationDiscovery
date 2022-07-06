using Xunit;

namespace CodeChops.ImplementationDiscovery.UnitTests;

public class EmptyImplementationsTests
{
    [Fact]
    public void EnumShouldBeGenerated()
    {
        Assert.Equal(typeof(EmptyImplementationsMock.Implementations), typeof(EmptyImplementationsMock.Implementations));
    }
}