using System.Linq;
using Aspose.Words;
using Xunit;

namespace tests;

public class ParserTests
{
    [Fact]
    public void Has_header___Returns_its_text()
    {
        var document = new Document();
        AddHeaderFooter(document, "foo", HeaderFooterType.HeaderFirst);

        var sut = new Parser();
        var actual = sut.Parse(document);

        Assert.Equal(new[] { "foo" }, actual);
    }

    [Fact]
    public void Has_empty_header___Ignores_it()
    {
        var document = new Document();
        AddHeaderFooter(document, string.Empty, HeaderFooterType.HeaderFirst);

        var sut = new Parser();
        var actual = sut.Parse(document);

        Assert.Empty(actual);
    }

    [Fact]
    public void Has_multiple_headers___Returns_their_text()
    {
        var document = new Document();
        AddHeaderFooter(document, "foo", HeaderFooterType.HeaderFirst);
        AddHeaderFooter(document, "bar", HeaderFooterType.HeaderEven);
        AddHeaderFooter(document, "baz", HeaderFooterType.HeaderPrimary);

        var sut = new Parser();
        var actual = sut.Parse(document);

        var expected = new[] { "bar", "foo", "baz" }.ToHashSet();
        Assert.True(expected.SetEquals(actual));
    }

    [Fact]
    public void Has_headers_in_different_sections___Returns_their_text()
    {
        var document = new Document();
        var builder = new DocumentBuilder(document);
        builder.MoveToHeaderFooter(HeaderFooterType.HeaderFirst);
        builder.Write("foo");
        builder.MoveToDocumentEnd();
        builder.InsertBreak(BreakType.PageBreak);
        builder.InsertBreak(BreakType.SectionBreakNewPage);
        builder.MoveToHeaderFooter(HeaderFooterType.HeaderFirst);
        builder.Write("bar");
        document.UpdateFields();

        var sut = new Parser();
        var actual = sut.Parse(document);

        var expected = new[] { "foo", "bar" }.ToHashSet();
        Assert.True(expected.SetEquals(actual));
    }

    private static void AddHeaderFooter(Document document, string text, HeaderFooterType type)
    {
        var builder = new DocumentBuilder(document);
        builder.MoveToHeaderFooter(type);
        builder.Write(text);
        document.UpdateFields();
    }
}