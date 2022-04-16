using Aspose.Words;
using Aspose.Words.Notes;

public class Parser
{
    public string[] Parse(Document document)
    {
        var result = new List<string>();
        var headerFooters = document
            .GetChildNodes(NodeType.HeaderFooter, true)
            .Select(x => x.GetText())
            .Select(ReplaceControlChars)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();

        result.AddRange(headerFooters);
            
        if (!headerFooters.Any())
        {
            var footnotes = document
                .GetChildNodes(NodeType.Footnote, true)
                .Select(x => x.GetText())
                .Select(ReplaceControlChars)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            result.AddRange(footnotes);
        }

        return result.ToArray();
    }
    
    private string ReplaceControlChars(string value)
    {
        return value
            .Replace("\x02", string.Empty)
            .Replace(" ", string.Empty)
            .Replace("\r", string.Empty);
    }
}