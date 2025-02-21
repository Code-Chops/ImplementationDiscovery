using Architect.DomainModeling;

namespace CodeChops.ImplementationDiscovery.UnitTests.CreationMethods;

[DiscoverImplementations]
public partial interface ICreationMock
{
    string Value { get; }
}

public record UninitializedCreationMock : ICreationMock
{
    public string Value => "ThisValueShouldNotBeSet";
}

public record NewableCreationMock : ICreationMock, INewable<NewableCreationMock>, IDomainObject
{
    public string Value => "ValueSetInConstructor";
}

public record CreatableCreationMock : ICreationMock, ICreatable<CreatableCreationMock>, IDomainObject
{
    public string Value { get; private init; } = null!;

    public static CreatableCreationMock Create()
    {
        return new() { Value = "ValueSetInFactory" };
    }
}