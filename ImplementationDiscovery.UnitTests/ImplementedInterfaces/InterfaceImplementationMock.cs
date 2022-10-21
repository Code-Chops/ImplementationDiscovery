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
	where TSelf : DayOfWeek<TSelf, TDay>, IDiscoverable, new()
	where TDay : class
{
	public static TSelf CreateDay()
	{
		var member = CreateMember(null!);
		return member;
	}
}