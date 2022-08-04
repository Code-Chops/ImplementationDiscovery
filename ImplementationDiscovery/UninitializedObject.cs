namespace CodeChops.ImplementationDiscovery;

public readonly struct UninitializedObject<T> : IEquatable<UninitializedObject<T>>, IComparable<UninitializedObject<T>>
	where T : class
{
	public T UninitializedInstance { get; }
		
	public UninitializedObject(T uninitializedInstance)
	{
		this.UninitializedInstance = uninitializedInstance;
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
		=> this.UninitializedInstance.Equals(other.UninitializedInstance);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode()
		=> this.UninitializedInstance.GetHashCode();
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator T(UninitializedObject<T> magicEnum) => magicEnum.UninitializedInstance;
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator UninitializedObject<T>(T value) => new(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int CompareTo(UninitializedObject<T> other) 
		=> String.Compare(this.UninitializedInstance.GetType().FullName, other.UninitializedInstance.GetType().FullName, StringComparison.Ordinal);

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