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
        var builder = new DocumentBuilder(document);
        builder.MoveToHeaderFooter(HeaderFooterType.HeaderFirst);
        builder.Write("foo");
        document.UpdateFields();

        var sut = new Parser();
        var actual = sut.Parse(document);

        Assert.Equal(new [] { "foo" }, actual);
    }

    [Fact]
    public void Has_empty_header___Ignores_it()
    {
        var document = new Document();
        var builder = new DocumentBuilder(document);
        builder.MoveToHeaderFooter(HeaderFooterType.HeaderFirst);
        builder.Write(string.Empty);
        document.UpdateFields();

        var sut = new Parser();
        var actual = sut.Parse(document);

        Assert.Empty(actual);
    }
}