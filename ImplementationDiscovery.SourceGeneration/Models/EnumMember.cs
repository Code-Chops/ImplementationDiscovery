﻿namespace CodeChops.ImplementationDiscovery.SourceGeneration.Models;

internal record EnumMember : IEnumModel
{
	/// <summary>
	/// The enum full name.
	/// </summary>
	public string EnumIdentifier { get; }
	public string Name { get; }
	public object? Value { get; }
	public string? Comment { get; }

	/// <param name="enumIdentifier">Be aware of global namespaces!</param>
	public EnumMember(string enumIdentifier, string name, object? value = null, string? comment = null)
	{
		this.EnumIdentifier = String.IsNullOrWhiteSpace(enumIdentifier) ? throw new ArgumentNullException(nameof(enumIdentifier)) : enumIdentifier;
		this.Name = String.IsNullOrWhiteSpace(name) ? throw new ArgumentNullException(nameof(name)) : name;
		this.Value = value;
		this.Comment = String.IsNullOrWhiteSpace(comment) ? null : comment;
	}

	public EnumMember(string enumIdentifier, AttributeData data)
	{
		this.EnumIdentifier = String.IsNullOrWhiteSpace(enumIdentifier) ? throw new ArgumentNullException(nameof(enumIdentifier)) : enumIdentifier;
		
		if (!data.TryGetArguments(out var arguments))
		{
			throw new Exception($"Could not retrieve attribute parameters of attribute {data.AttributeClass?.Name}.");
		}

		this.Name = (string)arguments![nameof(this.Name)].Value!;

		this.Value = arguments.TryGetValue(nameof(this.Value), out var valueArgument) 
			? valueArgument.Type?.ToDisplayString() == "System.String" ? $"\"{valueArgument.Value}\"" : valueArgument.Value
			: null;

		this.Comment = (string?)(arguments.TryGetValue(nameof(this.Comment), out var commentArgument) ? commentArgument.Value : null);
	}
}