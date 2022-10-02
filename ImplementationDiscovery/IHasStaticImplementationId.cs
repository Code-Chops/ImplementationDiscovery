namespace CodeChops.ImplementationDiscovery;

/// <summary>
/// Contains a static type discriminator. 
/// </summary>
public interface IHasStaticImplementationId<out TImplementationsEnum>
	where TImplementationsEnum : IImplementationsEnum
{
	public static abstract TImplementationsEnum GetImplementationId();
}