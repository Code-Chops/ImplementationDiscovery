﻿namespace CodeChops.ImplementationDiscovery;

public interface IImplementationsEnum<TBaseType> : IImplementationsEnum, IMagicEnum<DiscoveredObject<TBaseType>>
	where TBaseType : notnull
{
	static abstract IEnumerable<TBaseType> GetDiscoveredObjects();
}

public interface IImplementationsEnum
{
}