namespace CodeChops.ImplementationDiscovery;

/// <summary>
/// Contains a static type discriminator. 
/// </summary>
public interface IHasStaticImplementationId<out TEnum>
	where TEnum : IDiscoveredImplementationsEnum
{
	public static abstract TEnum StaticImplementationId { get; }
}