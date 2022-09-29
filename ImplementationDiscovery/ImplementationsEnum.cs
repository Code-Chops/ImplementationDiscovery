using CodeChops.MagicEnums.Core;

namespace CodeChops.ImplementationDiscovery;

/// <summary>
/// An enum with discovered members (uninitialized objects) as values.
/// </summary>
/// <typeparam name="TSelf">The type of the number enum itself. Is also equal to the type of each member.</typeparam>
/// <typeparam name="TBaseType">The base type of the implementations.</typeparam>
public abstract record ImplementationsEnum<TSelf, TBaseType> : MagicEnumCore<TSelf, DiscoveredObject<TBaseType>>, IDiscoveredImplementationsEnum
	where TSelf : ImplementationsEnum<TSelf, TBaseType>, new()
	where TBaseType : class
{
	/// <summary>
	/// Creates a new enum member and returns it.
	/// </summary>
	/// <param name="value">The (newable) uninitialized object.</param>
	/// <returns>The newly created member.</returns>
	/// <exception cref="InvalidOperationException">When a member with the same name already exists.</exception>
	protected static TSelf CreateMember(DiscoveredObject<TBaseType> value) 
		=> MagicEnumCore<TSelf, DiscoveredObject<TBaseType>>.CreateMember(
			valueCreator: () => value, 
			memberCreator: () => new TSelf(), 
			name: GetNameWithoutBacktick(value));
	
	/// <summary>
	/// Creates a new enum member and returns it or gets an existing member if one already exist of the same name.
	/// </summary>
	/// <param name="value">The (newable) uninitialized object.</param>
	/// <returns>The newly created member or an existing member with the same name.</returns>
	public static TSelf GetOrCreateMember(DiscoveredObject<TBaseType> value) 
		=> MagicEnumCore<TSelf, DiscoveredObject<TBaseType>>.GetOrCreateMember(
			name: GetNameWithoutBacktick(value),
			valueCreator: () => value, 
			memberCreator: () => new TSelf());
	
	/// <summary>
	/// Get an enumerable over the uninitialized objects.
	/// </summary>
	public static IEnumerable<TBaseType> GetDiscoveredObjects() => GetMembers().Select(member => member.Value.UninitializedInstance);

	private static string GetNameWithoutBacktick(DiscoveredObject<TBaseType> value)
	{
		var name = value.UninitializedInstance.GetType().Name;
		var index = name.IndexOf('`');
		
		return index == -1 
			? name 
			: name[..index];
	}
}