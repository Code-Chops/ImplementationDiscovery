// ReSharper disable once CheckNamespace
namespace CodeChops.DomainDrivenDesign.DomainModeling.Identities;

/// <summary>
/// Contains a static type discriminator. 
/// </summary>
public interface IHasStaticImplementationEnum<out TTypeId>
	where TTypeId : IId
{
	public static abstract TTypeId StaticImplementationEnum { get; }
}