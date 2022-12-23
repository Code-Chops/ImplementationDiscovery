namespace CodeChops.ImplementationDiscovery.UnitTests.Generics;

[DiscoverImplementations(generateImplementationIds: true)]
// ReSharper disable once UnusedTypeParameter
internal abstract partial record ClassWithGenericTypeToImplement<TGenericType> 
	where TGenericType : struct;

internal partial record ImplementationWithGenericTypeMock<TGenericType> : ClassWithGenericTypeToImplement<TGenericType> 
	where TGenericType : struct;

//internal partial record ImplementationWithoutGenericTypeMock : ClassWithGenericTypeToImplement<int>;

[DiscoverImplementations]
// ReSharper disable once UnusedTypeParameter
internal abstract partial record ClassWithExtraGenericTypeToImplement<TDirection, TDeltaPointNumber>
	where TDeltaPointNumber : struct, IComparable<TDeltaPointNumber>, IEquatable<TDeltaPointNumber>;

internal record ImplementationWithExtraGenericTypeMock<TDeltaPointNumber> : ClassWithExtraGenericTypeToImplement<TDeltaPointNumber, TDeltaPointNumber>
	where TDeltaPointNumber : struct, IComparable<TDeltaPointNumber>, IEquatable<TDeltaPointNumber>;