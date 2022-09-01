namespace CodeChops.ImplementationDiscovery.UninitializedObjects;

public record NewableUninitializedObject<TBaseType> : UninitializedObject<TBaseType>
	where TBaseType : class
{
	#region Casting
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator TBaseType(NewableUninitializedObject<TBaseType> uninitializedObject) => uninitializedObject.UninitializedInstance;
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Type(NewableUninitializedObject<TBaseType> uninitializedObject) => uninitializedObject.Type;
	
	#endregion
	
	protected NewableUninitializedObject(Type type, Func<TBaseType> instanceFactory)
		: base(type)
	{
		this.InstanceFactory = instanceFactory;
	}

	/// <summary>
	/// Creates a new instance by using the public constructor without parameters.
	/// </summary>
	public TBaseType CreateNewInstance() => this.InstanceFactory();

	private Func<TBaseType> InstanceFactory { get; }

	public static NewableUninitializedObject<TBaseType> Create<TImplementation>() 
		where TImplementation : TBaseType, new()
	{
		return new NewableUninitializedObject<TBaseType>(type: typeof(TImplementation), instanceFactory: () => new TImplementation());
	}
}