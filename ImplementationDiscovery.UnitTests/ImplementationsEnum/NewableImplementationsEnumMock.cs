using CodeChops.ImplementationDiscovery.UninitializedObjects;

namespace CodeChops.ImplementationDiscovery.UnitTests.ImplementationsEnum;

public record NewableImplementationsEnumMock : ImplementationsEnum<NewableImplementationsEnumMock, NewableUninitializedObject<Color>, Color>
{
	public static NewableImplementationsEnumMock Blue { get; }	= CreateMember(NewableUninitializedObject<Color>.Create<Blue>());
	public static NewableImplementationsEnumMock Red { get; }	= CreateMember(NewableUninitializedObject<Color>.Create<Red>());
}

public sealed record Blue : Color;
public sealed record Red : Color;
public abstract record Color;