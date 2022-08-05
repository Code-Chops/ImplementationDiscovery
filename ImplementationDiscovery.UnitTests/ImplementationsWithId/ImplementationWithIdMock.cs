﻿using CodeChops.ImplementationDiscovery.Attributes;

namespace CodeChops.ImplementationDiscovery.UnitTests.ImplementationsWithId;

[DiscoverImplementations(generateTypeIdsForImplementations: true)]
public abstract partial class ImplementationWithIdMockBase
{
}

public partial class ImplementationWithIdMock1 : ImplementationWithIdMockBase
{
}

public partial class ImplementationWithIdMock2 : ImplementationWithIdMockBase
{
}