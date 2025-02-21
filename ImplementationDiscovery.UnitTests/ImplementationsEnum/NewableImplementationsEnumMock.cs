namespace CodeChops.ImplementationDiscovery.UnitTests.ImplementationsEnum;

public record NewableImplementationsEnumMock : ImplementationsEnum<NewableImplementationsEnumMock, Color>
{
    public static NewableImplementationsEnumMock Blue { get; } = CreateMember(new DiscoveredObject<Color>(typeof(Blue)));
    public static NewableImplementationsEnumMock Red { get; }  = CreateMember(new DiscoveredObject<Color>(typeof(Red)));
}

public sealed record Blue : Color;
public sealed record Red : Color;
public abstract record Color;