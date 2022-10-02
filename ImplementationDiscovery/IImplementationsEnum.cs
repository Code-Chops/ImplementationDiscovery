namespace CodeChops.ImplementationDiscovery;

public interface IImplementationsEnum<out TBaseType> : IImplementationsEnum, IMagicEnum
	where TBaseType : notnull
{
	IEnumerable<TBaseType> GetDiscoveredObjects();
}

public interface IImplementationsEnum
{
}