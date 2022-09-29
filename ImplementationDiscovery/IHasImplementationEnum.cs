// ReSharper disable once CheckNamespace
namespace CodeChops.DomainDrivenDesign.DomainModeling.Identities;

/// <summary>
/// Contains a implementation type discriminator. 
/// </summary>
public interface IHasImplementationEnum<out TId>
	where TId : IId
{
	TId ImplementationEnum { get; }
}

/// <summary>
/// Used to get the implementation type ID of runtime types (when the type is not known).
/// </summary>
public interface IHasImplementationEnum
{
	IId GetTypeId();
}