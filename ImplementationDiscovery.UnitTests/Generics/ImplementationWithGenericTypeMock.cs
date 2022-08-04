namespace CodeChops.ImplementationDiscovery.UnitTests.Generics;

[DiscoverImplementations]
internal abstract partial record ClassWithGenericTypeToImplement<TGenericType>;

internal record ImplementationWithGenericTypeMock<TGenericType> : ClassWithGenericTypeToImplement<TGenericType>;

internal record ImplementationWithoutGenericTypeMock : ClassWithGenericTypeToImplement<int>;

[DiscoverImplementations]
internal abstract partial record ClassWithExtraGenericTypeToImplement<TDirection, TDeltaPointNumber>
	where TDeltaPointNumber : struct, IComparable<TDeltaPointNumber>, IEquatable<TDeltaPointNumber>, IConvertible;

internal record ImplementationWithExtraGenericTypeMock<TDeltaPointNumber> : ClassWithExtraGenericTypeToImplement<ImplementationWithExtraGenericTypeMock<TDeltaPointNumber>, TDeltaPointNumber>
	where TDeltaPointNumber : struct, IComparable<TDeltaPointNumber>, IEquatable<TDeltaPointNumber>, IConvertible;