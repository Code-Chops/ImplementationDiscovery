namespace CodeChops.ImplementationDiscovery.UnitTests.ImplementationsEnum;

public record NewableImplementationsEnumMock : ImplementationsEnum<NewableImplementationsEnumMock, Color>, IDiscoverable
{
	public static bool IsInitialized { get; private set; }

	public static NewableImplementationsEnumMock Blue { get; }	= CreateMember(new DiscoveredObject<Color>(typeof(Blue)));
	public static NewableImplementationsEnumMock Red { get; }	= CreateMember(new DiscoveredObject<Color>(typeof(Red)));
	
	public static void SetInitialized()
	{
		IsInitialized = true;
	}
}

public sealed record Blue : Color;
public sealed record Red : Color;
public abstract record Color;