﻿using System.Text;
using CodeChops.ImplementationDiscovery.SourceGeneration.Models;

namespace CodeChops.ImplementationDiscovery.SourceGeneration.SourceBuilders;

internal static class ImplementationIdSourceBuilder
{
    /// <summary>
    /// Creates a partial record of the enum definition which includes the discovered enum members. It also generates an extension class for the explicit enum definitions.
    /// </summary>
    public static void CreateSource(SourceProductionContext context, IEnumerable<DiscoveredEnumMember> allDiscoveredMembers, Dictionary<string, EnumDefinition> enumDefinitionsByIdentifier)
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

            if (NameHelpers.HasGenericParameter(definition.OuterClassName!)) continue;
            
            var members = discoveredMembersByDefinition.Value.ToList();

            CreateStaticDiscoveredTypeIdFiles(context, definition!, members);
        }
    }
    
    private static void CreateStaticDiscoveredTypeIdFiles(SourceProductionContext context, EnumDefinition definition, IEnumerable<DiscoveredEnumMember> relevantDiscoveredMembers)
    {
        if (definition.DiscoverabilityMode != DiscoverabilityMode.Implementation || !definition.GenerateIdsForImplementations) return;

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
using EnumNamespace = global::{definition.Namespace}.{definition.OuterClassName};

{(member.Namespace is null ? null : $"namespace {member.Namespace};")}
");
            if (!NameHelpers.HasGenericParameter(member.Name))
            {
                var typeIdName = $"EnumNamespace.{definition.OuterClassName}TypeId";
                code.AppendLine($@"
{member.Declaration} {member.Name} : CodeChops.DomainDrivenDesign.DomainModeling.Identities.IHasStaticTypeId<{typeIdName}>
{{
	public static new {typeIdName} StaticTypeId {{ get; }} = new {typeIdName}(EnumNamespace.{definition.Name}.{member.Name}.Name);
    {GetNonStaticTypeId()}
}}");
            }

            code.AppendLine(@"                
#nullable restore
");
			
            var typeIdFileName = FileNameHelpers.GetValidFileName($"{member.Namespace}.{member.Name}.g.cs");
            context.AddSource(typeIdFileName, SourceText.From(code.ToString(), Encoding.UTF8));


            string? GetNonStaticTypeId()
            {
                if (!definition.GenerateIdsForImplementations) return null;

                var code = $@"
    public {(definition.OuterClassTypeKind == TypeKind.Class ? "override " : "")}global::CodeChops.DomainDrivenDesign.DomainModeling.Identities.Id GetStaticTypeId() => StaticTypeId;
";
                return code;
            }
        }
    }

}