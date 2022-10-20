namespace CodeChops.ImplementationDiscovery.UnitTests.CustomName;

[DiscoverImplementations(enumName: "CustomNameTest")]
public abstract partial record CustomNameMock;

public record CustomNameImplementation : CustomNameMock;