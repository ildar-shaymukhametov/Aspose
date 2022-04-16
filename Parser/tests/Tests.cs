using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
        AddHeaderFooter(builder, headerText, type);
        AddNewPage(builder);
        AddHeaderFooter(builder, anotherHeaderText, type);

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
        AddFootnote(builder, headerText, type);
        AddNewPage(builder);
        AddFootnote(builder, anotherHeaderText, type);

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

    [Fact]
    public void Has_no_header_and_no_footnote___Returns_first_paragraph_of_every_section()
    {
        var paragraphText = GetRandomText();
        var anotherParagraphText = GetRandomText();

        var document = new Document();
        var builder = new DocumentBuilder(document);
        AddParagraph(document, paragraphText);
        AddNewPage(builder);
        AddParagraph(document, anotherParagraphText);

        var sut = new Parser();
        var actual = sut.Parse(document);

        var expected = new[] { paragraphText, anotherParagraphText }.ToHashSet();
        Assert.True(expected.SetEquals(actual));
    }

    private static void AddHeaderFooter(DocumentBuilder builder, string text, HeaderFooterType type)
    {
        builder.MoveToHeaderFooter(type);
        builder.Write(text);
        builder.Document.UpdateFields();
    }

    private static void AddHeaderFooter(Document document, string text, HeaderFooterType type)
    {
        var builder = new DocumentBuilder(document);
        AddHeaderFooter(builder, text, type);
    }

    private static void AddFootnote(DocumentBuilder builder, string text, FootnoteType type)
    {
        builder.InsertFootnote(type, text);
    }

    private static void AddFootnote(Document document, string text, FootnoteType type)
    {
        var builder = new DocumentBuilder(document);
        AddFootnote(builder, text, type);
    }

    private static void AddParagraph(Document document, string text)
    {
        var builder = new DocumentBuilder(document);
        builder.Writeln(text);
    }

    private static void AddNewPage(DocumentBuilder builder)
    {
        builder.MoveToDocumentEnd();
        builder.InsertBreak(BreakType.PageBreak);
        builder.InsertBreak(BreakType.SectionBreakNewPage);
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