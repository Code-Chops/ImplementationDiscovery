using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using CodeChops.DomainDrivenDesign.DomainModeling.Identities;

namespace CodeChops.ImplementationDiscovery;

/// <summary>
/// An enum with discovered members (uninitialized objects) as values.
/// Use <see cref="CodeChops.MagicEnums.Core.MagicEnumCore{TSelf, TValue}.CreateMember(TValue, string)"/> to create a member.
/// </summary>
/// <typeparam name="TSelf">The type of the number enum itself. Is also equal to the type of each member.</typeparam>
public abstract record MagicDiscoveredImplementationsEnum<TSelf> : MagicDiscoveredImplementationsEnum<TSelf, object>
	where TSelf : MagicDiscoveredImplementationsEnum<TSelf>;

/// <summary>
/// An enum with discovered members (uninitialized objects) as values.
/// Use <see cref="CodeChops.MagicEnums.Core.MagicEnumCore{TSelf, TValue}.CreateMember(TValue, string)"/> to create a member.
/// </summary>
/// <typeparam name="TSelf">The type of the number enum itself. Is also equal to the type of each member.</typeparam>
/// <typeparam name="TBaseType">The base type of the implementations.</typeparam>
public abstract record MagicDiscoveredImplementationsEnum<TSelf, TBaseType> : MagicCustomEnum<TSelf, TBaseType>, IId
	where TSelf : MagicDiscoveredImplementationsEnum<TSelf, TBaseType>
	where TBaseType : notnull
{
	public object GetValue() => this.Value;
	
	public bool HasDefaultValue => this.Value.Equals(DefaultValue);
	private static readonly TBaseType DefaultValue = default!;
	
	
	protected static TSelf CreateMember<TValue>([CallerMemberName] string name = null!)
	{
		var id = CreateMember(typeof(TValue), name);
		return id;
	}

	// ReSharper disable once MemberCanBePrivate.Global
	protected static TSelf CreateMember(Type value, [CallerMemberName] string name = null!)
	{
		if (value is null) throw new ArgumentNullException(nameof(value), "A type enum cannot contain a null type.");

		var id = CreateMember(value: (TBaseType)FormatterServices.GetUninitializedObject(value), name);

		return id;
	}
}