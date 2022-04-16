using Aspose.Words;

public class Parser
{
    public string[] Parse(Document document)
    {
        var result = document
            .Sections[0]
            .Select(x => x.GetText().ReplaceLineEndings(string.Empty))
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToArray();

        return result;
    }
}