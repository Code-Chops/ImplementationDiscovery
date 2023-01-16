namespace CodeChops.ImplementationDiscovery.Discovered;

public interface IDiscoveredObject : IEquatable<IDiscoveredObject>, IComparable<IDiscoveredObject>, IValueObject
{
	Type Type { get; }
}