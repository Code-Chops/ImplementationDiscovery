namespace CodeChops.ImplementationDiscovery;

/// <summary>
/// An enum with discovered members (uninitialized objects) as values.
/// Use <see cref="CodeChops.MagicEnums.Core.MagicEnumCore{TSelf, TValue}.CreateMember(TValue, string)"/> to create a member.
/// </summary>
/// <typeparam name="TSelf">The type of the number enum itself. Is also equal to the type of each member.</typeparam>
public abstract record MagicNewableDiscoveredImplementationsEnum<TSelf, TBaseType> : MagicDiscoveredImplementationsEnum<TSelf, TBaseType>
	where TSelf : MagicDiscoveredImplementationsEnum<TSelf, TBaseType>
	where TBaseType : class, new()
{
	public TBaseType ConstructNewObject() => new();
}