namespace CodeChops.ImplementationDiscovery.UnitTests;

[DiscoverImplementations]
public abstract partial class ClassWithGenericTypeToImplement<TGenericType>
{
}

internal class ImplementationWithGenericTypeMock<TGenericType> : ClassWithGenericTypeToImplement<TGenericType>
{
}

internal class ImplementationWithoutGenericTypeMock : ClassWithGenericTypeToImplement<int>
{
}