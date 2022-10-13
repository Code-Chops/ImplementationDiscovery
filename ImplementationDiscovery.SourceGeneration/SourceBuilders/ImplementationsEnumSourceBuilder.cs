﻿using System.Diagnostics;
using System.Text;
using CodeChops.ImplementationDiscovery.SourceGeneration.Models;

namespace CodeChops.ImplementationDiscovery.SourceGeneration.SourceBuilders;

internal static class ImplementationsEnumSourceBuilder
{
	/// <summary>
	/// Creates a partial record of the enum definition which includes the discovered enum members. It also generates an extension class for the explicit enum definitions.
	/// </summary>
	public static void CreateSource(SourceProductionContext context, IEnumerable<DiscoveredEnumMember> allDiscoveredMembers, 
		List<EnumDefinition> definitions, AnalyzerConfigOptionsProvider configOptionsProvider)
	{
		try
		{
			if (definitions.Count == 0) return;
			var enumDefinitionsByIdentifier = definitions.ToDictionary(d => d.EnumIdentifier);
	
			// Get the discovered members and their definition.
			// Exclude the members that have no definition.
			var relevantDiscoveredMembersByDefinitions = allDiscoveredMembers
				.GroupBy(member => enumDefinitionsByIdentifier.TryGetValue(member.EnumIdentifier, out var definition) ? definition : null)
				.Where(group => group.Key is not null)
				.GroupBy(group => group.Key!.EnumIdentifier)
				.ToDictionary(group => group.Key, group => group.First());
	
			foreach (var definition in enumDefinitionsByIdentifier.Values)
			{
				var relevantDiscoveredMembers = relevantDiscoveredMembersByDefinitions.TryGetValue(definition.EnumIdentifier, out var members)
					? members.ToList()
					: new List<DiscoveredEnumMember>();
	
				CreateEnumFile(context, definition, relevantDiscoveredMembers, configOptionsProvider);
			}

		}
#pragma warning disable CS0168
		catch (Exception e)
#pragma warning restore CS0168
        {
            Debugger.Launch();
            throw;
        }		
	}
	
