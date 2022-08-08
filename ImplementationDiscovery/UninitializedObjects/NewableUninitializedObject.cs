namespace CodeChops.ImplementationDiscovery.UninitializedObjects;

public record NewableUninitializedObject<TBaseType> : UninitializedObject<TBaseType>
	where TBaseType : class
{
	/// <summary>
	/// Creates a new instance by using the public constructor without parameters.
	/// </summary>
	public TBaseType CreateNewInstance() => this.NewInstanceFactory();
	private Func<TBaseType> NewInstanceFactory { get; }

	public static NewableUninitializedObject<TBaseType> Create<TImplementation>()
		where TImplementation : TBaseType, new()
	{
		return new NewableUninitializedObject<TBaseType>(
			type: typeof(TImplementation), 
			newInstanceFactory: () => new TImplementation());
	}

	private NewableUninitializedObject(Type type, Func<TBaseType> newInstanceFactory) 
		: base(type)
	{
		this.NewInstanceFactory = newInstanceFactory;
	}
}