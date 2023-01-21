namespace CodeChops.ImplementationDiscovery.UnitTests.ImplementationsEnum;

public record ImplementationsEnumMock : ImplementationsEnum<ImplementationsEnumMock, Animal>
{
	public static ImplementationsEnumMock Cat { get; } = CreateMember(new DiscoveredObject<Animal>(typeof(Cat)));
	public static ImplementationsEnumMock Dog { get; } = CreateMember(new DiscoveredObject<Animal>(typeof(Dog)));
}

public sealed record Cat : Animal;
public sealed record Dog : Animal;
public abstract record Animal;