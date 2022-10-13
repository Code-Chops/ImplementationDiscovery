﻿namespace CodeChops.ImplementationDiscovery.UnitTests.ImplementationsWithId;

public class ImplementationWithIdTests
{
    [Fact]
    public void ImplementationsExist()
    {
        Assert.Equal(nameof(ImplementationWithIdMock1), ImplementationWithIdMockEnum.ImplementationWithIdMock1.GetImplementationId().Name);
        Assert.Equal(nameof(ImplementationWithIdMock2), ImplementationWithIdMockEnum.ImplementationWithIdMock2.GetImplementationId().Name);
    }

    [Fact]
    public void IdsAreCreated()
    {
        Assert.Equal(typeof(DiscoveredObject<ImplementationWithIdMockBase>), ImplementationWithIdMock1.ImplementationId.GetValue().GetType());
        Assert.Equal(typeof(DiscoveredObject<ImplementationWithIdMockBase>), ImplementationWithIdMock2.ImplementationId.GetValue().GetType());
    }
}