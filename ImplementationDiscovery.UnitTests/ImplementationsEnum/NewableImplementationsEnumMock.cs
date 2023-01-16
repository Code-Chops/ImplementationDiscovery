using CodeChops.ImplementationDiscovery.Discovered;
using CodeChops.ImplementationDiscovery.Enums;

namespace CodeChops.ImplementationDiscovery.UnitTests.ImplementationsEnum;

public record NewableImplementationsEnumMock : SimpleImplementationsEnum<NewableImplementationsEnumMock, Color>
{
	public static NewableImplementationsEnumMock Blue { get; }	= CreateMember(new SimpleDiscoveredObject<Color>(typeof(Blue)));
	public static NewableImplementationsEnumMock Red { get; }	= CreateMember(new SimpleDiscoveredObject<Color>(typeof(Red)));
}

public sealed record Blue : Color;
public sealed record Red : Color;
public abstract record Color;