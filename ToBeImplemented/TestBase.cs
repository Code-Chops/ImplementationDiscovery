using CodeChops.ImplementationDiscovery;

namespace ToBeImplemented;

// ReSharper disable once UnusedTypeParameter
public abstract record TestBase<T> : TestUltimateBase<T>;

[DiscoverImplementations(generateImplementationIds: false, generateProxies: true)]
public abstract partial record TestUltimateBase<T>;

[DiscoverImplementations(generateImplementationIds: false, generateProxies: true)]
public partial interface ITest
{
}

public abstract record TestBase : ITest;