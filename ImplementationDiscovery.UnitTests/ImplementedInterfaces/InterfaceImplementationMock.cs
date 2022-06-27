using CodeChops.MagicEnums;

namespace CodeChops.ImplementationDiscovery.UnitTests.ImplementedInterfaces;

[DiscoverImplementations]
public partial interface IInterfaceToImplement
{
}

public class InterfaceImplementationMock : IInterfaceToImplement
{
}

[DiscoverImplementations]
public partial interface IDayOfWeek<TDay>
	where TDay : class
{
}

public abstract record DayOfWeek<TDayOfWeek, TDay> : MagicCustomEnum<TDayOfWeek, TDay>, IDayOfWeek<TDay>
	where TDayOfWeek : DayOfWeek<TDayOfWeek, TDay>
	where TDay : class
{
	public static TDayOfWeek CreateMember(string dayName)
	{
		var member = CreateMember(dayName);
		return member;
	}
}

public record DayOfWeek<TDay> : DayOfWeek<DayOfWeek<TDay>, TDay>
	where TDay : class
{
	public static DayOfWeek<TDay> Monday { get; }   = CreateMember("Maandag");
	public static DayOfWeek<TDay> Tuesday { get; }  = CreateMember("Dinsdag");
}