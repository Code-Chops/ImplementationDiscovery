﻿namespace CodeChops.ImplementationDiscovery.UnitTests.EmptyImplementations;

public class EmptyImplementationsTests
{
    [Fact]
    public void EnumShouldBeGenerated()
    {
        Assert.Equal(typeof(EmptyImplementationsMock.TypeEnum), typeof(EmptyImplementationsMock.TypeEnum));
    }
}