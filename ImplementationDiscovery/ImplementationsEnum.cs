using CodeChops.MagicEnums.Core;

namespace CodeChops.ImplementationDiscovery;

/// <summary>
/// An enum with discovered members (uninitialized objects) as values.
/// </summary>
/// <typeparam name="TSelf">The type of the number enum itself. Is also equal to the type of each member.</typeparam>
/// <typeparam name="TBaseType">The base type of the implementations.</typeparam>
public abstract record ImplementationsEnum<TSelf, TBaseType> : MagicEnumCore<TSelf, DiscoveredObject<TBaseType>>, IImplementationsEnum<TBaseType>
	where TSelf : ImplementationsEnum<TSelf, TBaseType>, new() 
	where TBaseType : notnull
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
	public IEnumerable<TBaseType> GetDiscoveredObjects() => MagicEnumCore<TSelf, DiscoveredObject<TBaseType>>.GetMembers().Select(member => member.Value.UninitializedInstance);

	private static string GetNameWithoutBacktick(DiscoveredObject<TBaseType> value)
	{
		var name = value.UninitializedInstance.GetType().Name;
		var index = name.IndexOf('`');
		
		return index == -1 
			? name 
			: name[..index];
	}
	
	
	// #region ForwardInstancesMethodsToStatic 
	//
	// /// <inheritdoc cref="MagicEnumCore{TSelf, DiscoveredObject}.GetDefaultValue"/>
	// public new DiscoveredObject<TBaseType>? GetDefaultValue() => MagicEnumCore<TSelf, DiscoveredObject<TBaseType>>.GetDefaultValue();
	//
	// /// <inheritdoc cref="MagicEnumCore{TSelf, DiscoveredObject}.GetMemberCount"/>
	// public new int GetMemberCount() => MagicEnumCore<TSelf, DiscoveredObject<TBaseType>>.GetMemberCount();
	//
	// /// <inheritdoc cref="MagicEnumCore{TSelf, DiscoveredObject}.GetUniqueValueCount"/>
	// public new int GetUniqueValueCount() => MagicEnumCore<TSelf, DiscoveredObject<TBaseType>>.GetUniqueValueCount();
	//
	// /// <inheritdoc cref="MagicEnumCore{TSelf, DiscoveredObject}.GetMembers()"/>
	// public new IEnumerable<IImplementationsEnum<TBaseType>> GetMembers() => MagicEnumCore<TSelf, DiscoveredObject<TBaseType>>.GetMembers();
	//
	// /// <inheritdoc cref="MagicEnumCore{TSelf, DiscoveredObject}.GetValues()"/>
	// public new IEnumerable<DiscoveredObject<TBaseType>> GetValues() => MagicEnumCore<TSelf, DiscoveredObject<TBaseType>>.GetValues();
	//
	// /// <inheritdoc cref="MagicEnumCore{TSelf, DiscoveredObject}.TryGetSingleMember(string, out TSelf)"/>
	// public bool TryGetSingleMember(string memberName, [NotNullWhen(true)] out IImplementationsEnum<TBaseType>? member)
	// {
	// 	if (!TryGetSingleMember(memberName, out TSelf? foundMember))
	// 	{
	// 		member = null;
	// 		return false;
	// 	}
	//
	// 	member = foundMember;
	// 	return true;
	// }
	//
	// /// <inheritdoc cref="MagicEnumCore{TSelf, DiscoveredObject}.GetSingleMember(string)"/>
	// public new IImplementationsEnum<TBaseType> GetSingleMember(string memberName) => MagicEnumCore<TSelf, DiscoveredObject<TBaseType>>.GetSingleMember(memberName);
	//
	// /// <inheritdoc cref="MagicEnumCore{TSelf, DiscoveredObject}.TryGetSingleMember(DiscoveredObject, out TSelf?)"/>
	// public bool TryGetSingleMember(DiscoveredObject<TBaseType> memberValue, [NotNullWhen(true)] out IImplementationsEnum<TBaseType>? member)
	// {
	// 	if (!TryGetSingleMember(memberValue, out TSelf? foundMember))
	// 	{
	// 		member = null;
	// 		return false;
	// 	}
	//
	// 	member = foundMember;
	// 	return true;
	// }
	//
	// /// <inheritdoc cref="MagicEnumCore{TSelf, DiscoveredObject}.GetSingleMember(DiscoveredObject)"/>
	// public new IImplementationsEnum<TBaseType> GetSingleMember(DiscoveredObject<TBaseType> memberValue) => MagicEnumCore<TSelf, DiscoveredObject<TBaseType>>.GetSingleMember(memberValue);
	//
	// /// <inheritdoc cref="MagicEnumCore{TSelf, DiscoveredObject}.TryGetMembers(DiscoveredObject, out IReadOnlyCollection{TSelf}?)"/>
	// public bool TryGetMembers(DiscoveredObject<TBaseType> memberValue, [NotNullWhen(true)] out IReadOnlyCollection<IImplementationsEnum<TBaseType>>? members)
	// {
	// 	if (!TryGetMembers(memberValue, out IReadOnlyCollection<TSelf>? foundMembers))
	// 	{
	// 		members = null;
	// 		return false;
	// 	}
	//
	// 	members = foundMembers;
	// 	return true;
	// }
	//
	// /// <inheritdoc cref="MagicEnumCore{TSelf, DiscoveredObject}.GetMembers(DiscoveredObject)"/>
	// public new IEnumerable<IImplementationsEnum<TBaseType>> GetMembers(DiscoveredObject<TBaseType> memberValue) => MagicEnumCore<TSelf, DiscoveredObject<TBaseType>>.GetMembers(memberValue);
	//
	// #endregion

}