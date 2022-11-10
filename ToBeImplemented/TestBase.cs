using CodeChops.ImplementationDiscovery;

namespace ToBeImplemented;

// ReSharper disable once UnusedTypeParameter
public abstract partial class TestBase<T> : ITest
{
}

[DiscoverImplementations(generateImplementationIds: true)]
public partial interface ITest
{
}