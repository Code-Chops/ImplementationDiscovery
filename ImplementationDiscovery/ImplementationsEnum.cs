using System.Diagnostics.CodeAnalysis;
using CodeChops.MagicEnums.Core;

namespace CodeChops.ImplementationDiscovery;

/// <summary>
/// An enum with discovered members as member values.
/// </summary>
/// <typeparam name="TSelf">The type of the number enum itself. Is also equal to the type of each member.</typeparam>
/// <typeparam name="TBaseType">The base type of the implementations.</typeparam>
public abstract record ImplementationsEnum<TSelf, TBaseType> : MagicCustomEnum<TSelf, DiscoveredObject<TBaseType>>, IImplementationsEnum<TBaseType>
	where TSelf : ImplementationsEnum<TSelf, TBaseType>, new()
	where TBaseType : notnull
{
	/// <inheritdoc cref="DiscoveredObject{TBaseType}.Type"/>
	public TBaseType Instance => this.Value.Instance;
	
	/// <inheritdoc cref="DiscoveredObject{TBaseType}.Instance"/>
	public Type Type => this.Value.Type;
	
	/// <inheritdoc cref="DiscoveredObject{TBaseType}.Create"/>
	public TBaseType Create() => this.Value.Create();
	
	protected static string EnumName { get; } = typeof(TSelf).Name;

	/// <summary>
	/// Get an enumerable over the instances.
	/// </summary>
	public static IEnumerable<TBaseType> GetInstances() 
		=> GetMembers().Select(member => member.Value.Instance);

	protected static string GetNameWithoutBacktick(DiscoveredObject<TBaseType> value)
	{
		var name = value.Type.Name;
		var index = name.IndexOf('`');
		
		return index == -1 
			? name 
			: name[..index];
	}
	
	/// <inheritdoc cref="CodeChops.MagicEnums.MagicCustomEnum{TSelf, TValue}.CreateMember"/>
	protected new static TSelf CreateMember(
		DiscoveredObject<TBaseType> value,
		Func<TSelf>? memberCreator = null,
		[CallerMemberName] string? name = null)
		=> CreateMember(
			valueCreator: () => value, 
			memberCreator: memberCreator, 
			name: name);
	
	/// <inheritdoc cref="CodeChops.MagicEnums.MagicCustomEnum{TSelf, TValue}.CreateMember{TMember}"/>
	protected new static TSelf CreateMember<TMember>(
		DiscoveredObject<TBaseType> value,
		Func<TMember>? memberCreator = null,
		[CallerMemberName] string? name = null)
		where TMember : TSelf
		=> GetOrCreateMember(name, () => value, memberCreator);
	
	/// <inheritdoc cref="CodeChops.MagicEnums.Core.MagicEnumCore{TSelf, TValue}.CreateMember{TMember}"/>
	protected new static TSelf CreateMember<TMember>(
		Func<DiscoveredObject<TBaseType>> valueCreator,
		Func<TMember>? memberCreator = null,
		[CallerMemberName] string? name = null)
		where TMember : TSelf
		=> GetOrCreateMember(name, valueCreator, memberCreator);

	/// <inheritdoc cref="CodeChops.MagicEnums.Core.MagicEnumCore{TSelf, TValue}.GetOrCreateMember{TValue}"/>
	public static TSelf GetOrCreateMember(DiscoveredObject<TBaseType> value) 
		=> GetOrCreateMember(
			name: GetNameWithoutBacktick(value),
			valueCreator: () => value, 
			memberCreator: () => new TSelf());

	/// <inheritdoc cref="CodeChops.MagicEnums.MagicCustomEnum{TSelf, TValue}.GetOrCreateMember"/>
	protected new static TSelf GetOrCreateMember(string? name, DiscoveredObject<TBaseType> value, Func<TSelf>? memberCreator = null)
		=> GetOrCreateMember(
			name: name ?? GetNameWithoutBacktick(value),
			valueCreator: () => value, 
			memberCreator: memberCreator ?? (() => new TSelf()));

	/// <inheritdoc cref="CodeChops.MagicEnums.Core.MagicEnumCore{TSelf, TValue}.GetOrCreateMember{TMember}"/>
	protected new static TSelf GetOrCreateMember<TMember>([CallerMemberName] string? name = null, Func<DiscoveredObject<TBaseType>>? valueCreator = null, Func<TMember>? memberCreator = null)
		where TMember : TSelf
	{
		if (name is null)
			throw new ArgumentNullException($"Empty name provided to {nameof(GetOrCreateMember)} for enum {EnumName}.");

		return MagicCustomEnum<TSelf, DiscoveredObject<TBaseType>>.GetOrCreateMember(name, valueCreator ?? (() => new DiscoveredObject<TBaseType>(typeof(TBaseType))), memberCreator);
	}
	
	/// <inheritdoc cref="CodeChops.MagicEnums.Core.MagicEnumCore{TSelf, TValue}.TryGetSingleMember(string, out DiscoveredObject{TBaseType})"/> 
	public new static bool TryGetSingleMember(string memberName, [NotNullWhen(true)] out TSelf? member)
		=> MagicEnumCore<TSelf, DiscoveredObject<TBaseType>>.TryGetSingleMember(memberName, out member);
	
	/// <inheritdoc cref="CodeChops.MagicEnums.Core.MagicEnumCore{TSelf, TValue}.GetSingleMember(string)"/> 
	public new static TSelf GetSingleMember(string memberName)
		=> MagicEnumCore<TSelf, DiscoveredObject<TBaseType>>.GetSingleMember(memberName);
	
	/// <inheritdoc cref="CodeChops.MagicEnums.Core.MagicEnumCore{TSelf, TValue}.TryGetSingleMember(DiscoveredObject{TBaseType}, out TSelf?)"/> 
	public new static bool TryGetSingleMember(DiscoveredObject<TBaseType> memberValue, [NotNullWhen(true)] out TSelf? member)
		=> MagicEnumCore<TSelf, DiscoveredObject<TBaseType>>.TryGetSingleMember(memberValue, out member);
	
	/// <inheritdoc cref="CodeChops.MagicEnums.Core.MagicEnumCore{TSelf, TValue}.GetSingleMember(DiscoveredObject{TBaseType})"/> 
	public new static TSelf GetSingleMember(DiscoveredObject<TBaseType> memberValue)
		=> MagicEnumCore<TSelf, DiscoveredObject<TBaseType>>.GetSingleMember(memberValue);
	
	/// <inheritdoc cref="CodeChops.MagicEnums.Core.MagicEnumCore{TSelf, TValue}.TryGetMembers(DiscoveredObject{TBaseType}, out IReadOnlyCollection{TSelf}?)"/> 
	public new static bool TryGetMembers(DiscoveredObject<TBaseType> memberValue, [NotNullWhen(true)] out IReadOnlyCollection<TSelf>? members)
		=> MagicEnumCore<TSelf, DiscoveredObject<TBaseType>>.TryGetMembers(memberValue, out members);
	
	/// <inheritdoc cref="CodeChops.MagicEnums.Core.MagicEnumCore{TSelf, TValue}.GetMembers(DiscoveredObject{TBaseType})"/> 
	public new static IEnumerable<TSelf> GetMembers(DiscoveredObject<TBaseType> memberValue)
		=> MagicEnumCore<TSelf, DiscoveredObject<TBaseType>>.GetMembers(memberValue);
}