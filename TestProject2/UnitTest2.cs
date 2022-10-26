using CodeChops.ImplementationDiscovery;

namespace TestProject2;

// ReSharper disable once UnusedTypeParameter
public abstract partial class UnitTest2<T> : ITest
{
}

[DiscoverImplementations(generateImplementationIds: true)]
public partial interface ITest
{
}