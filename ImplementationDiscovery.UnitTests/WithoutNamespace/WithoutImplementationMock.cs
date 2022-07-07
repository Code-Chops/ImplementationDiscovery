using CodeChops.ImplementationDiscovery;

[DiscoverImplementations]
public partial interface IInterfaceToImplementWithoutNamespace {} 

public partial record WithoutImplementationMock : IInterfaceToImplementWithoutNamespace;