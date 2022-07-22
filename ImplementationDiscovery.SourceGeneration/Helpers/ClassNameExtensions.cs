namespace CodeChops.ImplementationDiscovery.SourceGeneration.Helpers;

public static class ClassNameExtensions
{
	public static string GetClassNameWithoutGenerics(this string className)
	{
		var genericParameterIndex = className.IndexOf('<');
		var classNameWithoutGenerics = genericParameterIndex <= 0 ? className : className.Substring(0, genericParameterIndex);

		return classNameWithoutGenerics;
	}

	public static bool HasGenericParameter(this string className)
	{
		var hasGenericParameter = className.Contains('<');
		return hasGenericParameter;
	}
}