using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace CodeChops.ImplementationDiscovery;

[StructLayout(LayoutKind.Auto)] 
public readonly record struct DiscoveredObject<TBaseType> : IComparable<DiscoveredObject<TBaseType>>, IValueObject
	where TBaseType : notnull
{
	#region Comparison
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(DiscoveredObject<TBaseType> other) => this.Type == other.Type;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => String.GetHashCode(this.Type.Name, StringComparison.Ordinal);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int CompareTo(DiscoveredObject<TBaseType> other) => String.Compare(this.Type.FullName, other.Type.FullName, StringComparison.Ordinal);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator <(DiscoveredObject<TBaseType> left, DiscoveredObject<TBaseType> right) => left.CompareTo(right) < 0;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator <=(DiscoveredObject<TBaseType> left, DiscoveredObject<TBaseType> right) => left.CompareTo(right) <= 0;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator >(DiscoveredObject<TBaseType> left, DiscoveredObject<TBaseType> right) => left.CompareTo(right) > 0;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator >=(DiscoveredObject<TBaseType> left, DiscoveredObject<TBaseType> right) => left.CompareTo(right) >= 0;
	#endregion

	#region Casting
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator DiscoveredObject<TBaseType>(Type type) => new(type);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator TBaseType(DiscoveredObject<TBaseType> discoveredObject) => discoveredObject.Instance;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Type(DiscoveredObject<TBaseType> discoveredObject) => discoveredObject.Type;
	#endregion
	
	/// <summary>
	/// <para>An instance of the discovered object.</para>
	/// <para>
	/// <em>Don't mutate this instance as it will be used by other processes.</em>
	/// </para>
	/// </summary>
	public TBaseType Instance { get; }

	/// <summary>
	/// The type of the discovered object.
	/// </summary>
	public Type Type { get; }

	/// <summary>
	/// Creates a new instance of the discovered object.
	/// </summary>
	public TBaseType Create() => this._instanceCreator();
	private readonly Func<TBaseType> _instanceCreator;
	
	private static ConcurrentDictionary<Type, object> UninitializedObjectCache { get; } = new();  
	
	public DiscoveredObject(Type type)
	{
		var instanceCreator = (TBaseType)UninitializedObjectCache.GetOrAdd(type, FormatterServices.GetUninitializedObject);
		
		this.Instance = instanceCreator;
		this.Type = type;
		this._instanceCreator = () => instanceCreator;
	}

	public DiscoveredObject(Func<TBaseType> instanceCreator)
	{
		var instance = instanceCreator();
		
		this.Instance = instance;
		this.Type = instance.GetType();
		this._instanceCreator = instanceCreator;
	}
}