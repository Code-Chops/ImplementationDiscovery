namespace CodeChops.ImplementationDiscovery;

public interface IDiscoveredImplementationsEnum<TBaseType> : IMagicEnum<DiscoveredObject<TBaseType>>
	where TBaseType : class
{
}