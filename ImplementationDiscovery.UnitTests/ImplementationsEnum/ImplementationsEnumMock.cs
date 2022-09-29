using CodeChops.ImplementationDiscovery.UninitializedObjects;

namespace CodeChops.ImplementationDiscovery.UnitTests.ImplementationsEnum;

public record ImplementationsEnumMock : ImplementationsEnum<ImplementationsEnumMock, UninitializedObject<Animal>, Animal>
{
	public static ImplementationsEnumMock Cat { get; } = CreateMember(UninitializedObject<Animal>.Create(typeof(Cat)));
	public static ImplementationsEnumMock Dog { get; } = CreateMember(UninitializedObject<Animal>.Create(typeof(Dog)));
}

public sealed record Cat : Animal;
public sealed record Dog : Animal;
public abstract record Animal;