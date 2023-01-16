namespace CodeChops.ImplementationDiscovery.Enums;

public record SimpleImplementationsEnum<TSelf, TBaseType> : ImplementationsEnumBase<TSelf, TBaseType>
	where TSelf : ImplementationsEnumBase<TSelf, TBaseType>, new()
	where TBaseType : notnull
{
	/// <inheritdoc cref="Discovered.DiscoveredObjectBase{TBaseType}.Type"/>
	public TBaseType Instance => this.Value.Instance;

	/// <inheritdoc cref="SimpleDiscoveredObject{TBaseType}.Create()"/>
	public TBaseType Create() => this.Value.Create();

	public new SimpleDiscoveredObject<TBaseType> Value => (SimpleDiscoveredObject<TBaseType>)((ImplementationsEnumBase<TSelf, TBaseType>)this).Value;
	
	/// <summary>
	/// Get an enumerable over the instances.
	/// </summary>
	public static IEnumerable<TBaseType> GetInstances() 
		=> GetMembers().Select(member => ((SimpleDiscoveredObject<TBaseType>)member.Value).Instance);
}