using System.Reflection;
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
	public static implicit operator DiscoveredObject<TBaseType>(Type type) => new(type);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator TBaseType(DiscoveredObject<TBaseType> discoveredObject) => discoveredObject.Instance;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Type(DiscoveredObject<TBaseType> discoveredObject) => discoveredObject.Type;

	#endregion

	public Type Type { get; }
	private Func<TBaseType> InstanceCreator { get; }
	public TBaseType Instance { get; }

	public DiscoveredObject(Type type, bool generateUninitializedObjects = true)
	{
		this.Type = type;
		this.InstanceCreator = GetInstanceCreator(type, generateUninitializedObjects);
		this.Instance = this.InstanceCreator();
	}

	/// <summary>
	/// Creates a new instance by trying to access the parameterless constructor. If not possible it creates a new uninitialized object.
	/// </summary>
	private static Func<TBaseType> GetInstanceCreator(Type type, bool generateUninitializedObjects)
	{
		if (generateUninitializedObjects)
			return () => (TBaseType)FormatterServices.GetUninitializedObject(type);

		var implementsICreatable = type.GetInterfaces().Any(i => i == typeof(ICreatable<>));
		if (implementsICreatable)
		{
			var factoryMethod = type.GetMethod(nameof(ICreatable<DiscoveredObject<TBaseType>>.Create));
			
			if (factoryMethod is not null)
				return () => (TBaseType)factoryMethod.Invoke(null, Array.Empty<object>())!;
		}

		var parameterlessConstructor = type.GetConstructor(Type.EmptyTypes);
		if (parameterlessConstructor is null)
			throw new InvalidOperationException($"Could not create instance of {type.Name}: No parameterless constructor defined or ICreatable<> implemented. Creation of uninitialized objects is disable.");
				
		return () => (TBaseType)parameterlessConstructor.Invoke(Array.Empty<object>());
	}
}