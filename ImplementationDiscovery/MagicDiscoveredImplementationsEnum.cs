using CodeChops.ImplementationDiscovery.UninitializedObjects;
using CodeChops.MagicEnums.Core;

namespace CodeChops.ImplementationDiscovery;

/// <summary>
/// An enum with discovered members (uninitialized objects) as values.
/// </summary>
/// <typeparam name="TSelf">The type of the number enum itself. Is also equal to the type of each member.</typeparam>
/// <typeparam name="TValue">The uninitialized enum object.</typeparam>
/// <typeparam name="TBaseType">The base type of the implementations.</typeparam>
public abstract record MagicDiscoveredImplementationsEnum<TSelf, TValue, TBaseType> : MagicEnumCore<TSelf, TValue>
	where TSelf : MagicDiscoveredImplementationsEnum<TSelf, TValue, TBaseType>, new()
	where TValue : UninitializedObject<TBaseType>, IEquatable<TValue>
	where TBaseType : class
{
	/// <summary>
	/// Creates a new enum member and returns it.
	/// </summary>
	/// <param name="value">The (newable) uninitialized object.</param>
	/// <returns>The newly created member.</returns>
	/// <exception cref="InvalidOperationException">When a member with the same name already exists.</exception>
	protected static TSelf CreateMember(TValue value) 
		=> MagicEnumCore<TSelf, TValue>.CreateMember(
			valueCreator: () => value, 
			memberCreator: () => new TSelf(), 
			name: GetNameWithoutBacktick(value.UninitializedInstance.GetType().Name));
	
	/// <summary>
	/// Creates a new enum member and returns it or gets an existing member if one already exist of the same name.
	/// </summary>
	/// <param name="name">The name of the new member.</param>
	/// <param name="value">The (newable) uninitialized object.</param>
	/// <returns>The newly created member or an existing member with the same name.</returns>
	public static TSelf GetOrCreateMember(string name, TValue value) 
		=> MagicEnumCore<TSelf, TValue>.GetOrCreateMember(
			name: GetNameWithoutBacktick(name), 
			valueCreator: () => value, 
			memberCreator: () => new TSelf());
	
	/// <summary>
	/// Get an enumerable over the uninitialized objects.
	/// </summary>
	public static IEnumerable<TBaseType> GetUninitializedObjects() => GetMembers().Select(member => member.Value.UninitializedInstance);

	private static string GetNameWithoutBacktick(string name)
	{
		var index = name.IndexOf('`');
		
		return index == -1 
			? name 
			: name[..index];
	}
}