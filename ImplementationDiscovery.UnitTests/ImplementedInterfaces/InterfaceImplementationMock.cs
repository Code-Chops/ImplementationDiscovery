using CodeChops.ImplementationDiscovery.Attributes;
using CodeChops.ImplementationDiscovery.UninitializedObjects;

namespace CodeChops.ImplementationDiscovery.UnitTests.ImplementedInterfaces;

[DiscoverImplementations]
public partial interface IInterfaceToImplement
{
}

public class InterfaceImplementationMock : IInterfaceToImplement
{
}

[DiscoverImplementations]
// ReSharper disable once UnusedTypeParameter
public partial interface IDayOfWeek<TDay>
	where TDay : class
{
}

public abstract record DayOfWeek<TDayOfWeek, TDay> : MagicDiscoveredImplementationsEnum<TDayOfWeek, TDay, UninitializedObject<TDay>>, IDayOfWeek<TDay>
	where TDayOfWeek : DayOfWeek<TDayOfWeek, TDay>
	where TDay : class
{
	public static TDayOfWeek CreateDay(string dayName)
	{
		var member = CreateMember(null!);
		return member;
	}
}

public record DayOfWeek<TDay> : DayOfWeek<DayOfWeek<TDay>, TDay>
	where TDay : class
{
	public static DayOfWeek<TDay> Monday { get; }   = CreateDay("Maandag");
	public static DayOfWeek<TDay> Tuesday { get; }  = CreateDay("Dinsdag");
}