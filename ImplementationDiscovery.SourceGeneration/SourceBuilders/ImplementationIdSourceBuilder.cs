﻿using System.Text;
using CodeChops.ImplementationDiscovery.SourceGeneration.Models;

namespace CodeChops.ImplementationDiscovery.SourceGeneration.SourceBuilders;

internal static class ImplementationIdSourceBuilder
{
    public static void CreateSource(SourceProductionContext context, IEnumerable<DiscoveredEnumMember> allDiscoveredMembers, 
        List<EnumDefinition> definitions, AnalyzerConfigOptionsProvider configOptionsProvider)
    {
        if (definitions.Count == 0) return;
        
        try
        {
            var enumDefinitionsByIdentifier = definitions
                .GroupBy(d => d.EnumIdentifier)
                .ToDictionary(group => group.Key, group => group.First());
            
		    // Get the discovered members and their definition.
		    // Exclude the members that have no definition, or the members that are discovered while their definition doesn't allow it.
		    var relevantDiscoveredMembersByDefinitions = allDiscoveredMembers
                .GroupBy(member => enumDefinitionsByIdentifier.TryGetValue(member.EnumIdentifier, out var definition) ? definition : null)
                .Where(group => group.Key is not null)
                .GroupBy(group => group.Key)
                .ToDictionary(group => group.Key, group => group.First());
            
            foreach (var discoveredMembersByDefinition in relevantDiscoveredMembersByDefinitions)
            {
                var definition = discoveredMembersByDefinition.Key!;
                var members = discoveredMembersByDefinition.Value.ToList();
            
                CreateDiscoveredImplementationIdFiles(context, definition, members, configOptionsProvider);
            }
        }
        
#pragma warning disable CS0168
        catch (Exception e)
#pragma warning restore CS0168
        {
            var descriptor = new DiagnosticDescriptor(nameof(ImplementationIdSourceBuilder), "Error", $"{nameof(ImplementationIdSourceBuilder)} failed to generate due to an error. Please inform CodeChops (www.CodeChops.nl). Error: {e}", "Compilation", DiagnosticSeverity.Error, isEnabledByDefault: true);
            context.ReportDiagnostic(Diagnostic.Create(descriptor, null));

            context.AddSource($"{nameof(ImplementationIdSourceBuilder)}_Exception_{Guid.NewGuid()}", SourceText.From($"/*{e}*/", Encoding.UTF8));
        }
    }
    
    private static void CreateDiscoveredImplementationIdFiles(SourceProductionContext context, EnumDefinition definition, 
        IEnumerable<DiscoveredEnumMember> relevantDiscoveredMembers, AnalyzerConfigOptionsProvider configOptionsProvider)
    {
        if (!definition.GenerateImplementationIds && !definition.HasSingletonImplementations) return;
        if (definition.HasSingletonImplementations && !definition.GenerateImplementationIds) return;
        
        var partialMembers = relevantDiscoveredMembers.Where(m => m.IsPartial);
		
        foreach (var member in partialMembers)
        {
            var code = new StringBuilder();

            // Create the whole source.
            code.Append($@"// <auto-generated />
#nullable enable
#pragma warning disable CS0109

{GetUsings()}

{(member.Namespace is null ? null : $"namespace {member.Namespace};")}
");

            var implementationsEnum = $"global::{definition.Namespace}.{definition.Name}{definition.TypeParameters}";

            code.AppendLine($@"
{member.Declaration} {member.GetClassName()}{definition.TypeParameters} : IHasImplementationId<{implementationsEnum}>, IHasStaticImplementationId<{implementationsEnum}>, IDiscovered
    {definition.BaseTypeGenericConstraints}
{{
");

            if (definition.HasSingletonImplementations)
            {
                code.AppendLine($@"
	public IId Id => ImplementationId;
");
            
                code.AppendLine($@"
	public new static {implementationsEnum} ImplementationId {{ get; }} = {implementationsEnum}.{member.GetSimpleName(definition)};
    public {(definition.BaseTypeTypeKind == TypeKind.Class ? "override " : "")}{implementationsEnum} GetImplementationId() => ImplementationId;
");
            }

            code.AppendLine($@"
}}
       
#nullable restore
");
			
            var implementationIdFileName = FileNameHelpers.GetFileName($"{member.Namespace}.{member.GetSimpleName(definition)}.ImplementationId", configOptionsProvider);
            context.AddSource(implementationIdFileName, SourceText.From(code.ToString(), Encoding.UTF8));

            
            string GetUsings()
            {
                var usings = definition.Usings.Concat(new[] { "using System;", "using CodeChops.ImplementationDiscovery;" });
			
                return usings.Distinct().OrderBy(u => u).Aggregate(new StringBuilder(), (sb, u) => sb.AppendLine(u)).ToString();
            }
        }
    }

}