// ReSharper disable once CheckNamespace
namespace CodeChops.DomainDrivenDesign.DomainModeling.Identities;

/// <summary>
/// Contains a static type discriminator. 
/// </summary>
public interface IHasStaticDiscoveredTypeEnum<out TTypeId>
	where TTypeId : IId
{
	public static abstract TTypeId StaticTypeEnum { get; }
}