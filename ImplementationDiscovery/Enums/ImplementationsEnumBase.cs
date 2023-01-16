using System.Diagnostics.CodeAnalysis;
using CodeChops.MagicEnums.Core;

namespace CodeChops.ImplementationDiscovery.Enums;

/// <summary>
/// An enum with discovered members as member values.
/// </summary>
/// <typeparam name="TSelf">The type of the number enum itself. Is also equal to the type of each member.</typeparam>
/// <typeparam name="TBaseType">The base type of the implementations.</typeparam>
public abstract record ImplementationsEnumBase<TSelf, TBaseType> : MagicCustomEnum<TSelf, DiscoveredObjectBase<TBaseType>>, IImplementationsEnum<TBaseType>
	where TSelf : ImplementationsEnumBase<TSelf, TBaseType>, new()
	where TBaseType : notnull
{
	/// <inheritdoc cref="DiscoveredObjectBase{TBaseType}.Type"/>
	public Type Type => this.Value.Type;
	
	protected static string EnumName { get; } = typeof(TSelf).Name;

	protected static string GetNameWithoutBacktick(DiscoveredObjectBase<TBaseType> value)
	{
		var name = value.Type.Name;
		var index = name.IndexOf('`');
		
		return index == -1 
			? name 
			: name[..index];
	}
	
	/// <inheritdoc cref="CodeChops.MagicEnums.Core.MagicEnumCore{TSelf, TValue}.CreateMember{TMember}"/>
	protected new static TSelf CreateMember(DiscoveredObjectBase<TBaseType> value,
		Func<TSelf>? memberCreator = null,
		[CallerMemberName] string? name = null)
		=> CreateMember(
			valueCreator: () => value, 
			memberCreator: memberCreator, 
			name: name);
	
	/// <inheritdoc cref="CodeChops.MagicEnums.Core.MagicEnumCore{TSelf, TValue}.CreateMember{TMember}"/>
	protected new static TSelf CreateMember<TMember>(DiscoveredObjectBase<TBaseType> value,
		Func<TMember>? memberCreator = null,
		[CallerMemberName] string? name = null)
		where TMember : TSelf
		=> GetOrCreateMember(name, () => value, memberCreator);
	
	/// <inheritdoc cref="CodeChops.MagicEnums.Core.MagicEnumCore{TSelf, TValue}.CreateMember{TMember}"/>
	protected new static TSelf CreateMember<TMember>(
		Func<DiscoveredObjectBase<TBaseType>> valueCreator,
		Func<TMember>? memberCreator = null,
		[CallerMemberName] string? name = null)
		where TMember : TSelf
		=> GetOrCreateMember(name, valueCreator, memberCreator);

	/// <inheritdoc cref="CodeChops.MagicEnums.Core.MagicEnumCore{TSelf, TValue}.GetOrCreateMember{TValue}"/>
	public static TSelf GetOrCreateMember(DiscoveredObjectBase<TBaseType> value) 
		=> GetOrCreateMember(
			name: GetNameWithoutBacktick(value),
			valueCreator: () => value, 
			memberCreator: () => new TSelf());

	/// <inheritdoc cref="CodeChops.MagicEnums.MagicCustomEnum{TSelf, TValue}.GetOrCreateMember"/>
	protected new static TSelf GetOrCreateMember(string? name, DiscoveredObjectBase<TBaseType> value, Func<TSelf>? memberCreator = null)
		=> GetOrCreateMember(
			name: name ?? GetNameWithoutBacktick(value),
			valueCreator: () => value, 
			memberCreator: memberCreator ?? (() => new TSelf()));

	/// <inheritdoc cref="CodeChops.MagicEnums.Core.MagicEnumCore{TSelf, TValue}.GetOrCreateMember{TMember}"/>
	protected new static TSelf GetOrCreateMember<TMember>([CallerMemberName] string? name = null, Func<DiscoveredObjectBase<TBaseType>>? valueCreator = null, Func<TMember>? memberCreator = null)
		where TMember : TSelf
	{
		if (name is null)
			throw new ArgumentNullException($"Empty name provided to {nameof(GetOrCreateMember)} for enum {EnumName}.");

		return MagicCustomEnum<TSelf, DiscoveredObjectBase<TBaseType>>.GetOrCreateMember(
			name: name, 
			valueCreator ?? (() => new SimpleDiscoveredObject<TBaseType>(typeof(TBaseType))), memberCreator);
	}
	
	/// <inheritdoc cref="CodeChops.MagicEnums.Core.MagicEnumCore{TSelf, TValue}.TryGetSingleMember(string, out TSelf?)"/> 
	public new static bool TryGetSingleMember(string memberName, [NotNullWhen(true)] out TSelf? member)
		=> MagicEnumCore<TSelf, DiscoveredObjectBase<TBaseType>>.TryGetSingleMember(memberName, out member);
	
	/// <inheritdoc cref="CodeChops.MagicEnums.Core.MagicEnumCore{TSelf, TValue}.GetSingleMember(string)"/> 
	public new static TSelf GetSingleMember(string memberName)
		=> MagicEnumCore<TSelf, DiscoveredObjectBase<TBaseType>>.GetSingleMember(memberName);
	
	/// <inheritdoc cref="CodeChops.MagicEnums.Core.MagicEnumCore{TSelf, TValue}.TryGetSingleMember(TValue, out TSelf?)"/> 
	public new static bool TryGetSingleMember(DiscoveredObjectBase<TBaseType> memberValue, [NotNullWhen(true)] out TSelf? member)
		=> MagicEnumCore<TSelf, DiscoveredObjectBase<TBaseType>>.TryGetSingleMember(memberValue, out member);
	
	/// <inheritdoc cref="CodeChops.MagicEnums.Core.MagicEnumCore{TSelf, TValue}.GetSingleMember(TValue)"/> 
	public new static TSelf GetSingleMember(DiscoveredObjectBase<TBaseType> memberValue)
		=> MagicEnumCore<TSelf, DiscoveredObjectBase<TBaseType>>.GetSingleMember(memberValue);
	
	/// <inheritdoc cref="CodeChops.MagicEnums.Core.MagicEnumCore{TSelf, TValue}.TryGetMembers(TValue, out IReadOnlyCollection{TSelf}?)"/> 
	public new static bool TryGetMembers(DiscoveredObjectBase<TBaseType> memberValue, [NotNullWhen(true)] out IReadOnlyCollection<TSelf>? members)
		=> MagicEnumCore<TSelf, DiscoveredObjectBase<TBaseType>>.TryGetMembers(memberValue, out members);
	
	/// <inheritdoc cref="CodeChops.MagicEnums.Core.MagicEnumCore{TSelf, TValue}.GetMembers(TValue)"/> 
	public new static IEnumerable<TSelf> GetMembers(DiscoveredObjectBase<TBaseType> memberValue)
		=> MagicEnumCore<TSelf, DiscoveredObjectBase<TBaseType>>.GetMembers(memberValue);
}