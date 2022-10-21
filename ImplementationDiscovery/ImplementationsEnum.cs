using System.Diagnostics.CodeAnalysis;
using System.Reflection;
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
	public TBaseType UninitializedInstance => this.Value.UninitializedInstance;
	public Type Type => this.Value.Type;
	protected static bool IsInitialized { get; private set; }
	private static string EnumName { get; } = typeof(TSelf).Name;

	/// <summary>
	/// Creates a new enum member and returns it.
	/// </summary>
	/// <param name="value">The (newable) uninitialized object.</param>
	/// <returns>The newly created member.</returns>
	/// <exception cref="InvalidOperationException">When a member with the same name already exists.</exception>
	protected static TSelf CreateMember(DiscoveredObject<TBaseType> value) 
		=> CreateMember(
			valueCreator: () => value, 
			memberCreator: () => new TSelf(), 
			name: GetNameWithoutBacktick(value));
	
	protected new static TSelf CreateMember<TMember>(
		Func<DiscoveredObject<TBaseType>> valueCreator,
		Func<TMember>? memberCreator = null,
		[CallerMemberName] string? name = null)
		where TMember : TSelf
		=> GetOrCreateMember(name, valueCreator, memberCreator);

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
	
	protected new static TSelf GetOrCreateMember<TMember>([CallerMemberName] string? name = null, Func<DiscoveredObject<TBaseType>>? valueCreator = null, Func<TMember>? memberCreator = null)
		where TMember : TSelf
	{
		if (name is null)
			throw new InvalidOperationException($"Empty name: Unable to retrieve implementation {EnumName}.{name}.");

		if (!IsInitialized)
			return MagicEnumCore<TSelf, DiscoveredObject<TBaseType>>.GetOrCreateMember(
				name: name,
				valueCreator: valueCreator ?? (() => new DiscoveredObject<TBaseType>(typeof(TBaseType))),
				memberCreator: memberCreator);

		return TryGetSingleMember(name, out var member) 
			? member 
			: throw new InvalidOperationException($"Unable to retrieve resource {EnumName}.");
	}

	// ReSharper disable once MethodOverloadWithOptionalParameter
	protected static TSelf GetOrCreateMember([CallerMemberName] string? name = null, DiscoveredObject<TBaseType>? value = null, Func<TSelf>? memberCreator = null)
	{
		if (name is null)
			throw new InvalidOperationException($"Empty name: Unable to retrieve implementation {EnumName}.{name}.");

		if (!IsInitialized)
		{
			var properties = typeof(TSelf).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
			
			foreach (var property in properties)
				property.GetGetMethod(nonPublic: true)!.Invoke(obj: null, parameters: null);

			IsInitialized = true;
		}

		return TryGetSingleMember(name, out var member) 
			? member 
			: throw new InvalidOperationException($"Unable to retrieve resource {EnumName}.");
	}

	/// <summary>
	/// Get an enumerable over the uninitialized objects.
	/// </summary>
	public static IEnumerable<TBaseType> GetDiscoveredObjects() 
		=> GetMembers().Select(member => member.Value.UninitializedInstance);

	private static string GetNameWithoutBacktick(DiscoveredObject<TBaseType> value)
	{
		var name = value.Type.Name;
		var index = name.IndexOf('`');
		
		return index == -1 
			? name 
			: name[..index];
	}
}