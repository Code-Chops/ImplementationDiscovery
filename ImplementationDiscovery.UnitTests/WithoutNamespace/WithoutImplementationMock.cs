using CodeChops.ImplementationDiscovery.Attributes;

namespace CodeChops.ImplementationDiscovery.UnitTests.WithoutNamespace;

[DiscoverImplementations]
public partial interface IInterfaceToImplementWithoutNamespace { } 

public partial record WithoutImplementationMock : IInterfaceToImplementWithoutNamespace;