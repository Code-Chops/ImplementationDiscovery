namespace CodeChops.ImplementationDiscovery.UnitTests.MagicDiscoveredImplementationsEnum;

public record UninitializedObjectWithBaseTypeEnumMock : MagicDiscoveredImplementationsEnum<UninitializedObjectWithBaseTypeEnumMock, Animal>
{
	public static UninitializedObjectWithBaseTypeEnumMock Cat { get; } = CreateMember<Cat>();
	public static UninitializedObjectWithBaseTypeEnumMock Dog { get; } = CreateMember<Dog>();

	/// <summary>
	/// Adds a null value. This should throw.
	/// </summary>
	public static void AddNullValue() => CreateMember(value: null!);
}

public record UninitializedObjectEnumMock : MagicDiscoveredImplementationsEnum<UninitializedObjectEnumMock>
{
	public static UninitializedObjectEnumMock Cat { get; } = CreateMember<Cat>();
	public static UninitializedObjectEnumMock Dog { get; } = CreateMember<Dog>();
}

public sealed record Cat : Animal;
public sealed record Dog : Animal;
public abstract record Animal;