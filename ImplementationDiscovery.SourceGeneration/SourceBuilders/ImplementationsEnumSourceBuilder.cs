﻿using System.Text;
using CodeChops.ImplementationDiscovery.SourceGeneration.Models;

namespace CodeChops.ImplementationDiscovery.SourceGeneration.SourceBuilders;

internal static class ImplementationsEnumSourceBuilder
{
	/// <summary>
	/// Creates a partial record of the enum definition which includes the discovered enum members. It also generates an extension class for the explicit enum definitions.
	/// </summary>
	public static void CreateSource(SourceProductionContext context, List<DiscoveredEnum> discoveredEnums, AnalyzerConfigOptionsProvider configOptionsProvider)
	{
		if (discoveredEnums.Count == 0) return;
		
		try
		{
			foreach (var discoveredEnum in discoveredEnums)
				CreateEnumFile(context, discoveredEnum, configOptionsProvider);
		}
		
#pragma warning disable CS0168
		catch (Exception e)
#pragma warning restore CS0168
        {
	        var descriptor = new DiagnosticDescriptor(nameof(ImplementationsEnumSourceBuilder), "Error", $"{nameof(ImplementationsEnumSourceBuilder)} failed to generate due to an error. Please inform CodeChops (www.CodeChops.nl). Error: {e}", "Compilation", DiagnosticSeverity.Error, isEnabledByDefault: true);
	        context.ReportDiagnostic(Diagnostic.Create(descriptor, null));
	        
	        context.AddSource($"{nameof(ImplementationsEnumSourceBuilder)}_Exception_{Guid.NewGuid()}", SourceText.From($"/*{e}*/", Encoding.UTF8));
        }		
	}
	
