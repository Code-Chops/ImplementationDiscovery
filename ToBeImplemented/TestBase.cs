using CodeChops.ImplementationDiscovery;

namespace ToBeImplemented;

// ReSharper disable once UnusedTypeParameter
public abstract record TestBase<T> : TestUltimateBase
{
}

[DiscoverImplementations(generateImplementationIds: true)]
public abstract partial record TestUltimateBase
{
}

[DiscoverImplementations(generateImplementationIds: true)]
public partial interface ITest
{
}
	
public abstract record TestBase : ITest;
