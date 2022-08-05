using CodeChops.ImplementationDiscovery.UninitializedObjects;

namespace CodeChops.ImplementationDiscovery.UnitTests.MagicDiscoveredImplementationsEnum;

public record MagicNewlyDiscoveredImplementationsEnumMock : MagicDiscoveredImplementationsEnum<MagicNewlyDiscoveredImplementationsEnumMock, Color, NewableUninitializedObject<Color>>
{
	public static MagicNewlyDiscoveredImplementationsEnumMock Blue { get; }	= CreateMember(NewableUninitializedObject<Color>.Create<Blue>());
	public static MagicNewlyDiscoveredImplementationsEnumMock Red { get; }	= CreateMember(NewableUninitializedObject<Color>.Create<Red>());
}

public sealed record Blue : Color;
public sealed record Red : Color;
public abstract record Color;