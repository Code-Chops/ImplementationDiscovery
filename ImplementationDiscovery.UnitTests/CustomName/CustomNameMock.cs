namespace CodeChops.ImplementationDiscovery.UnitTests.EmptyImplementations;

[DiscoverImplementations(enumName: "EmptyEnum")]
public abstract partial record CustomNameMock;

public record CustomNameImplementation : CustomNameMock;