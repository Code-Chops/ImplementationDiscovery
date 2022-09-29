namespace CodeChops.ImplementationDiscovery;

/// <summary>
/// Contains a static type discriminator. 
/// </summary>
public interface IHasStaticImplementationId<out TImplementationId>
	where TImplementationId : IId
{
	public static abstract TImplementationId StaticImplementationId { get; }
}