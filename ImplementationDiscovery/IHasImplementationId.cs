namespace CodeChops.ImplementationDiscovery;

/// <summary>
/// Contains an implementation enum value.
/// </summary>
public interface IHasImplementationId<out TImplementationsEnum>
	where TImplementationsEnum : IImplementationsEnum
{
	TImplementationsEnum GetImplementationId();
}