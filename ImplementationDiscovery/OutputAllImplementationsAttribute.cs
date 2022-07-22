namespace CodeChops.ImplementationDiscovery;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
public sealed class OutputAllImplementations : Attribute
{
}