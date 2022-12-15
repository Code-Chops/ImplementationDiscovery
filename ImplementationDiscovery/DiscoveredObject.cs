using System.Collections.Concurrent;
using System.Diagnostics;
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
	public static implicit operator TBaseType(DiscoveredObject<TBaseType> discoveredObject) => discoveredObject.UninitializedInstance;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Type(DiscoveredObject<TBaseType> discoveredObject) => discoveredObject.Type;
	#endregion

	public Type Type { get; }
	public TBaseType UninitializedInstance { get; }
	
	private static readonly ConcurrentDictionary<Type, Func<TBaseType>> TypeCreatorCache = new();
	
	public DiscoveredObject(Type type)
	{
		this.Type = type;
		this.UninitializedInstance = CreateUninitializedInstance(type);
	}
	
	public static TBaseType CreateUninitializedInstance(Type type )
	{
		return (TBaseType)FormatterServices.GetUninitializedObject(type);
	}
	
	/// <summary>
	/// <p>Tries to find the <see cref="CodeChops.DomainDrivenDesign.DomainModeling.Factories.ICreatable{TObject}.Create(Validator)"/> factory method and invoke it.</p>
	/// <p>If the method is not found, it tries to find a parameterless constructor and invoke it.</p>
	/// </summary>
	/// <exception cref="InvalidOperationException">When the factory method and parameterless constructor are not found.</exception>
	public TBaseType CreateInstance()
	{
		return TypeCreatorCache.GetOrAdd(this.Type, GetInstanceCreator).Invoke();

		static Func<TBaseType> GetInstanceCreator(Type type)
		{
			var implementsICreatable = type.GetInterfaces().Any(i => i == typeof(ICreatable<>));
			if (implementsICreatable)
			{
				var factoryMethod = type.GetMethod(nameof(ICreatable<Dummy>.Create));

				if (factoryMethod is not null)
					return () => (TBaseType)factoryMethod.Invoke(null, Array.Empty<object>())!;
			}

			var parameterlessConstructor = type.GetConstructor(Type.EmptyTypes);
			if (parameterlessConstructor is null)
				throw new InvalidOperationException($"Could not create instance of {type.Name}: No parameterless constructor defined or ICreatable<> implemented. Creation of uninitialized objects is disable.");

			return () => (TBaseType)parameterlessConstructor.Invoke(Array.Empty<object>());
		}
	}

	// ReSharper disable once ClassNeverInstantiated.Local
	private class Dummy : ICreatable<Dummy>, IDomainObject
	{
		public static Dummy Create(Validator? validator = null) => throw new UnreachableException();
	}
}