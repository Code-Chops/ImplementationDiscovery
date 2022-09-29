namespace CodeChops.ImplementationDiscovery;

/// <summary>
/// Contains a implementation type discriminator. 
/// </summary>
public interface IHasImplementationId<out TEnum>
	where TEnum : IDiscoveredImplementationsEnum
{
	TEnum ImplementationId { get; }
}