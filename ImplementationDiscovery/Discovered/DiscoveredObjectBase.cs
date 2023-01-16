namespace CodeChops.ImplementationDiscovery.Discovered;

[GenerateIdentity<string>] 
[DiscoverImplementations]
public abstract partial class DiscoveredObjectBase<TBaseType> : IEntity<DiscoveredObjectBase<TBaseType>>, IComparable<DiscoveredObjectBase<TBaseType>>
	where TBaseType : notnull
{
	public Type Type { get; }

	protected DiscoveredObjectBase(Type type)
	{
		this.Id = new Identity(type.FullName ?? type.Name);
		this.Type = type;
	}

	public int CompareTo(DiscoveredObjectBase<TBaseType>? other)
		=> String.Compare(((IId<Identity, string?>)this.Id).Value, ((IId<Identity, string?>?)other?.Id)?.Value, StringComparison.Ordinal);
}