	private static void CreateEnumFile(SourceProductionContext context, DiscoveredEnum discoveredEnum, AnalyzerConfigOptionsProvider configOptionsProvider)
	{
		var code = new StringBuilder();

		var definition = discoveredEnum.Definition;
		
		// Place the members that are discovered in the enum definition file itself first. The order can be relevant because the value of enum members can be implicitly incremental.
		// Do a distinct on the file path and line position so the members will be deduplicated while typing their invocation.
		// Also do a distinct on the member name.		
		var members = discoveredEnum.Members
			.OrderByDescending(member => member.FilePath == definition.FilePath)
			.GroupBy(member => (member.FilePath, member.LinePosition))
			.Select(group => group.First())
			.GroupBy(member => member.GetSimpleName(definition))
			.Select(membersByName => membersByName.First())
			.ToList();

		// Is used for correct enum member outlining.
		var longestMemberNameLength = members
			.Select(member => member.GetSimpleName(definition))
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
				"using System.Diagnostics.CodeAnalysis;",
				"using System.Reflection;",
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
			if (definition.BaseTypeDeclaration is null || !definition.IsPartial || definition.ExternalDefinition is not null) return null;

			var code = new StringBuilder();

			var implementationsEnum = $"{definition.Name}{definition.TypeParameters}";
			
			code.AppendLine($@"
{definition.BaseTypeDeclaration} {definition.BaseTypeNameIncludingGenerics} {(definition.BaseTypeTypeKind == TypeKind.Class ? $": IHasImplementationId<{implementationsEnum}>, IHasStaticImplementationId<{implementationsEnum}>" : null)}
{{");
				
			if (definition.BaseTypeTypeKind == TypeKind.Class)
			{
				code.AppendLine($@"
	public new static {implementationsEnum} ImplementationId {{ get; }} = new();
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
				var outlineSpaces = new String(' ', longestMemberNameLength - member.GetSimpleName(definition).Length);
				
				code.Append($@"
/// <item><c><![CDATA[ {member.GetSimpleName(definition)}{outlineSpaces} = {member.Value ?? "?"} ]]></c></item>");
			}
			
			code.Append($@"
/// </list>
/// </summary>");

			var baseType = definition.ExternalDefinition is not null
				? $"{definition.ExternalDefinition.Namespace}.{definition.ExternalDefinition.Name}{definition.ExternalDefinition.TypeParameters}"
				: $"ImplementationsEnum<{definition.Name}{definition.TypeParameters}, {definition.BaseTypeNameIncludingGenerics}>";
			
			var concreteDefinition = definition.ExternalDefinition ?? definition;
			
			code.Append($@"
{definition.Accessibility} partial record {definition.Name}{definition.TypeParameters} : {baseType}
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
				var outlineSpaces = new String(' ', longestMemberNameLength - member.GetSimpleName(definition).Length);

				code.Append(@$"
	{member.Accessibility} static {concreteDefinition.Name}{concreteDefinition.TypeParameters} {member.GetSimpleName(definition)} {{ get; }} {outlineSpaces}= CreateMember(new DiscoveredObject<{definition.BaseTypeNameIncludingGenerics}>(typeof({member.Value})));
");
			}

			code.AppendLine($@"
	public static new bool IsInitialized {{ get; }}");
				
			code.Append($@"
	static {definition.Name}()
	{{
		foreach (var property in typeof({definition.Name}{definition.TypeParameters}).GetProperties(BindingFlags.Public | BindingFlags.Static))
			property.GetGetMethod()!.Invoke(obj: null, parameters: null);

		IsInitialized = true;
	}}
}}
");

			return code.ToString();
		}


		string? GetExtensionMethod()
		{
			if (definition.ExternalDefinition is not null)
				return null;

			var commentName = definition.BaseTypeNameIncludingGenerics.Replace('<', '{').Replace('>', '}');
			
			return $@"
{definition.Accessibility} static class {definition.Name}Extensions
{{
	public static IEnumerable<{definition.BaseTypeNameIncludingGenerics}> GetDiscoveredObjects{definition.TypeParameters}(this {definition.Name}{definition.TypeParameters} implementationsEnum) 
	{definition.BaseTypeGenericConstraints}
		=> MagicEnumCore<{definition.Name}{definition.TypeParameters}, DiscoveredObject<{definition.BaseTypeNameIncludingGenerics}>>.GetMembers().Select(member => member.UninitializedInstance);

	#region ForwardInstanceMethodsToStatic 
	
	/// <inheritdoc cref=""MagicEnumCore{{{commentName}, DiscoveredObject}}.GetDefaultValue""/>
	public static DiscoveredObject<{definition.BaseTypeNameIncludingGenerics}> GetDefaultValue{definition.TypeParameters}(this {definition.Name}{definition.TypeParameters} implementationsEnum) 
	{definition.BaseTypeGenericConstraints}
		=> MagicEnumCore<{definition.Name}{definition.TypeParameters}, DiscoveredObject<{definition.BaseTypeNameIncludingGenerics}>>.GetDefaultValue();
	
	/// <inheritdoc cref=""MagicEnumCore{{{commentName}, DiscoveredObject}}.GetMemberCount""/>
	public static int GetMemberCount{definition.TypeParameters}(this {definition.Name}{definition.TypeParameters} implementationsEnum) 
	{definition.BaseTypeGenericConstraints}
		=> MagicEnumCore<{definition.Name}{definition.TypeParameters}, DiscoveredObject<{definition.BaseTypeNameIncludingGenerics}>>.GetMemberCount();
	
	/// <inheritdoc cref=""MagicEnumCore{{{commentName}, DiscoveredObject}}.GetUniqueValueCount""/>
	public static int GetUniqueValueCount{definition.TypeParameters}(this {definition.Name}{definition.TypeParameters} implementationsEnum) 
	{definition.BaseTypeGenericConstraints}
		=> MagicEnumCore<{definition.Name}{definition.TypeParameters}, DiscoveredObject<{definition.BaseTypeNameIncludingGenerics}>>.GetUniqueValueCount();
	
	/// <inheritdoc cref=""MagicEnumCore{{{commentName}, DiscoveredObject}}.GetMembers()""/>
	public static IEnumerable<IImplementationsEnum<{definition.BaseTypeNameIncludingGenerics}>> GetMembers{definition.TypeParameters}(this {definition.Name}{definition.TypeParameters} implementationsEnum) 
	{definition.BaseTypeGenericConstraints}
		=> MagicEnumCore<{definition.Name}{definition.TypeParameters}, DiscoveredObject<{definition.BaseTypeNameIncludingGenerics}>>.GetMembers();
	
	/// <inheritdoc cref=""MagicEnumCore{{{commentName}, DiscoveredObject}}.GetValues()""/>
	public static IEnumerable<DiscoveredObject<{definition.BaseTypeNameIncludingGenerics}>> GetValues{definition.TypeParameters}(this {definition.Name}{definition.TypeParameters} implementationsEnum) 
	{definition.BaseTypeGenericConstraints}
		=> MagicEnumCore<{definition.Name}{definition.TypeParameters}, DiscoveredObject<{definition.BaseTypeNameIncludingGenerics}>>.GetValues();
	
	/// <inheritdoc cref=""MagicEnumCore{{{commentName}, DiscoveredObject}}.TryGetSingleMember(string, out {commentName})""/>
	public static bool TryGetSingleMember{definition.TypeParameters}(this {definition.Name}{definition.TypeParameters} implementationsEnum, string memberName, [NotNullWhen(true)] out IImplementationsEnum<{definition.BaseTypeNameIncludingGenerics}>? member)
	{definition.BaseTypeGenericConstraints}
	{{
		if (!MagicEnumCore<{definition.Name}{definition.TypeParameters}, DiscoveredObject<{definition.BaseTypeNameIncludingGenerics}>>.TryGetSingleMember(memberName, out var foundMember))
		{{
			member = null;
			return false;
		}}
	
		member = foundMember;
		return true;
	}}
	
	/// <inheritdoc cref=""MagicEnumCore{{{commentName}, DiscoveredObject}}.GetSingleMember(string)""/>
	public static IImplementationsEnum<{definition.BaseTypeNameIncludingGenerics}> GetSingleMember{definition.TypeParameters}(this {definition.Name}{definition.TypeParameters} implementationsEnum, string memberName) 
	{definition.BaseTypeGenericConstraints}
		=> MagicEnumCore<{definition.Name}{definition.TypeParameters}, DiscoveredObject<{definition.BaseTypeNameIncludingGenerics}>>.GetSingleMember(memberName);
	
	/// <inheritdoc cref=""MagicEnumCore{{{commentName}, DiscoveredObject}}.TryGetSingleMember(DiscoveredObject, out {commentName}?)""/>
	public static bool TryGetSingleMember{definition.TypeParameters}(DiscoveredObject<{definition.BaseTypeNameIncludingGenerics}> memberValue, [NotNullWhen(true)] out IImplementationsEnum<{definition.BaseTypeNameIncludingGenerics}>? member)
	{definition.BaseTypeGenericConstraints}
	{{
		if (!MagicEnumCore<{definition.Name}{definition.TypeParameters}, DiscoveredObject<{definition.BaseTypeNameIncludingGenerics}>>.TryGetSingleMember(memberValue, out var foundMember))
		{{
			member = null;
			return false;
		}}
	
		member = foundMember;
		return true;
	}}
	
	/// <inheritdoc cref=""MagicEnumCore{{{commentName}, DiscoveredObject}}.GetSingleMember(DiscoveredObject)""/>
	public static IImplementationsEnum<{definition.BaseTypeNameIncludingGenerics}> GetSingleMember{definition.TypeParameters}(DiscoveredObject<{definition.BaseTypeNameIncludingGenerics}> memberValue) 
	{definition.BaseTypeGenericConstraints}
		=> MagicEnumCore<{definition.Name}{definition.TypeParameters}, DiscoveredObject<{definition.BaseTypeNameIncludingGenerics}>>.GetSingleMember(memberValue);
	
	/// <inheritdoc cref=""MagicEnumCore{{{commentName}, DiscoveredObject}}.TryGetMembers(DiscoveredObject, out IReadOnlyCollection{{{commentName}}}?)""/>
	public static bool TryGetMembers{definition.TypeParameters}(this {definition.Name}{definition.TypeParameters} implementationsEnum, DiscoveredObject<{definition.BaseTypeNameIncludingGenerics}> memberValue, [NotNullWhen(true)] out IReadOnlyCollection<IImplementationsEnum<{definition.BaseTypeNameIncludingGenerics}>>? members)
	{definition.BaseTypeGenericConstraints}
	{{
		if (!MagicEnumCore<{definition.Name}{definition.TypeParameters}, DiscoveredObject<{definition.BaseTypeNameIncludingGenerics}>>.TryGetMembers(memberValue, out var foundMembers))
		{{
			members = null;
			return false;
		}}
	
		members = foundMembers;
		return true;
	}}
	
	/// <inheritdoc cref=""MagicEnumCore{{{commentName}, DiscoveredObject}}.GetMembers(DiscoveredObject)""/>
	public static IEnumerable<IImplementationsEnum<{definition.BaseTypeNameIncludingGenerics}>> GetMembers{definition.TypeParameters}(this {definition.Name}{definition.TypeParameters} implementationsEnum, DiscoveredObject<{definition.BaseTypeNameIncludingGenerics}> memberValue) 
	{definition.BaseTypeGenericConstraints}
		=> MagicEnumCore<{definition.Name}{definition.TypeParameters}, DiscoveredObject<{definition.BaseTypeNameIncludingGenerics}>>.GetMembers(memberValue);
	
	#endregion
}}
";
		}
	}
}