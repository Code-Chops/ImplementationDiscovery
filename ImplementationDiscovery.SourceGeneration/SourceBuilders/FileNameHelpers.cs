namespace CodeChops.ImplementationDiscovery.SourceGeneration.SourceBuilders;

internal static class FileNameHelpers
{
    public static string GetValidFileName(string definitionName)
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
}