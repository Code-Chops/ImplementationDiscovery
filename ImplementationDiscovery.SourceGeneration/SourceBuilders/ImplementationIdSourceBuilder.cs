﻿using System.Text;
using CodeChops.ImplementationDiscovery.SourceGeneration.Models;

namespace CodeChops.ImplementationDiscovery.SourceGeneration.SourceBuilders;

internal static class ImplementationIdSourceBuilder
{
    /// <summary>
    /// Creates a partial record of the enum definition which includes the discovered enum members. It also generates an extension class for the explicit enum definitions.
    /// </summary>
    public static void CreateSource(SourceProductionContext context, IEnumerable<DiscoveredEnumMember> allDiscoveredMembers, 
        Dictionary<string, EnumDefinition> enumDefinitionsByIdentifier, AnalyzerConfigOptionsProvider configOptionsProvider)
    {
        if (enumDefinitionsByIdentifier.Count == 0) return;

        // Get the discovered members and their definition.
        // Exclude the members that have no definition, or the members that are discovered while their definition doesn't allow it.
        var relevantDiscoveredMembersByDefinitions = allDiscoveredMembers
            .GroupBy(member => enumDefinitionsByIdentifier.TryGetValue(member.EnumIdentifier, out var definition) ? definition : null)
            .Where(grouping => grouping.Key is not null)
            .ToDictionary(grouping => grouping.Key, grouping => grouping.Where(member => grouping.Key!.DiscoverabilityMode == member.DiscoverabilityMode));
        
        foreach (var discoveredMembersByDefinition in relevantDiscoveredMembersByDefinitions)
        {
            var definition = discoveredMembersByDefinition.Key!;

            if (NameHelpers.HasGenericParameter(definition.BaseTypeName!)) continue;
            
            var members = discoveredMembersByDefinition.Value.ToList();

            CreateStaticDiscoveredTypeIdFiles(context, definition, members, configOptionsProvider);
        }
    }
    
    private static void CreateStaticDiscoveredTypeIdFiles(SourceProductionContext context, EnumDefinition definition, 
        IEnumerable<DiscoveredEnumMember> relevantDiscoveredMembers, AnalyzerConfigOptionsProvider configOptionsProvider)
    {
        if (definition.DiscoverabilityMode != DiscoverabilityMode.Implementation || !definition.GenerateTypeIdsForImplementations) return;

        var partialMembers = relevantDiscoveredMembers.Where(m => m.IsPartial);
		
        foreach (var member in partialMembers)
        {
            var code = new StringBuilder();

            // Create the whole source.
            code.Append($@"// <auto-generated />
#nullable enable
#pragma warning disable CS0109

using System;
using CodeChops.DomainDrivenDesign.DomainModeling.Identities;
using CodeChops.ImplementationDiscovery;
using BaseType = global::{definition.Namespace}.{definition.BaseTypeName};

{(member.Namespace is null ? null : $"namespace {member.Namespace};")}
");
            if (!NameHelpers.HasGenericParameter(member.Name))
            {
                var typeIdName = $"global::{definition.Namespace}.{definition.Name}";
                code.AppendLine($@"
{member.Declaration} {member.Name} : global::CodeChops.ImplementationDiscovery.IHasDiscoverableImplementations<{typeIdName}>, IHasStaticTypeId<{typeIdName}>
{{

	public new static {typeIdName} StaticTypeId {{ get; }} = {typeIdName}.{member.Name};
    {GetNonStaticTypeId(typeIdName)}
}}");
            }

            code.AppendLine(@"                
#nullable restore
");
			
            var typeIdFileName = FileNameHelpers.GetFileName($"{member.Namespace}.{member.Name}.TypeId", configOptionsProvider);
            context.AddSource(typeIdFileName, SourceText.From(code.ToString(), Encoding.UTF8));


            string? GetNonStaticTypeId(string typeIdName)
            {
                if (!definition.GenerateTypeIdsForImplementations) return null;

                var code = $@"
    public {(definition.BaseTypeTypeKind == TypeKind.Class ? "override " : "")}{typeIdName} TypeId => ({typeIdName})StaticTypeId;
";
                return code;
            }
        }
    }

}