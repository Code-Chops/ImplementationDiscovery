namespace CodeChops.ImplementationDiscovery;

public interface IHasDiscoverableImplementations<out TEnum> : IHasImplementationId<TEnum>, IHasStaticImplementationId<TEnum> 
	where TEnum : IDiscoveredImplementationsEnum
{
}