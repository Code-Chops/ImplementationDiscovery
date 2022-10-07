namespace CodeChops.ImplementationDiscovery;

public interface IImplementationsEnum<TBaseType> : IImplementationsEnum, IMagicEnum<DiscoveredObject<TBaseType>>
	where TBaseType : notnull
{
}

public interface IImplementationsEnum
{
}