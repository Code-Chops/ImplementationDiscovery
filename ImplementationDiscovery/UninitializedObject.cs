namespace CodeChops.ImplementationDiscovery;

public readonly struct UninitializedObject<T> : IEquatable<UninitializedObject<T>>, IComparable<UninitializedObject<T>>
	where T : class
{
	public T Instance { get; }
		
	public UninitializedObject(T instance)
	{
		this.Instance = instance;
	}

	#region Comparison

	public static bool operator ==(UninitializedObject<T> left, UninitializedObject<T> right) 
		=> left.Equals(right);

	public static bool operator !=(UninitializedObject<T> left, UninitializedObject<T> right) 
		=> !(left == right);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals(object? obj) 
	    => obj is UninitializedObject<T> uninitializedObject && this.Equals(uninitializedObject);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(UninitializedObject<T> other) 
		=> this.Instance.Equals(other.Instance);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode()
		=> this.Instance.GetHashCode();
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator T(UninitializedObject<T> magicEnum) => magicEnum.Instance;
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator UninitializedObject<T>(T value) => new(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int CompareTo(UninitializedObject<T> other) 
		=> String.Compare(this.Instance.GetType().FullName, other.Instance.GetType().FullName, StringComparison.Ordinal);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator <(UninitializedObject<T> left, UninitializedObject<T> right)	=> left.CompareTo(right) <	0;
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator <=(UninitializedObject<T> left, UninitializedObject<T> right)	=> left.CompareTo(right) <= 0;
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator >(UninitializedObject<T> left, UninitializedObject<T> right)	=> left.CompareTo(right) >	0;
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator >=(UninitializedObject<T> left, UninitializedObject<T> right)	=> left.CompareTo(right) >= 0;

    #endregion

}