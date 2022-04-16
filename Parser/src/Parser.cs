using Aspose.Words;

public class Parser
{
    public string[] Parse(Document document)
    {
        var result = document
            .Sections
            .SelectMany(x => ((Section)x).HeadersFooters)
            .Select(x => x.GetText().ReplaceLineEndings(string.Empty))
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToArray();

        return result;
    }
}