	private static void CreateEnumFile(SourceProductionContext context, EnumDefinition definition, List<DiscoveredEnumMember> members, AnalyzerConfigOptionsProvider configOptionsProvider)
	{
		var code = new StringBuilder();

		// Place the members that are discovered in the enum definition file itself first. The order can be relevant because the value of enum members can be implicitly incremental.
		// Do a distinct on the file path and line position so the members will be deduplicated while typing their invocation.
		// Also do a distinct on the member name.		
		members = members
			.OrderByDescending(member => member.FilePath == definition.FilePath)
			.GroupBy(member => (member.FilePath, member.LinePosition))
			.Select(group => group.First())
			.GroupBy(member => member.Name)
			.Select(membersByName => membersByName.First())
			.ToList();

		// Is used for correct enum member outlining.
		var longestMemberNameLength = members
			.Select(member => member.Name)
			.OrderByDescending(name => name.Length)
			.FirstOrDefault()?.Length ?? 0;

		// Create the whole source.
		code.Append($@"// <auto-generated />
#nullable enable
#pragma warning disable CS0109

{GetUsings()}
{GetNamespaceDeclaration()}
{GetImplementationIdProperty()}
{GetEnumRecord()}
{GetExtensionMethod()}
#nullable restore
");

		var enumCodeFileName = FileNameHelpers.GetFileName($"{definition.Namespace}.{definition.Name}", configOptionsProvider);
		context.AddSource(enumCodeFileName, SourceText.From(code.ToString(), Encoding.UTF8));
		return;

		
		string GetUsings()
		{
			var usings = definition.Usings.Concat(new[]
			{
				"using System;", 
				"using CodeChops.ImplementationDiscovery;", 
				"using CodeChops.MagicEnums.Core;", 
				$"using {definition.Namespace ?? "System"};", 
				"using System.Diagnostics.CodeAnalysis;"
			});
			
			return usings.Distinct().OrderBy(u => u).Aggregate(new StringBuilder(), (sb, u) => sb.AppendLine(u)).ToString();
		}

		// Creates the namespace definition of the location of the enum definition (or null if the namespace is not defined).
		string? GetNamespaceDeclaration()
		{
			if (definition.Namespace is null) return null;

			var code = $@"namespace {definition.Namespace};";
			return code;
		}


		string? GetImplementationIdProperty()
		{
			if (definition.BaseTypeDeclaration is null) return null;

			var code = new StringBuilder();

			var implementationsEnum = $"{definition.Name}{definition.TypeParameters}";
			
			code.AppendLine($@"
{definition.BaseTypeDeclaration} {definition.BaseTypeName} {(definition.BaseTypeTypeKind == TypeKind.Class ? $": IHasImplementationId<{implementationsEnum}>, IHasStaticImplementationId<{implementationsEnum}>, IDiscovered" : ": IDiscovered")}
{{");
				
			if (definition.BaseTypeTypeKind == TypeKind.Class)
			{
				code.AppendLine($@"
	public new static {implementationsEnum} ImplementationId => new();
	public new virtual {implementationsEnum} GetImplementationId() => ImplementationId;");
			}
			
			code.Append($@"
}}");

			return code.ToString();
		}
		
		
		// Creates the partial enum record (or null if the enum has no members).
		string GetEnumRecord()
		{
			var code = new StringBuilder();

			// Create the comments on the enum record.
			code.Append($@"
/// <summary>
/// <list type=""bullet"">");
			
			foreach (var member in members)
			{
				var outlineSpaces = new String(' ', longestMemberNameLength - member.Name.Length);
				
				code.Append($@"
/// <item><c><![CDATA[ {member.Name}{outlineSpaces} = {member.Value ?? "?"} ]]></c></item>");
			}
			
			code.Append($@"
/// </list>
/// </summary>");
			
			code.Append($@"
{definition.Accessibility} partial record {definition.Name}{definition.TypeParameters} : ImplementationsEnum<{definition.Name}{definition.TypeParameters}, {definition.BaseTypeName}>
	{definition.BaseTypeGenericConstraints}
{{	
");

			// Add the discovered members to the enum record.
			foreach (var member in members)
			{
				// Create the comment on the enum member.
				if (member.Value is not null)
				{
					code.Append($@"
	/// <summary>");

					if (member.Value is not null)
					{
						code.Append($@"
	/// <c><![CDATA[ (value: {member.Value}) ]]></c>");
					}

					code.Append($@"
	/// </summary>");
				}

				// Create the enum member itself.
				var outlineSpaces = new String(' ', longestMemberNameLength - member.Name.Length);

				var typeName = member.TypeParameters is null && !member.IsConvertibleToConcreteType
					? $"global::{member.Namespace}.{member.Name}"
					: $"global::{definition.Namespace}.{definition.BaseTypeName}";

				code.Append(@$"
	{member.Accessibility} static {typeName} {member.Name} {{ get; }} {outlineSpaces}= ({typeName})CreateMember(new DiscoveredObject<{definition.BaseTypeName}>(typeof({member.Value}))).Value.UninitializedInstance;
");
			}

			code.Append($@"
}}
");

			return code.ToString();
		}


		string GetExtensionMethod() => $@"
/// <summary>
/// Call this method in order to create discovered enum members while invoking them (on the fly). So enum members are automatically deleted when not being used.
/// </summary>
{definition.Accessibility} static class {definition.Name}Extensions
{{
	public static {definition.Name}{definition.TypeParameters} {ImplementationDiscoverySourceGenerator.GenerateMethodName}{definition.TypeParameters}(this {definition.Name}{definition.TypeParameters} member, {definition.BaseTypeName}? value = null, string? comment = null) 
	{definition.BaseTypeGenericConstraints}
		=> member;

	public static IEnumerable<{definition.BaseTypeName}> GetDiscoveredObjects{definition.TypeParameters}(this {definition.Name}{definition.TypeParameters} implementationsEnum) 
	{definition.BaseTypeGenericConstraints}
		=> MagicEnumCore<{definition.Name}{definition.TypeParameters}, DiscoveredObject<{definition.BaseTypeName}>>.GetMembers().Select(member => member.Value.UninitializedInstance);

	#region ForwardInstanceMethodsToStatic 
	
	/// <inheritdoc cref=""MagicEnumCore{{{definition.BaseTypeName}, DiscoveredObject}}.GetDefaultValue""/>
	public static DiscoveredObject<{definition.BaseTypeName}> GetDefaultValue{definition.TypeParameters}(this {definition.Name}{definition.TypeParameters} implementationsEnum) 
	{definition.BaseTypeGenericConstraints}
		=> MagicEnumCore<{definition.Name}{definition.TypeParameters}, DiscoveredObject<{definition.BaseTypeName}>>.GetDefaultValue();
	
	/// <inheritdoc cref=""MagicEnumCore{{{definition.BaseTypeName}, DiscoveredObject}}.GetMemberCount""/>
	public static int GetMemberCount{definition.TypeParameters}(this {definition.Name}{definition.TypeParameters} implementationsEnum) 
	{definition.BaseTypeGenericConstraints}
		=> MagicEnumCore<{definition.Name}{definition.TypeParameters}, DiscoveredObject<{definition.BaseTypeName}>>.GetMemberCount();
	
	/// <inheritdoc cref=""MagicEnumCore{{{definition.BaseTypeName}, DiscoveredObject}}.GetUniqueValueCount""/>
	public static int GetUniqueValueCount{definition.TypeParameters}(this {definition.Name}{definition.TypeParameters} implementationsEnum) 
	{definition.BaseTypeGenericConstraints}
		=> MagicEnumCore<{definition.Name}{definition.TypeParameters}, DiscoveredObject<{definition.BaseTypeName}>>.GetUniqueValueCount();
	
	/// <inheritdoc cref=""MagicEnumCore{{{definition.BaseTypeName}, DiscoveredObject}}.GetMembers()""/>
	public static IEnumerable<IImplementationsEnum<{definition.BaseTypeName}>> GetMembers{definition.TypeParameters}(this {definition.Name}{definition.TypeParameters} implementationsEnum) 
	{definition.BaseTypeGenericConstraints}
		=> MagicEnumCore<{definition.Name}{definition.TypeParameters}, DiscoveredObject<{definition.BaseTypeName}>>.GetMembers();
	
	/// <inheritdoc cref=""MagicEnumCore{{{definition.BaseTypeName}, DiscoveredObject}}.GetValues()""/>
	public static IEnumerable<DiscoveredObject<{definition.BaseTypeName}>> GetValues{definition.TypeParameters}(this {definition.Name}{definition.TypeParameters} implementationsEnum) 
	{definition.BaseTypeGenericConstraints}
		=> MagicEnumCore<{definition.Name}{definition.TypeParameters}, DiscoveredObject<{definition.BaseTypeName}>>.GetValues();
	
	/// <inheritdoc cref=""MagicEnumCore{{{definition.BaseTypeName}, DiscoveredObject}}.TryGetSingleMember(string, out {definition.BaseTypeName})""/>
	public static bool TryGetSingleMember{definition.TypeParameters}(this {definition.Name}{definition.TypeParameters} implementationsEnum, string memberName, [NotNullWhen(true)] out IImplementationsEnum<{definition.BaseTypeName}>? member)
	{definition.BaseTypeGenericConstraints}
	{{
		if (!MagicEnumCore<{definition.Name}{definition.TypeParameters}, DiscoveredObject<{definition.BaseTypeName}>>.TryGetSingleMember(memberName, out {definition.Name}{definition.TypeParameters}? foundMember))
		{{
			member = null;
			return false;
		}}
	
		member = foundMember;
		return true;
	}}
	
	/// <inheritdoc cref=""MagicEnumCore{{{definition.BaseTypeName}, DiscoveredObject}}.GetSingleMember(string)""/>
	public static IImplementationsEnum<{definition.BaseTypeName}> GetSingleMember{definition.TypeParameters}(this {definition.Name}{definition.TypeParameters} implementationsEnum, string memberName) 
	{definition.BaseTypeGenericConstraints}
		=> MagicEnumCore<{definition.Name}{definition.TypeParameters}, DiscoveredObject<{definition.BaseTypeName}>>.GetSingleMember(memberName);
	
	/// <inheritdoc cref=""MagicEnumCore{{{definition.BaseTypeName}, DiscoveredObject}}.TryGetSingleMember(DiscoveredObject, out {definition.BaseTypeName}?)""/>
	public static bool TryGetSingleMember{definition.TypeParameters}(DiscoveredObject<{definition.BaseTypeName}> memberValue, [NotNullWhen(true)] out IImplementationsEnum<{definition.BaseTypeName}>? member)
	{definition.BaseTypeGenericConstraints}
	{{
		if (!MagicEnumCore<{definition.Name}{definition.TypeParameters}, DiscoveredObject<{definition.BaseTypeName}>>.TryGetSingleMember(memberValue, out {definition.Name}{definition.TypeParameters}? foundMember))
		{{
			member = null;
			return false;
		}}
	
		member = foundMember;
		return true;
	}}
	
	/// <inheritdoc cref=""MagicEnumCore{{{definition.BaseTypeName}, DiscoveredObject}}.GetSingleMember(DiscoveredObject)""/>
	public static IImplementationsEnum<{definition.BaseTypeName}> GetSingleMember{definition.TypeParameters}(DiscoveredObject<{definition.BaseTypeName}> memberValue) 
	{definition.BaseTypeGenericConstraints}
		=> MagicEnumCore<{definition.Name}{definition.TypeParameters}, DiscoveredObject<{definition.BaseTypeName}>>.GetSingleMember(memberValue);
	
	/// <inheritdoc cref=""MagicEnumCore{{{definition.BaseTypeName}, DiscoveredObject}}.TryGetMembers(DiscoveredObject, out IReadOnlyCollection{{{definition.BaseTypeName}}}?)""/>
	public static bool TryGetMembers{definition.TypeParameters}(this {definition.Name}{definition.TypeParameters} implementationsEnum, DiscoveredObject<{definition.BaseTypeName}> memberValue, [NotNullWhen(true)] out IReadOnlyCollection<IImplementationsEnum<{definition.BaseTypeName}>>? members)
	{definition.BaseTypeGenericConstraints}
	{{
		if (!MagicEnumCore<{definition.Name}{definition.TypeParameters}, DiscoveredObject<{definition.BaseTypeName}>>.TryGetMembers(memberValue, out IReadOnlyCollection<{definition.Name}{definition.TypeParameters}>? foundMembers))
		{{
			members = null;
			return false;
		}}
	
		members = foundMembers;
		return true;
	}}
	
	/// <inheritdoc cref=""MagicEnumCore{{{definition.BaseTypeName}, DiscoveredObject}}.GetMembers(DiscoveredObject)""/>
	public static IEnumerable<IImplementationsEnum<{definition.BaseTypeName}>> GetMembers{definition.TypeParameters}(this {definition.Name}{definition.TypeParameters} implementationsEnum, DiscoveredObject<{definition.BaseTypeName}> memberValue) 
	{definition.BaseTypeGenericConstraints}
		=> MagicEnumCore<{definition.Name}{definition.TypeParameters}, DiscoveredObject<{definition.BaseTypeName}>>.GetMembers(memberValue);
	
	#endregion
}}
";
	}
}