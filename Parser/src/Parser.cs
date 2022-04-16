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
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();

        var footnotes = document
            .Sections[0]
            .GetChildNodes(NodeType.Footnote, true)
            .Cast<Footnote>()
            .Select(x => x.GetText())
            .ToList();

        result.AddRange(footnotes);

        return result.Select(ReplaceControlChars).ToArray();
    }
    
    private string ReplaceControlChars(string value)
    {
        return value
            .Replace(ControlChar.SpaceChar.ToString(), string.Empty)
            .Replace(ControlChar.Cr, string.Empty);
    }
}