using CodeChops.ImplementationDiscovery.Attributes;

namespace CodeChops.ImplementationDiscovery.UnitTests.EmptyImplementations;

[DiscoverImplementations(enumName: nameof(EmptyEnum))]
public abstract partial record EmptyImplementationsMock;