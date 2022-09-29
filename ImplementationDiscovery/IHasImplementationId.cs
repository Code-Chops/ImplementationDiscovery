namespace CodeChops.ImplementationDiscovery;

/// <summary>
/// Contains a implementation type discriminator. 
/// </summary>
public interface IHasImplementationId<out TId>
	where TId : IId
{
	TId ImplementationId { get; }
}