using System.Reflection;
using System.Runtime.InteropServices;

namespace CodeChops.ImplementationDiscovery;

[StructLayout(LayoutKind.Auto)] 
public readonly record struct DiscoveredObject<TBaseType> : IComparable<DiscoveredObject<TBaseType>>
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
	public static implicit operator TBaseType(DiscoveredObject<TBaseType> discoveredObject) => discoveredObject.UninitializedInstance;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Type(DiscoveredObject<TBaseType> discoveredObject) => discoveredObject.Type;

	#endregion

	internal TBaseType UninitializedInstance { get; }
	internal Type Type { get; }
	private ConstructorInfo? EmptyConstructor { get; }

	/// <summary>
	/// Creates a new instance by trying to access the parameterless constructor. If not possible it creates a new uninitialized object.
	/// </summary>
	public TBaseType CreateInstance() => (TBaseType)(this.EmptyConstructor?.Invoke(Array.Empty<object>()) ?? FormatterServices.GetUninitializedObject(this.Type));

	public DiscoveredObject(Type type)
	{
		this.Type = type;
		this.EmptyConstructor = this.Type.GetConstructor(Type.EmptyTypes);
		this.UninitializedInstance = this.CreateInstance();
	}
}