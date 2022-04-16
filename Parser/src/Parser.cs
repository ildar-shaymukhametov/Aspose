using Aspose.Words;

public class Parser
{
    public string[] Parse(Document document)
    {
        if (document == null)
        {
            throw new ArgumentNullException(nameof(document));
        }

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

        return GetSectionsFirstParagraphText(document);
    }

    private string[] GetNodesText(Document document, NodeType type)
    {
        return document.GetChildNodes(type, true)
            .Select(x => x.GetText())
            .Select(ReplaceControlChars)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToArray();
    }

    private string[] GetSectionsFirstParagraphText(Document document)
    {
        var result = document.GetChildNodes(NodeType.Paragraph, true)
            .Cast<Paragraph>()
            .GroupBy(x => x.ParentSection)
            .Select(x => x.First())
            .Select(x => x.GetText())
            .Select(ReplaceControlChars)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToArray();
            
        return result;
    }

    private string ReplaceControlChars(string value)
    {
        return value
            .Replace("\x02", string.Empty)
            .Replace(" ", string.Empty)
            .Replace("\r", string.Empty);
    }
}