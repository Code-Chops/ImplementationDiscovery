namespace CodeChops.ImplementationDiscovery;

/// <summary>
/// Contains an implementation enum value.
/// </summary>
public interface IHasImplementationId<TBaseType>
	where TBaseType : notnull
{
	IImplementationsEnum<TBaseType> ImplementationId { get; }
}