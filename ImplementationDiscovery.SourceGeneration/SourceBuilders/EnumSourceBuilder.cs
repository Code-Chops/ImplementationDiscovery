﻿using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using CodeChops.ImplementationDiscovery.SourceGeneration.Entities;

namespace CodeChops.ImplementationDiscovery.SourceGeneration.SourceBuilders;

public static class EnumSourceBuilder
{
	/// <summary>
	/// Creates a partial record of the enum definition which includes the discovered enum members. It also generates an extension class for the explicit enum definitions.
	/// </summary>
	public static void CreateSource(SourceProductionContext context, IEnumerable<DiscoveredEnumMember> allDiscoveredMembers, Dictionary<string, EnumDefinition> enumDefinitionsByIdentifier)
	{
		if (enumDefinitionsByIdentifier.Count == 0) return;

		// Get the discovered members and their definition.
		// Exclude the members that have no definition, or the members that are discovered while their definition doesn't allow it.
		var relevantDiscoveredMembersByDefinition = allDiscoveredMembers
			.GroupBy(member => enumDefinitionsByIdentifier.TryGetValue(member.EnumIdentifier, out var definition) ? definition : null)
			.Where(grouping => grouping.Key is not null)
			.ToDictionary(grouping => grouping.Key, grouping => grouping.Where(member => grouping.Key!.DiscoverabilityMode == member.DiscoverabilityMode));

		foreach (var definition in enumDefinitionsByIdentifier.Values)
		{
			var relevantDiscoveredMembers = relevantDiscoveredMembersByDefinition.TryGetValue(definition, out var members)
				? members.ToList()
				: new List<DiscoveredEnumMember>();

			CreateEnumFile(context, definition!, relevantDiscoveredMembers);

			CreateStaticTypeIdFiles(context, definition!, relevantDiscoveredMembers);
		}
	}
	
	private static string GetValidFileName(string definitionName)
	{
		Span<char> buffer = new char[definitionName.Length];
		var i = 0;
		var invalidCharacters = Path.GetInvalidFileNameChars();

		foreach (var c in definitionName)
		{
			var newCharacter = c;
			if (invalidCharacters.Contains(c)) newCharacter = '_';
			buffer[i] = newCharacter;
			i++;
		}

		return buffer.Slice(0, i).ToString();
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

		var members = definition.MembersFromAttribute.Concat(relevantDiscoveredMembers);

		// Is used for correct enum member outlining.
		var longestMemberNameLength = members
			.Select(member => member.Name)
			.OrderByDescending(name => name.Length)
			.FirstOrDefault()?.Length ?? 0;

		// Create the whole source.
		code.Append($@"// <auto-generated />
#nullable enable

using System;
using CodeChops.MagicEnums;
{GetValueTypeUsing()}
{GetNamespaceDeclaration()}
{GetEnumRecord()}
{GetExtensionMethod()}
#nullable restore");

		var enumCodeFileName = GetValidFileName($"{definition.Identifier}.{definition.Name}.g.cs");
		context.AddSource(enumCodeFileName, SourceText.From(code.ToString(), Encoding.UTF8));
		return;

		
		// Creates a using for the definition of the enum value type (or null if not applicable).
		string? GetValueTypeUsing()
		{
			var ns = definition.ValueTypeNamespace;
			if (ns == "System") return null;

			ns = $"using {ns};{Environment.NewLine}";
			return ns;
		}


		// Creates the namespace definition of the location of the enum definition (or null if the namespace is not defined).
		string? GetNamespaceDeclaration()
		{
			if (definition.Namespace is null) return null;

			var code = $@"namespace {definition.Namespace};";
			return code;
		}


		// Creates the partial enum record (or null if the enum has no members).
		string? GetEnumRecord()
		{
			var isImplementationDiscovery = definition.DiscoverabilityMode == DiscoverabilityMode.Implementation;
			if (!members.Any() && !isImplementationDiscovery) return null;

			var code = new StringBuilder();

			var indent = isImplementationDiscovery ? (char?)'\t' : null;
			
			// Add the outer class
			code.AppendLine($@"
{definition.BaseClassDefinition} {definition.BaseClassName}
{{
");

			// Create the comments on the enum record.
			code.Append($@"
{indent}/// <summary>
{indent}/// <code>");
			
			foreach (var member in members)
			{
				var outlineSpacesLength = longestMemberNameLength - member.Name.Length;
				
				code.Append($@"
{indent}/// -{member.Name.PadRight(outlineSpacesLength)} = {member.Value ?? "?"}");
			}
			
			code.Append($@"
{indent}/// </code>
{indent}/// </summary>");
			
			// Define the magic enum record.
			var parentDefinition = $"MagicUninitializedObjectEnum<{definition.Name}, {definition.ValueTypeNamespace}.{definition.ValueTypeName}>";
			code.Append($@"
{indent}{definition.AccessModifier}partial record {(definition.IsStruct ? "struct " : "class")} {definition.Name} {(isImplementationDiscovery ? $": {parentDefinition}" : null)}
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

					if (member.Value is not null)
					{
						code.Append($@"
{indent}	/// <para>(value: {member.Value})</para>");
					}

					if (member.Comment is not null)
					{
						code.Append($@"
{indent}	/// {member.Comment}");
					}

					code.Append($@"
{indent}	/// </summary>");
				}

				// Create the enum member itself.
				var outlineSpaces = new String(' ', longestMemberNameLength - member.Name.Length);
				code.Append(@$"
{indent}	public static {definition.Name} {member.Name} {{ get; }} {outlineSpaces}= CreateMember({member.Value});
");
			}

			code.Append($@"
{indent}}}
");

			if (isImplementationDiscovery)
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
	public static {definition.Name} {SourceGenerator.GenerateMethodName}(this {definition.Name} member, {definition.ValueTypeName}? value = null, string? comment = null) => member;
}}
";			
		}
	}

	private static void CreateStaticTypeIdFiles(SourceProductionContext context, EnumDefinition definition, IEnumerable<DiscoveredEnumMember> relevantDiscoveredMembers)
	{
		if (definition.DiscoverabilityMode != DiscoverabilityMode.Implementation || !definition.GenerateIdsForImplementations) return;

		var partialMembers = relevantDiscoveredMembers.Where(m => m.IsPartial);
		
		foreach (var member in partialMembers)
		{
			var code = new StringBuilder();

			// Create the whole source.
			code.Append($@"// <auto-generated />
#nullable enable

using System;
using CodeChops.Identities;

namespace {member.Namespace};
");
			
			code.AppendLine($@"
{member.Definition} {member.Name} : IHasStaticTypeId<Id<string>>
{{
	public static Id<string> StaticTypeId {{ get; }} = new Id<string>(""{member.Name}"");
	public IId GetStaticTypeId() => StaticTypeId;
}}

#nullable restore
");
			
			var staticTypeIdFileName = GetValidFileName($"{member.Namespace}.{member.Name}.g.cs");
			context.AddSource(staticTypeIdFileName, SourceText.From(code.ToString(), Encoding.UTF8));
		}
	}
}