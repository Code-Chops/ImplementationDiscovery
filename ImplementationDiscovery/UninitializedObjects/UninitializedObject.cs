namespace CodeChops.ImplementationDiscovery.UninitializedObjects;

public record UninitializedObject<TBaseType>: IComparable<UninitializedObject<TBaseType>>
	where TBaseType : class
{
	public TBaseType UninitializedInstance { get; }

	protected UninitializedObject(TBaseType uninitializedInstance)
	{
		this.UninitializedInstance = uninitializedInstance;
	}

	public static UninitializedObject<TBaseType> Create(Type type)
	{
		return new UninitializedObject<TBaseType>((TBaseType)FormatterServices.GetUninitializedObject(type));
	}
	
	#region Comparison
	
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
	public virtual bool Equals(UninitializedObject<TBaseType>? other) 
		=> this.UninitializedInstance.Equals(other?.UninitializedInstance);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode()
		=> this.UninitializedInstance.GetHashCode();
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int CompareTo(UninitializedObject<TBaseType>? other) 
		=> String.Compare(this.UninitializedInstance.GetType().FullName, other?.UninitializedInstance.GetType().FullName, StringComparison.Ordinal);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator <(UninitializedObject<TBaseType> left, UninitializedObject<TBaseType> right)	=> left.CompareTo(right) <	0;
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator <=(UninitializedObject<TBaseType> left, UninitializedObject<TBaseType> right)	=> left.CompareTo(right) <= 0;
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator >(UninitializedObject<TBaseType> left, UninitializedObject<TBaseType> right)	=> left.CompareTo(right) >	0;
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator >=(UninitializedObject<TBaseType> left, UninitializedObject<TBaseType> right)	=> left.CompareTo(right) >= 0;

    #endregion
}