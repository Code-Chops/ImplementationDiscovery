namespace CodeChops.ImplementationDiscovery.SourceGeneration.Models;

internal record EnumMember : IEnumModel
{
	/// <summary>
	/// The enum full name.
	/// </summary>
	public string EnumIdentifier { get; }
	public string Name { get; }
	public object? Value { get; }

	public EnumMember(string enumIdentifier, string name, object? value = null)
	{
		this.EnumIdentifier = String.IsNullOrWhiteSpace(enumIdentifier) ? throw new ArgumentNullException(nameof(enumIdentifier)) : enumIdentifier;
		this.Name = String.IsNullOrWhiteSpace(name) ? throw new ArgumentNullException(nameof(name)) : name;
		this.Value = value;
	}
}