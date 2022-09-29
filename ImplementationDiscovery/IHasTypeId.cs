// ReSharper disable once CheckNamespace
namespace CodeChops.DomainDrivenDesign.DomainModeling.Identities;

/// <summary>
/// Contains a type discriminator. 
/// </summary>
public interface IHasTypeId<out TId>
	where TId : IId
{
	TId TypeEnum { get; }
}

/// <summary>
/// Used to get the type ID of runtime types (when the type is not known).
/// </summary>
public interface IHasTypeId
{
	IId GetTypeId();
}