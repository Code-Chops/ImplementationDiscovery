namespace CodeChops.ImplementationDiscovery;

public interface IImplementationsEnum<TBaseType> : IImplementationsEnum, IMagicEnum<DiscoveredObject<TBaseType>>
    where TBaseType : notnull
{
    TBaseType Instance { get; }
}

public interface IImplementationsEnum
{
    Type Type { get; }
}