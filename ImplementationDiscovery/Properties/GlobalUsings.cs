global using System.Runtime.CompilerServices;
global using System.Runtime.Serialization;

global using CodeChops.MagicEnums;
using CodeChops.ImplementationDiscovery;

[DiscoverImplementations]
public partial record Animal;

public partial record Dog : Animal;
public partial record Cat : Animal;