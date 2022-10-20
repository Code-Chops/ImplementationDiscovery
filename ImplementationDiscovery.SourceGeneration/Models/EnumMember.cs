using System.Text.RegularExpressions;

namespace CodeChops.ImplementationDiscovery.SourceGeneration.Models;

internal record EnumMember : IEnumModel
{
	private static Regex IsValidName { get; } = new(@"^[a-zA-Z_]\w*(\.[a-zA-Z_]\w*)*$");
	
	/// <summary>
	/// The enum full name.
	/// </summary>
	public string EnumIdentifier { get; }
	private string Name { get; }
	public string? Value { get; }

	public EnumMember(string enumIdentifier, string name, string? value = null)
	{
		this.EnumIdentifier = String.IsNullOrWhiteSpace(enumIdentifier) ? throw new ArgumentNullException(nameof(enumIdentifier)) : enumIdentifier;
		this.Name = String.IsNullOrWhiteSpace(name) ? throw new ArgumentNullException(nameof(name)) : name;
		this.Value = value;
	}

	public string GetClassName() => this.Name;

	public string GetSimpleName(EnumDefinition definition)
	{
		if (definition.BaseTypeDeclaration is null) return this.Name;
		
		var name = this.Name;

		if (name.StartsWith(definition.BaseTypeNameIncludingGenerics))
			name = name.Substring(definition.BaseTypeNameIncludingGenerics.Length);
		
		if (name.EndsWith(definition.BaseTypeNameIncludingGenerics))
			name = name.Substring(0, name.Length - definition.BaseTypeNameIncludingGenerics.Length);
		
		return IsValidName.IsMatch(name) ? name : this.Name;
	}
}