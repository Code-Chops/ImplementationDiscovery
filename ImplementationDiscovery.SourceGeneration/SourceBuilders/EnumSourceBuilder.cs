﻿using System.Text;
using CodeChops.ImplementationDiscovery.SourceGeneration.Models;

namespace CodeChops.ImplementationDiscovery.SourceGeneration.SourceBuilders;

internal static class EnumSourceBuilder
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

		foreach (var definition in enumDefinitionsByIdentifier.Values)
		{
			var relevantDiscoveredMembers = relevantDiscoveredMembersByDefinitions.TryGetValue(definition, out var members)
				? members.ToList()
				: new List<DiscoveredEnumMember>();

			CreateEnumFile(context, definition!, relevantDiscoveredMembers);
		}
	}
	
	private static void CreateEnumFile(SourceProductionContext context, EnumDefinition definition, List<DiscoveredEnumMember> relevantDiscoveredMembers)
	{
		var code = new StringBuilder();

		// Place the members that are discovered in the enum definition file itself first. The order can be relevant because the value of enum members can be implicitly incremental.
		// Do a distinct on the file path and line position so the members will be deduplicated while typing their invocation.
		// Also do a distinct on the member name.		
		relevantDiscoveredMembers = relevantDiscoveredMembers
			.OrderByDescending(member => member.FilePath == definition.FilePath)
			.GroupBy(member => (member.FilePath, member.LinePosition))
			.Select(group => group.First())
			.GroupBy(member => member.Name)
			.Select(membersByName => membersByName.First())
			.ToList();

		var members = definition.MembersFromAttribute.Concat(relevantDiscoveredMembers).ToList();

		// Is used for correct enum member outlining.
		var longestMemberNameLength = members
			.Select(member => member.Name)
			.OrderByDescending(name => name.Length)
			.FirstOrDefault()?.Length ?? 0;

		// Create the whole source.
		code.Append($@"// <auto-generated />
#nullable enable
#pragma warning disable CS0109

using System;
using CodeChops.ImplementationDiscovery;
{GetBaseTypeUsing()}
{GetNamespaceDeclaration()}
{GetEnumRecord()}
{GetExtensionMethod()}
#nullable restore
");

		var enumCodeFileName = FileNameHelpers.GetValidFileName($"{definition.Identifier}.{definition.Name}.g.cs");
		context.AddSource(enumCodeFileName, SourceText.From(code.ToString(), Encoding.UTF8));
		return;

		
		// Creates a using for the definition of the enum value type (or null if not applicable).
		string? GetBaseTypeUsing()
		{
			if (definition.BaseTypeDeclaration is null || definition.Namespace is null or "System") return null;

			var @namespace = $"using {definition.Namespace};{Environment.NewLine}";
			return @namespace;
		}


		// Creates the namespace definition of the location of the enum definition (or null if the namespace is not defined).
		string? GetNamespaceDeclaration()
		{
			if (definition.Namespace is null) return null;

			var code = $@"namespace {definition.Namespace};";
			return code;
		}


		// Creates the partial enum record (or null if the enum has no members).
		string GetEnumRecord()
		{
			var hasOuterClass = definition.BaseTypeDeclaration is not null;

			var code = new StringBuilder();

			if (hasOuterClass)
			{
				code.AppendLine($@"
{definition.BaseTypeDeclaration} {definition.BaseTypeName} {(definition.GenerateTypeIdsForImplementations ? $": global::CodeChops.ImplementationDiscovery.IHasDiscoverableImplementations<{definition.BaseTypeName}.{definition.Name}>" : null)}
{{
");
			}

			var indent = hasOuterClass ? (char?)'\t' : null;
			if (definition.BaseTypeTypeKind == TypeKind.Class && definition.GenerateTypeIdsForImplementations)
			{
				code.AppendLine($@"
{indent}public new abstract {definition.Name} TypeId {{ get; }}
");
			}

			// Create the comments on the enum record.
			code.Append($@"
{indent}/// <summary>
{indent}/// <list type=""bullet"">");
			
			foreach (var member in members)
			{
				var outlineSpaces = new String(' ', longestMemberNameLength - member.Name.Length);
				
				code.Append($@"
{indent}/// <item><c><![CDATA[ {member.Name}{outlineSpaces} = {member.Value ?? "?"} ]]></c></item>");
			}
			
			code.Append($@"
{indent}/// </list>
{indent}/// </summary>");
			
			// Define the magic enum record.
			var baseTypeFullName = definition.BaseTypeName ?? nameof(Object); 
			var uninitializedObject = definition.HasNewableImplementations
				? "NewableUninitializedObject"
				: "UninitializedObject";
			var parentDefinition = $"MagicDiscoveredImplementationsEnum<{definition.Name}, {baseTypeFullName}, global::CodeChops.ImplementationDiscovery.UninitializedObjects.{uninitializedObject}<{baseTypeFullName}>>";

			code.Append($@"
{indent}{definition.AccessModifier} {(members.Any() ? null : "abstract ")}partial record {definition.Name} {(definition.DiscoverabilityMode == DiscoverabilityMode.Implementation ? $": {parentDefinition}" : null)}
{indent}{{	
");

			// Add the discovered members to the enum record.
			foreach (var member in members)
			{
				// Create the comment on the enum member.
				if (member.Value is not null || member.Comment is not null)
				{
					code.Append($@"
{indent}	/// <summary>");

					if (member.Comment is not null)
					{
						code.Append($@"
{indent}	/// <para>{member.Comment}</para>");
					}

					if (member.Value is not null)
					{
						code.Append($@"
{indent}	/// <c><![CDATA[ (value: {member.Value}) ]]></c>");
					}

					code.Append($@"
{indent}	/// </summary>");
				}

				// Create the enum member itself.
				var outlineSpaces = new String(' ', longestMemberNameLength - member.Name.Length);
				
				var memberInitialization = definition.HasNewableImplementations
					? $"global::CodeChops.ImplementationDiscovery.UninitializedObjects.NewableUninitializedObject<{baseTypeFullName}>.Create<{member.Value}>()"
					: $"global::CodeChops.ImplementationDiscovery.UninitializedObjects.UninitializedObject<{baseTypeFullName}>.Create(typeof({member.Value}))";
				code.Append(@$"
{indent}	public static {definition.Name} {member.Name} {{ get; }} {outlineSpaces}= CreateMember({memberInitialization});
");
			}

			code.Append($@"
{indent}}}
");

			if (hasOuterClass)
			{
				code.Append($@"
}}
");
			}

			return code.ToString();
		}


		string? GetExtensionMethod()
		{
			if (definition.DiscoverabilityMode == DiscoverabilityMode.Implementation) return null;

			return
$@"
/// <summary>
/// Call this method in order to create discovered enum members while invoking them (on the fly). So enum members are automatically deleted when not being used.
/// </summary>
{definition.AccessModifier}static class {definition.Name}Extensions
{{
	public static {definition.Name} {SourceGenerator.GenerateMethodName}(this {definition.Name} member, {definition.BaseTypeName}? value = null, string? comment = null) => member;
}}
";			
		}
	}
}