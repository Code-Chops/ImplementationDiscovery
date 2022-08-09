namespace CodeChops.ImplementationDiscovery.UninitializedObjects;

public record UninitializedObject<TBaseType>: IComparable<UninitializedObject<TBaseType>>
	where TBaseType : class
{
	public TBaseType UninitializedInstance { get; }
	private Type Type { get; } 
	
	protected UninitializedObject(Type type)
	{
		this.UninitializedInstance = (TBaseType)FormatterServices.GetUninitializedObject(type);
		this.Type = type;
	}

	public static UninitializedObject<TBaseType> Create(Type type)
	{
		return new UninitializedObject<TBaseType>(type);
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Type(UninitializedObject<TBaseType> uninitializedObject) => uninitializedObject.Type;
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator UninitializedObject<TBaseType>(Type type) => Create(type);

	
	#region Comparison
	
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
	public virtual bool Equals(UninitializedObject<TBaseType>? other) 
		=> this.Type == other?.Type;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode()
		=> this.Type.GetHashCode();
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int CompareTo(UninitializedObject<TBaseType>? other) 
		=> String.Compare(this.Type.FullName, other?.Type.FullName, StringComparison.Ordinal);

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