using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aspose.Words;
using Aspose.Words.Notes;
using Xunit;

namespace tests;

public class ParserTests
{
    [Theory]
    [ClassData(typeof(HeaderFooterTypeData))]
    public void Has_header_or_footer___Returns_its_text(HeaderFooterType type)
    {
        var headerText = GetRandomText();
        var document = new Document();
        AddHeaderFooter(document, headerText, type);

        var sut = new Parser();
        var actual = sut.Parse(document);

        Assert.Equal(new[] { headerText }, actual);
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

    [Theory]
    [ClassData(typeof(HeaderFooterTypeData))]
    public void Has_headers_or_footers_in_different_sections___Returns_their_text(HeaderFooterType type)
    {
        var headerText = GetRandomText();
        var anotherHeaderText = GetRandomText();
        var document = new Document();
        var builder = new DocumentBuilder(document);
        builder.MoveToHeaderFooter(type);
        builder.Write(headerText);
        builder.MoveToDocumentEnd();
        builder.InsertBreak(BreakType.PageBreak);
        builder.InsertBreak(BreakType.SectionBreakNewPage);
        builder.MoveToHeaderFooter(type);
        builder.Write(anotherHeaderText);
        document.UpdateFields();

        var sut = new Parser();
        var actual = sut.Parse(document);

        var expected = new[] { headerText, anotherHeaderText }.ToHashSet();
        Assert.True(expected.SetEquals(actual));
    }

    [Theory]
    [ClassData(typeof(FootnoteTypeData))]
    public void Has_footnote_or_endnote___Returns_its_text(FootnoteType type)
    {
        var headerText = GetRandomText();
        var document = new Document();
        AddFootnote(document, headerText, type);

        var sut = new Parser();
        var actual = sut.Parse(document);

        Assert.Equal(new[] { headerText }, actual);
    }

    [Theory]
    [ClassData(typeof(FootnoteTypeData))]
    public void Has_empty_footnote_or_endnote___Ignores_it(FootnoteType type)
    {
        var document = new Document();
        AddFootnote(document, string.Empty, type);

        var sut = new Parser();
        var actual = sut.Parse(document);

        Assert.Empty(actual);
    }

    [Theory]
    [ClassData(typeof(FootnoteTypeData))]
    public void Has_footnotes_or_endnotes_in_different_sections___Returns_their_text(FootnoteType type)
    {
        var headerText = GetRandomText();
        var anotherHeaderText = GetRandomText();
        var document = new Document();
        var builder = new DocumentBuilder(document);
        builder.InsertFootnote(type, headerText);
        builder.MoveToDocumentEnd();
        builder.InsertBreak(BreakType.PageBreak);
        builder.InsertBreak(BreakType.SectionBreakNewPage);
        builder.InsertFootnote(type, anotherHeaderText);

        var sut = new Parser();
        var actual = sut.Parse(document);

        var expected = new[] { headerText, anotherHeaderText }.ToHashSet();
        Assert.True(expected.SetEquals(actual));
    }

    [Fact]
    public void Has_header_and_footnote___Ignores_footnote()
    {
        var headerText = GetRandomText();
        var footnoteText = GetRandomText();
        var document = new Document();
        AddHeaderFooter(document, headerText, HeaderFooterType.FooterFirst);
        AddFootnote(document, footnoteText, FootnoteType.Footnote);

        var sut = new Parser();
        var actual = sut.Parse(document);

        Assert.Equal(new[] { headerText }, actual);
    }

    [Fact]
    public void Has_header_and_paragraph___Ignores_paragraph()
    {
        var headerText = GetRandomText();
        var paragraphText = GetRandomText();
        var document = new Document();
        AddHeaderFooter(document, headerText, HeaderFooterType.FooterFirst);
        AddParagraph(document, paragraphText);

        var sut = new Parser();
        var actual = sut.Parse(document);

        Assert.Equal(new[] { headerText }, actual);
    }

    [Fact]
    public void Has_footnote_and_paragraph___Ignores_paragraph()
    {
        var footnoteText = GetRandomText();
        var paragraphText = GetRandomText();
        var document = new Document();
        AddFootnote(document, footnoteText, FootnoteType.Footnote);
        AddParagraph(document, paragraphText);

        var sut = new Parser();
        var actual = sut.Parse(document);

        Assert.Equal(new[] { footnoteText }, actual);
    }

    private static void AddHeaderFooter(Document document, string text, HeaderFooterType type)
    {
        var builder = new DocumentBuilder(document);
        builder.MoveToHeaderFooter(type);
        builder.Write(text);
        document.UpdateFields();
    }

    private static void AddFootnote(Document document, string text, FootnoteType type)
    {
        var builder = new DocumentBuilder(document);
        builder.InsertFootnote(type, text);
    }

    private static void AddParagraph(Document document, string text)
    {
        var builder = new DocumentBuilder(document);
        builder.Writeln(text);
    }

    private string GetRandomText()
    {
        return Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
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

public class FootnoteTypeData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { FootnoteType.Footnote };
        yield return new object[] { FootnoteType.Endnote };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}