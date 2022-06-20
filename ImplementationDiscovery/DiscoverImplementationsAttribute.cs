namespace CodeChops.ImplementationDiscovery;

//TODO
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
public sealed class DiscoverImplementations : Attribute
{
}