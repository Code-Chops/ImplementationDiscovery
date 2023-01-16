namespace CodeChops.ImplementationDiscovery;

public interface IImplementationsEnum<TBaseType> : IImplementationsEnum, IMagicEnum<DiscoveredObjectBase<TBaseType>>
	where TBaseType : notnull
{
}

public interface IImplementationsEnum
{
	Type Type { get; }
}