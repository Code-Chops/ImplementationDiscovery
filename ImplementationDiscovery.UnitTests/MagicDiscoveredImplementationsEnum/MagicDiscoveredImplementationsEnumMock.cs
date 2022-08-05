using CodeChops.ImplementationDiscovery.UninitializedObjects;

namespace CodeChops.ImplementationDiscovery.UnitTests.MagicDiscoveredImplementationsEnum;

public record MagicDiscoveredImplementationsEnumMock : MagicDiscoveredImplementationsEnum<MagicDiscoveredImplementationsEnumMock, Animal, UninitializedObject<Animal>>
{
	public static MagicDiscoveredImplementationsEnumMock Cat { get; } = CreateMember(UninitializedObject<Animal>.Create(typeof(Cat)));
	public static MagicDiscoveredImplementationsEnumMock Dog { get; } = CreateMember(UninitializedObject<Animal>.Create(typeof(Dog)));
}

public sealed record Cat : Animal;
public sealed record Dog : Animal;
public abstract record Animal;