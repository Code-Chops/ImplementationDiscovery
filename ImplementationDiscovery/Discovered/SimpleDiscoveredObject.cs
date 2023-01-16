using System.Collections.Concurrent;

namespace CodeChops.ImplementationDiscovery.Discovered;

/// <summary>
/// An object of <typeparamref name="TBaseType"/> which is discovered and created using a parameterless factory.
/// </summary>
public class SimpleDiscoveredObject<TBaseType> : DiscoveredObjectBase<TBaseType>
	where TBaseType : notnull
{
	#region Casting
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator SimpleDiscoveredObject<TBaseType>(Type type) => new(type);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator TBaseType(SimpleDiscoveredObject<TBaseType> discoveredObject) => discoveredObject.Instance;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Type(SimpleDiscoveredObject<TBaseType> discoveredObject) => discoveredObject.Type;
	#endregion
	
	/// <summary>
	/// A shared instance of the discovered object. Use with caution!
	/// </summary>
	public TBaseType Instance { get; }

	/// <summary>
	/// Creates a new instance of the discovered object.
	/// </summary>
	public TBaseType Create() => this._instanceCreator();
	private readonly Func<TBaseType> _instanceCreator;

	private static ConcurrentDictionary<Type, Func<TBaseType>> UninitializedObjectCreatorCache { get; } = new(); 
	
	/// <param name="type">The concrete type of the implementation.</param>
	/// <param name="instanceCreator">Factory to create the instance. If omitted, an uninitialized value will be used.</param>
	/// <exception cref="ArgumentException">When the type provided differs from the type of the created instance.</exception>
	public SimpleDiscoveredObject(Type type, Func<TBaseType>? instanceCreator = null)
		: base(type)
	{
		instanceCreator ??= UninitializedObjectCreatorCache.GetOrAdd(type, () => (TBaseType)FormatterServices.GetUninitializedObject(type));
		var instance = instanceCreator();

		if (type != instance.GetType())
			throw new ArgumentException($"The provided {nameof(instanceCreator)} is of type '{instance.GetType().Name}', but should be of the same type as the provided {nameof(type)} '{type.FullName}'.");
		
		this.Instance = instance;
		this._instanceCreator = instanceCreator;
	}
}