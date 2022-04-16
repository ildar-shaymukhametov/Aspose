using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Aspose.Words;
using Xunit;

namespace tests;

public class ParserTests
{
    [Theory]
    [ClassData(typeof(HeaderFooterTypeData))]
    public void Has_header_or_footer___Returns_its_text(HeaderFooterType type)
    {
        var document = new Document();
        AddHeaderFooter(document, "foo", type);

        var sut = new Parser();
        var actual = sut.Parse(document);

        Assert.Equal(new[] { "foo" }, actual);
    }

    [Theory]
    [ClassData(typeof(HeaderFooterTypeData))]
    public void Has_empty_header_or_footer__Ignores_it(HeaderFooterType type)
    {
        var document = new Document();
        AddHeaderFooter(document, string.Empty, type);

        var sut = new Parser();
        var actual = sut.Parse(document);

        Assert.Empty(actual);
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

public class HeaderFooterTypeData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { HeaderFooterType.FooterEven };
        yield return new object[] { HeaderFooterType.FooterFirst };
        yield return new object[] { HeaderFooterType.FooterPrimary };
        yield return new object[] { HeaderFooterType.HeaderEven };
        yield return new object[] { HeaderFooterType.HeaderFirst };
        yield return new object[] { HeaderFooterType.HeaderPrimary };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}