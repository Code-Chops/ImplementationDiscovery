using CodeChops.ImplementationDiscovery.Discovered;
using CodeChops.ImplementationDiscovery.Enums;

namespace CodeChops.ImplementationDiscovery.UnitTests.ImplementationsEnum;

public record ImplementationsEnumMock : ImplementationsEnumBase<ImplementationsEnumMock, Animal>
{
	public static ImplementationsEnumMock Cat { get; } = CreateMember(new SimpleDiscoveredObject<Animal>(typeof(Cat)));
	public static ImplementationsEnumMock Dog { get; } = CreateMember(new SimpleDiscoveredObject<Animal>(typeof(Dog)));
}

public sealed record Cat : Animal;
public sealed record Dog : Animal;
public abstract record Animal;