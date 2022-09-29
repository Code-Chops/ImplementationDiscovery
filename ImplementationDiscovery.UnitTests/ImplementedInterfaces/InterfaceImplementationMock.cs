using CodeChops.ImplementationDiscovery.Attributes;

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

public abstract record DayOfWeek<TSelf, TDay> : ImplementationsEnum<TSelf, IDayOfWeek<TDay>>
	where TSelf : DayOfWeek<TSelf, TDay>, new()
	where TDay : class
{
	public static TSelf CreateDay()
	{
		var member = CreateMember(null!);
		return member;
	}
}

public record DayOfWeek<TDay> : DayOfWeek<DayOfWeek<TDay>, TDay>
	where TDay : class
{
	public static DayOfWeek<TDay> Monday { get; }   = CreateDay();
	public static DayOfWeek<TDay> Tuesday { get; }  = CreateDay();
}