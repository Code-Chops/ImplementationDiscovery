namespace CodeChops.ImplementationDiscovery.UnitTests.ImplementationsEnum;

public record ImplementationsEnumMock : ImplementationsEnum<ImplementationsEnumMock, Animal>
{
	public static ImplementationsEnumMock Cat { get; } = CreateMember(DiscoveredObject<Animal>.Create(typeof(Cat)));
	public static ImplementationsEnumMock Dog { get; } = CreateMember(DiscoveredObject<Animal>.Create(typeof(Dog)));
}

public sealed record Cat : Animal;
public sealed record Dog : Animal;
public abstract record Animal;