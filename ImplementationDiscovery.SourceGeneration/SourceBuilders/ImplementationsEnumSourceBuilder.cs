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
		
		catch (Exception e)
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
		code.AppendLine($@"// <auto-generated />
#nullable enable
#pragma warning disable CS0109
");
		
		code.AppendLine(GetUsings);
		code.AppendLine(GetNamespaceDeclaration);
		code.AppendLine(GetImplementationIdProperty);
		code.AppendLine(GetEnumRecord);

		code.Append(@"
#nullable restore
");

		var enumCodeFileName = FileNameHelpers.GetFileName($"{definition.Namespace}.{definition.Name}", configOptionsProvider);
		context.AddSource(enumCodeFileName, SourceText.From(code.ToString(), Encoding.UTF8));
		return;

		
		string GetUsings()
		{
			var usings = definition.Usings.Concat(new[]
			{
				"using System.Runtime.CompilerServices;",
				"using System;", 
				"using CodeChops.ImplementationDiscovery;", 
				"using CodeChops.MagicEnums;",
				"using CodeChops.MagicEnums.Core;", 
				"using System.Diagnostics.CodeAnalysis;",
				"using System.Reflection;",
			});
			
			return usings.Distinct().OrderBy(u => u).Aggregate(new StringBuilder(), (sb, u) => sb.AppendLine(u)).ToString();
		}

		
		// Creates the namespace definition of the location of the enum definition (or null if the namespace is not defined).
		string? GetNamespaceDeclaration()
		{
			if (definition.Namespace is null) 
				return null;

			var code = $@"namespace {definition.Namespace};";
			return code;
		}


		string? GetImplementationIdProperty()
		{
			if (definition.BaseTypeDeclaration is null || !definition.IsPartial || definition.IsProxy) 
				return null;

			var code = new StringBuilder();

			if (!definition.BaseTypeHasComments)
			{
				code.AppendLine(@$"
/// <summary>
/// Discovered implementations: <see cref=""{definition.EnumIdentifier.Replace('<', '{').Replace('>', '}')}""/>.
/// </summary>
");
			}

			code.TrimEnd().Append($@"
{definition.BaseTypeDeclaration} {definition.BaseTypeNameIncludingGenerics} {(definition is { GenerateImplementationIds: true, BaseTypeTypeKind: TypeKind.Class } ? $": IHasImplementationId<{definition.BaseTypeNameIncludingGenerics}>" : null)}
{{
	public static IImplementationsEnum<{definition.BaseTypeNameIncludingGenerics}> ImplementationEnum {{ get; }} = new {definition.Name}();
");
			
			if (definition is { GenerateImplementationIds: true, BaseTypeTypeKind: TypeKind.Class })
			{
				code.AppendLine($@"
	public abstract IImplementationsEnum<{definition.BaseTypeNameIncludingGenerics}> GetImplementationId();
");
			}

			code.TrimEnd().Append($@"
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
/// Discovered implementations for <see cref=""{definition.BaseTypeNameIncludingGenerics.Replace('<', '{').Replace('>', '}')}""/>:
/// <list type=""table"">
");
			
			foreach (var member in members)
			{
				code.TrimEnd().Append($@"
/// <item><see cref=""{member.Value.Replace('<', '{').Replace('>', '}')}""/></item>
");
			}
			
			code.TrimEnd().Append($@"
/// </list>
/// </summary>");

			var baseType = definition.IsProxy
				? $"{definition.ExternalDefinition!.Namespace}.{definition.ExternalDefinition.Name}"
				: $"ImplementationsEnum<{definition.Name}, {definition.BaseTypeNameIncludingGenerics}>";
			
			var originalDefinition = definition.ExternalDefinition ?? definition;

			code.Append($@"
{definition.Accessibility} partial record {definition.Name} : {baseType}, IInitializable
	");

			if (definition.BaseTypeGenericConstraints is not null)
				code.Append(definition.ExternalDefinition?.BaseTypeGenericConstraints ?? definition.BaseTypeGenericConstraints).TrimEnd();
			
			code.TrimEnd().AppendLine(@"
{
").TrimEnd();

			// Add the discovered members to the enum record.
			foreach (var member in members)
			{
				code.Append($@"
	/// <summary>
");

				code.TrimEnd().Append($@"
	/// <see cref=""{member.Value.Replace('<', '{').Replace('>', '}')}""/>
");

				code.TrimEnd().Append($@"
	/// </summary>");

				// Create the enum member itself.
				var outlineSpaces = new String(' ', longestMemberNameLength - member.GetSimpleName(definition).Length);

				var creation = GetCreation(member, definition);
				
				code.Append(@$"
	{member.Accessibility} static {originalDefinition.Name} {NameHelpers.GetNameWithoutGenerics(member.GetSimpleName(definition))} {outlineSpaces}{{ get; }} = CreateMember({creation});
");
			}

			if (definition.IsProxy)
			{
				code.AppendLine($@"
	#region Forwarding
	/// <inheritdoc cref=""ImplementationsEnum{{TSelf, TValue}}.CreateMember""/>
	protected new static {originalDefinition.Name} CreateMember(
		DiscoveredObject<{definition.BaseTypeNameIncludingGenerics}> value,
		Func<{originalDefinition.Name}>? memberCreator = null,
		[CallerMemberName] string? name = null)
		=> {originalDefinition.Name}.CreateMember(value: value, memberCreator: memberCreator, name: name);
	
	/// <inheritdoc cref=""ImplementationsEnum{{TSelf, TValue}}.CreateMember{{TMember}}""/>
	protected new static {originalDefinition.Name} CreateMember<TMember>(
		DiscoveredObject<{definition.BaseTypeNameIncludingGenerics}> value,
		Func<TMember>? memberCreator = null,
		[CallerMemberName] string? name = null)
		where TMember : {originalDefinition.Name}
		=> {originalDefinition.Name}.CreateMember<TMember>(value, memberCreator, name);
	
	/// <inheritdoc cref=""ImplementationsEnum{{TSelf, TValue}}.CreateMember{{TMember}}""/>
	protected new static {originalDefinition.Name} CreateMember<TMember>(
		Func<DiscoveredObject<{definition.BaseTypeNameIncludingGenerics}>> valueCreator,
		Func<TMember>? memberCreator = null,
		[CallerMemberName] string? name = null)
		where TMember : {originalDefinition.Name}
		=> {originalDefinition.Name}.CreateMember<TMember>(valueCreator, memberCreator, name);

	/// <inheritdoc cref=""ImplementationsEnum{{TSelf, TValue}}.GetOrCreateMember""/>
	public new static {originalDefinition.Name} GetOrCreateMember(DiscoveredObject<{definition.BaseTypeNameIncludingGenerics}> value) 
		=> {originalDefinition.Name}.GetOrCreateMember(value);

	/// <inheritdoc cref=""ImplementationsEnum{{TSelf, TValue}}.GetOrCreateMember""/>
	protected new static {originalDefinition.Name} GetOrCreateMember(
		string? name, 
		DiscoveredObject<{definition.BaseTypeNameIncludingGenerics}> value, 
		Func<{originalDefinition.Name}>? memberCreator = null)
		=> {originalDefinition.Name}.GetOrCreateMember(name: name, value: value, memberCreator: memberCreator);

	/// <inheritdoc cref=""ImplementationsEnum{{TSelf, TValue}}.GetOrCreateMember{{TMember}}""/>
	protected new static {originalDefinition.Name} GetOrCreateMember<TMember>(
		[CallerMemberName] string? name = null, 
		Func<DiscoveredObject<{definition.BaseTypeNameIncludingGenerics}>>? valueCreator = null, 
		Func<TMember>? memberCreator = null)
		where TMember : {originalDefinition.Name}
		=> {originalDefinition.Name}.GetOrCreateMember(name, valueCreator, memberCreator);
	
	/// <inheritdoc cref=""ImplementationsEnum{{TSelf, TValue}}.TryGetSingleMember(string, out DiscoveredObject{{TBaseType}})""/> 
	public new static bool TryGetSingleMember(string memberName, [NotNullWhen(true)] out {originalDefinition.Name}? member)
		=> {originalDefinition.Name}.TryGetSingleMember(memberName, out member);
	
	/// <inheritdoc cref=""ImplementationsEnum{{TSelf, TValue}}.GetSingleMember(string)""/> 
	public new static {originalDefinition.Name} GetSingleMember(string memberName)
		=> {originalDefinition.Name}.GetSingleMember(memberName);
	
	/// <inheritdoc cref=""ImplementationsEnum{{TSelf, TValue}}.TryGetSingleMember(DiscoveredObject{{TBaseType}}, out TSelf)""/> 
	public new static bool TryGetSingleMember(DiscoveredObject<{definition.BaseTypeNameIncludingGenerics}> memberValue, [NotNullWhen(true)] out {originalDefinition.Name}? member)
		=> {originalDefinition.Name}.TryGetSingleMember(memberValue, out member);

	/// <inheritdoc cref=""ImplementationsEnum{{TSelf, TValue}}.GetSingleMember(DiscoveredObject{{TBaseType}})""/> 
	public new static {originalDefinition.Name} GetSingleMember(DiscoveredObject<{definition.BaseTypeNameIncludingGenerics}> memberValue)
		=> {originalDefinition.Name}.GetSingleMember(memberValue);
	
	/// <inheritdoc cref=""ImplementationsEnum{{TSelf, TValue}}.TryGetMembers(DiscoveredObject{{TBaseType}}, out IReadOnlyCollection{{TSelf}}?)""/> 
	public new static bool TryGetMembers(DiscoveredObject<{definition.BaseTypeNameIncludingGenerics}> memberValue, [NotNullWhen(true)] out IReadOnlyCollection<{originalDefinition.Name}>? members)
		=> {originalDefinition.Name}.TryGetMembers(memberValue, out members);
	
	/// <inheritdoc cref=""ImplementationsEnum{{TSelf, TValue}}.GetMembers(DiscoveredObject{{TBaseType}})""/> 
	public new static IEnumerable<{originalDefinition.Name}> GetMembers(DiscoveredObject<{definition.BaseTypeNameIncludingGenerics}> memberValue)
		=> {originalDefinition.Name}.GetMembers(memberValue);
	#endregion
");
			}
			
			code.TrimEnd().AppendLine().AppendLine(@$"
	#region Initialization
	/// <summary>
	/// Is false when the enum is still in static buildup and true if this is finished.
	/// This parameter can be used to detect cyclic references during buildup and act accordingly.
	/// </summary>
	public new static bool IsInitialized {{ get; }}

	static {NameHelpers.GetNameWithoutGenerics(definition.Name)}()
	{{
		IsInitialized = true;		
	}}
	#endregion
}}
");

			return code.TrimEnd().ToString();
		}


		string GetCreation(DiscoveredEnumMember member, EnumDefinition definition)
			=> member.InstanceCreationMethod switch
			{
				InstanceCreationMethod.Factory			=> $"new DiscoveredObject<{definition.BaseTypeNameIncludingGenerics}>(static () => {member.Value}.Create())",
				InstanceCreationMethod.New				=> $"new DiscoveredObject<{definition.BaseTypeNameIncludingGenerics}>(static () => new {member.Value}())",
				InstanceCreationMethod.Uninitialized	=> $"new DiscoveredObject<{definition.BaseTypeNameIncludingGenerics}>(typeof({member.Value}))",
				_ => throw new ArgumentOutOfRangeException(nameof(member))
			};
	}
}