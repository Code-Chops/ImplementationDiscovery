using CodeChops.ImplementationDiscovery;

namespace ToBeImplementedMocks;

// ReSharper disable once UnusedTypeParameter
public abstract record TestBase<T> : TestUltimateBase<T>;

[DiscoverImplementations(generateImplementationIds: false, generateProxies: true)]
public abstract partial record TestUltimateBase<T>;

[DiscoverImplementations(generateImplementationIds: false, generateProxies: true)]
public partial interface ITest
{
    public static void GetMemberOnSourceAssembly(string name)
    {
        TestEnum.GetSingleMember(name);
    }
}

public abstract record TestBase : ITest;