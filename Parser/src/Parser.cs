using Aspose.Words;

public class Parser
{
    public string[] Parse(Document document)
    {
        var headerFooters = GetNodesText(document, NodeType.HeaderFooter);
        if (headerFooters.Any())
        {
            return headerFooters;
        }

        var footnotes = GetNodesText(document, NodeType.Footnote);
        if (footnotes.Any())
        {
            return footnotes;
        }

        return GetNodesText(document, NodeType.Paragraph);
    }

    private string[] GetNodesText(Document document, NodeType type)
    {
        return document.GetChildNodes(type, true)
            .Select(x => x.GetText())
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