using Aspose.Words;

public class Parser
{
    public string[] Parse(Document document)
    {
        var headerFooters = GetHeaderFooters(document);
        if (headerFooters.Any())
        {
            return headerFooters;
        }

        var footnotes = document
            .GetChildNodes(NodeType.Footnote, true)
            .Select(x => x.GetText())
            .Select(ReplaceControlChars)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToArray();

        return footnotes;
    }

    private string[] GetHeaderFooters(Document document)
    {
        return document.GetChildNodes(NodeType.HeaderFooter, true)
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