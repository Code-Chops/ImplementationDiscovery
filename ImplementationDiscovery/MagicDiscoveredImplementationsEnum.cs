using CodeChops.ImplementationDiscovery.UninitializedObjects;
using CodeChops.MagicEnums.Core;

namespace CodeChops.ImplementationDiscovery;

/// <summary>
/// An enum with discovered members (uninitialized objects) as values.
/// Use <see cref="CodeChops.MagicEnums.Core.MagicEnumCore{TSelf, TValue}.CreateMember(TValue, string)"/> to create a member.
/// </summary>
/// <typeparam name="TSelf">The type of the number enum itself. Is also equal to the type of each member.</typeparam>
/// <typeparam name="TBaseType">The base type of the implementations.</typeparam>
/// <typeparam name="TUninitializedObject">The uninitialized enum object.</typeparam>
public abstract record MagicDiscoveredImplementationsEnum<TSelf, TBaseType, TUninitializedObject> : MagicEnumCore<TSelf, TUninitializedObject>
	where TSelf : MagicDiscoveredImplementationsEnum<TSelf, TBaseType, TUninitializedObject>
	where TBaseType : class
	where TUninitializedObject : UninitializedObject<TBaseType>, IEquatable<TUninitializedObject>
{
	/// <inheritdoc cref="MagicEnumCore{TSelf,TValue}.CreateMember"/>
	public new static TSelf CreateMember(TUninitializedObject value, [CallerMemberName] string? name = null) 
		=> MagicEnumCore<TSelf, TUninitializedObject>.CreateMember(value: value, name: name!);
	
	/// <inheritdoc cref="MagicEnumCore{TSelf,TValue}.GetOrCreateMember"/>
	public new static TSelf GetOrCreateMember(TUninitializedObject value, [CallerMemberName] string? name = null) 
		=> MagicEnumCore<TSelf, TUninitializedObject>.GetOrCreateMember(value: value, name: name!);
}