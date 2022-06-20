using Microsoft.CodeAnalysis;

namespace CodeChops.ImplementationDiscovery.SourceGeneration.Entities;

internal record ImplementationData(DiscoveredEnumMember Member, ITypeSymbol Type, ITypeSymbol BaseType, string EnumNamespace);