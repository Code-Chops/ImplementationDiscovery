namespace CodeChops.ImplementationDiscovery.UnitTests.ImplementationsEnum;

public record NewableImplementationsEnumMock : ImplementationsEnum<NewableImplementationsEnumMock, Color>
{
	public static NewableImplementationsEnumMock Blue { get; }	= CreateMember(DiscoveredObject<Color>.Create<Blue>());
	public static NewableImplementationsEnumMock Red { get; }	= CreateMember(DiscoveredObject<Color>.Create<Red>());
}

public sealed record Blue : Color;
public sealed record Red : Color;
public abstract record Color;