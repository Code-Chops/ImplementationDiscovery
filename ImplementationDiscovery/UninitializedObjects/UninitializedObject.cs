namespace CodeChops.ImplementationDiscovery.UninitializedObjects;

public record UninitializedObject<TBaseType> : IComparable<UninitializedObject<TBaseType>>, IConvertible 
	where TBaseType : class
{
	#region Comparison

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public virtual bool Equals(UninitializedObject<TBaseType>? other) => this.Type == other?.Type;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => String.GetHashCode(this.Type.Name, StringComparison.Ordinal);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int CompareTo(UninitializedObject<TBaseType>? other) => String.Compare(this.Type.FullName, other?.Type.FullName, StringComparison.Ordinal);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator <(UninitializedObject<TBaseType> left, UninitializedObject<TBaseType> right) => left.CompareTo(right) < 0;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator <=(UninitializedObject<TBaseType> left, UninitializedObject<TBaseType> right) => left.CompareTo(right) <= 0;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator >(UninitializedObject<TBaseType> left, UninitializedObject<TBaseType> right) => left.CompareTo(right) > 0;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator >=(UninitializedObject<TBaseType> left, UninitializedObject<TBaseType> right) => left.CompareTo(right) >= 0;

	#endregion

	#region Casting

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator UninitializedObject<TBaseType>(Type type) => new(type);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator TBaseType(UninitializedObject<TBaseType> uninitializedObject) => uninitializedObject.UninitializedInstance;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Type(UninitializedObject<TBaseType> uninitializedObject) => uninitializedObject.Type;

	#endregion

	public TBaseType UninitializedInstance { get; }
	protected Type Type { get; }

	private UninitializedObject(Type type)
	{
		this.UninitializedInstance = (TBaseType)FormatterServices.GetUninitializedObject(type);
		this.Type = type;
	}

	public static UninitializedObject<TBaseType> Create(Type type) => new(type);
	
	public TypeCode GetTypeCode() => this.Type.Name.GetTypeCode();
	object IConvertible.ToType(Type type, IFormatProvider? provider) => ((IConvertible)this.Type.Name).ToType(type, provider);
	public string ToString(IFormatProvider? provider) => this.Type.Name.ToString(provider);
	bool IConvertible.ToBoolean(IFormatProvider? provider) => ((IConvertible)this.Type.Name).ToBoolean(provider);
	char IConvertible.ToChar(IFormatProvider? provider) => ((IConvertible)this.Type.Name).ToChar(provider);
	sbyte IConvertible.ToSByte(IFormatProvider? provider) => ((IConvertible)this.Type.Name).ToSByte(provider);
	byte IConvertible.ToByte(IFormatProvider? provider) => ((IConvertible)this.Type.Name).ToByte(provider);
	short IConvertible.ToInt16(IFormatProvider? provider) => ((IConvertible)this.Type.Name).ToInt16(provider);
	ushort IConvertible.ToUInt16(IFormatProvider? provider) => ((IConvertible)this.Type.Name).ToUInt16(provider);
	int IConvertible.ToInt32(IFormatProvider? provider) => ((IConvertible)this.Type.Name).ToInt32(provider);
	uint IConvertible.ToUInt32(IFormatProvider? provider) => ((IConvertible)this.Type.Name).ToUInt32(provider);
	long IConvertible.ToInt64(IFormatProvider? provider) => ((IConvertible)this.Type.Name).ToInt64(provider);
	ulong IConvertible.ToUInt64(IFormatProvider? provider) => ((IConvertible)this.Type.Name).ToUInt64(provider);
	float IConvertible.ToSingle(IFormatProvider? provider) => ((IConvertible)this.Type.Name).ToSingle(provider);
	double IConvertible.ToDouble(IFormatProvider? provider) => ((IConvertible)this.Type.Name).ToDouble(provider);
	decimal IConvertible.ToDecimal(IFormatProvider? provider) => ((IConvertible)this.Type.Name).ToDecimal(provider);
	DateTime IConvertible.ToDateTime(IFormatProvider? provider) => ((IConvertible)this.Type.Name).ToDateTime(provider);
}