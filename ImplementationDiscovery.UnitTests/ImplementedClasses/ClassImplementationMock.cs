using CodeChops.ImplementationDiscovery.Attributes;

namespace CodeChops.ImplementationDiscovery.UnitTests.ImplementedClasses;

[DiscoverImplementations]
public abstract partial class AbstractClassToImplement
{
}

public class ClassImplementationMock : AbstractClassToImplement
{
}