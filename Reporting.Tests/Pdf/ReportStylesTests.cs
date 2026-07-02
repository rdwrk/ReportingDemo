using MigraDoc.DocumentObjectModel;
using Reporting.Pdf.Styles;
using Xunit;

namespace Reporting.Tests.Pdf
{
    /// <summary>
    /// Verifies that <see cref="ReportStyles"/> constants have the expected values and that
    /// <see cref="ReportStyles.Apply"/> registers all named styles on a MigraDoc document.
    /// </summary>
    public class ReportStylesTests
    {
        // ── Style name constants ─────────────────────────────────────────────────

        [Fact] public void ReportTitle_ConstantValue()    => Assert.Equal("ReportTitle",    ReportStyles.ReportTitle);
        [Fact] public void ReportSubtitle_ConstantValue() => Assert.Equal("ReportSubtitle", ReportStyles.ReportSubtitle);
        [Fact] public void SectionHeading_ConstantValue() => Assert.Equal("SectionHeading", ReportStyles.SectionHeading);
        [Fact] public void TableHeader_ConstantValue()    => Assert.Equal("TableHeader",    ReportStyles.TableHeader);
        [Fact] public void TableBody_ConstantValue()      => Assert.Equal("TableBody",      ReportStyles.TableBody);
        [Fact] public void TableBodyAlt_ConstantValue()   => Assert.Equal("TableBodyAlt",   ReportStyles.TableBodyAlt);
        [Fact] public void TableGroupHdr_ConstantValue()  => Assert.Equal("TableGroupHdr",  ReportStyles.TableGroupHdr);
        [Fact] public void TableTotal_ConstantValue()     => Assert.Equal("TableTotal",     ReportStyles.TableTotal);
        [Fact] public void SummaryLabel_ConstantValue()   => Assert.Equal("SummaryLabel",   ReportStyles.SummaryLabel);
        [Fact] public void SummaryValue_ConstantValue()   => Assert.Equal("SummaryValue",   ReportStyles.SummaryValue);
        [Fact] public void FooterStyle_ConstantValue()    => Assert.Equal("FooterStyle",    ReportStyles.FooterStyle);

        // ── Colour constants ─────────────────────────────────────────────────────

        [Fact] public void ColourNavy_ConstantValue()    => Assert.Equal("#1F3864", ReportStyles.ColourNavy);
        [Fact] public void ColourBlue_ConstantValue()    => Assert.Equal("#2E75B6", ReportStyles.ColourBlue);
        [Fact] public void ColourGold_ConstantValue()    => Assert.Equal("#C9A227", ReportStyles.ColourGold);
        [Fact] public void ColourOverdue_ConstantValue() => Assert.Equal("#FFDDC1", ReportStyles.ColourOverdue);
        [Fact] public void ColourRed_ConstantValue()     => Assert.Equal("#C00000", ReportStyles.ColourRed);
        [Fact] public void ColourGrey_ConstantValue()    => Assert.Equal("#F2F2F2", ReportStyles.ColourGrey);
        [Fact] public void ColourMidGrey_ConstantValue() => Assert.Equal("#D9D9D9", ReportStyles.ColourMidGrey);
        [Fact] public void ColourWhite_ConstantValue()   => Assert.Equal("#FFFFFF", ReportStyles.ColourWhite);

        // ── Colour constants are valid MigraDoc hex strings ──────────────────────

        [Theory]
        [InlineData(ReportStyles.ColourNavy)]
        [InlineData(ReportStyles.ColourBlue)]
        [InlineData(ReportStyles.ColourGold)]
        [InlineData(ReportStyles.ColourOverdue)]
        [InlineData(ReportStyles.ColourRed)]
        [InlineData(ReportStyles.ColourGrey)]
        [InlineData(ReportStyles.ColourMidGrey)]
        [InlineData(ReportStyles.ColourWhite)]
        public void ColourConstants_AreValidMigraDocHexStrings(string colour)
        {
            // Color.Parse throws if the string is not a valid hex colour.
            var _ = Color.Parse(colour);
        }

        // ── Apply ────────────────────────────────────────────────────────────────

        [Fact]
        public void Apply_DoesNotThrow()
        {
            var doc = new Document();
            ReportStyles.Apply(doc);
        }

        [Fact]
        public void Apply_RegistersAllExpectedStyles()
        {
            var doc = new Document();
            ReportStyles.Apply(doc);

            var expected = new[]
            {
                ReportStyles.ReportTitle,
                ReportStyles.ReportSubtitle,
                ReportStyles.SectionHeading,
                ReportStyles.TableHeader,
                ReportStyles.TableBody,
                ReportStyles.TableBodyAlt,
                ReportStyles.TableGroupHdr,
                ReportStyles.TableTotal,
                ReportStyles.SummaryLabel,
                ReportStyles.SummaryValue,
                ReportStyles.FooterStyle,
            };

            foreach (var name in expected)
                Assert.NotNull(doc.Styles[name]);
        }

        [Fact]
        public void Apply_SetsNormalFontToFontBodyConstant()
        {
            var doc = new Document();
            ReportStyles.Apply(doc);
            Assert.Equal(ReportStyles.FontBody, doc.Styles[StyleNames.Normal].Font.Name);
        }

        // ── Font and size constants ──────────────────────────────────────────────

        [Fact] public void FontBody_ConstantValue()          => Assert.Equal("Calibri", ReportStyles.FontBody);
        [Fact] public void SizeTitle_ConstantValue()         => Assert.Equal(18,        ReportStyles.SizeTitle);
        [Fact] public void SizeSubtitle_ConstantValue()      => Assert.Equal(11,        ReportStyles.SizeSubtitle);
        [Fact] public void SizeSection_ConstantValue()       => Assert.Equal(12,        ReportStyles.SizeSection);
        [Fact] public void SizeNormal_ConstantValue()        => Assert.Equal(10,        ReportStyles.SizeNormal);
        [Fact] public void SizeTable_ConstantValue()         => Assert.Equal(9,         ReportStyles.SizeTable);
        [Fact] public void SizeFooter_ConstantValue()        => Assert.Equal(8,         ReportStyles.SizeFooter);
        [Fact] public void SpaceSubtitleAfter_ConstantValue()=> Assert.Equal(6,         ReportStyles.SpaceSubtitleAfter);
        [Fact] public void SpaceSectionBefore_ConstantValue()=> Assert.Equal(10,        ReportStyles.SpaceSectionBefore);
        [Fact] public void SpaceSectionAfter_ConstantValue() => Assert.Equal(4,         ReportStyles.SpaceSectionAfter);
    }
}
