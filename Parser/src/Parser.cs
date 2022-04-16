using Aspose.Words;
using Aspose.Words.Notes;

public class Parser
{
    public string[] Parse(Document document)
    {
        var result = document
            .Sections
            .SelectMany(x => ((Section)x).HeadersFooters)
            .Select(x => x.GetText())
            .ToList();

        var footnotes = document
            .GetChildNodes(NodeType.Footnote, true)
            .Cast<Footnote>()
            .Select(x => x.GetText())
            .ToList();

        result.AddRange(footnotes);

        return result
            .Select(ReplaceControlChars)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToArray();
    }
    
    private string ReplaceControlChars(string value)
    {
        return value
            .Replace("\x02", string.Empty)
            .Replace(" ", string.Empty)
            .Replace("\r", string.Empty);
    }
}