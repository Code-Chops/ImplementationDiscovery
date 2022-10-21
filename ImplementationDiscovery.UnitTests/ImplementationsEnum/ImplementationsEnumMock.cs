namespace CodeChops.ImplementationDiscovery.UnitTests.ImplementationsEnum;

public record ImplementationsEnumMock : ImplementationsEnum<ImplementationsEnumMock, Animal>, IDiscoverable
{
	public static bool IsInitialized { get; private set; }
	
	public static ImplementationsEnumMock Cat { get; } = CreateMember(new DiscoveredObject<Animal>(typeof(Cat)));
	public static ImplementationsEnumMock Dog { get; } = CreateMember(new DiscoveredObject<Animal>(typeof(Dog)));
	
	public static void SetInitialized()
	{
		IsInitialized = true;
	}
}

public sealed record Cat : Animal;
public sealed record Dog : Animal;
public abstract record Animal;