using MigraDoc.DocumentObjectModel;

namespace Reporting.Pdf.Styles
{
    /// <summary>
    /// Single source of truth for every visual property used across all reports.
    /// <para>
    /// <b>To restyle a report:</b> change the font, size, spacing, or colour constants in this file only —
    /// no other file needs to be touched. Call <see cref="Apply"/> once per document immediately after creation.
    /// </para>
    /// </summary>
    public static class ReportStyles
    {
        // ── Style name constants ─────────────────────────────────────────────────
        // Used as keys when registering and referencing MigraDoc styles.

        /// <summary>Large bold navy title in the page header.</summary>
        public const string ReportTitle    = "ReportTitle";

        /// <summary>Smaller blue subtitle line beneath the main title.</summary>
        public const string ReportSubtitle = "ReportSubtitle";

        /// <summary>Bold navy heading that introduces a content section. Always keeps with the next paragraph.</summary>
        public const string SectionHeading = "SectionHeading";

        /// <summary>White bold text on a navy background for table column headers.</summary>
        public const string TableHeader    = "TableHeader";

        /// <summary>Body text for even data rows.</summary>
        public const string TableBody      = "TableBody";

        /// <summary>Body text for odd (alternate) data rows.</summary>
        public const string TableBodyAlt   = "TableBodyAlt";

        /// <summary>Bold navy text used in group header rows that span all columns.</summary>
        public const string TableGroupHdr  = "TableGroupHdr";

        /// <summary>Bold white text on a blue background for subtotal and grand total rows.</summary>
        public const string TableTotal     = "TableTotal";

        /// <summary>Bold navy label cell in the summary panel.</summary>
        public const string SummaryLabel   = "SummaryLabel";

        /// <summary>Regular value cell in the summary panel.</summary>
        public const string SummaryValue   = "SummaryValue";

        /// <summary>Small blue text used in the page footer.</summary>
        public const string FooterStyle    = "FooterStyle";

        // ── Colour constants (hex with # prefix for MigraDoc Color.Parse) ────────
        // Change a value here to update that colour across every report.

        /// <summary>Dark navy (#1F3864) — primary brand colour for headings and header backgrounds.</summary>
        public const string ColourNavy    = "#1F3864";

        /// <summary>Mid blue (#2E75B6) — accent colour for subtitles, footers and total rows.</summary>
        public const string ColourBlue    = "#2E75B6";

        /// <summary>Gold (#C9A227) — available for highlighting or badges.</summary>
        public const string ColourGold    = "#C9A227";

        /// <summary>Pale orange (#FFDDC1) — cell background for overdue invoice rows.</summary>
        public const string ColourOverdue = "#FFDDC1";

        /// <summary>Deep red (#C00000) — font colour for overdue status text.</summary>
        public const string ColourRed     = "#C00000";

        /// <summary>Light grey (#F2F2F2) — alternate row and summary panel background.</summary>
        public const string ColourGrey    = "#F2F2F2";

        /// <summary>Mid grey (#D9D9D9) — group header row background and table border colour.</summary>
        public const string ColourMidGrey = "#D9D9D9";

        /// <summary>White (#FFFFFF) — standard even-row background.</summary>
        public const string ColourWhite   = "#FFFFFF";

        // ── Font constants ───────────────────────────────────────────────────────
        // Change FontBody here to swap the typeface across every report style at once.
        // FontBody must be registered in WindowsFontResolver.Map for PDF rendering to work.

        /// <summary>Primary typeface used across all report styles.</summary>
        public const string FontBody = "Calibri";

        // ── Font size constants (points) ─────────────────────────────────────────

        /// <summary>Font size for the report title in the page header.</summary>
        public const int SizeTitle    = 18;

        /// <summary>Font size for subtitle lines beneath the report title.</summary>
        public const int SizeSubtitle = 11;

        /// <summary>Font size for section heading paragraphs.</summary>
        public const int SizeSection  = 12;

        /// <summary>Font size for the base document Normal style.</summary>
        public const int SizeNormal   = 10;

        /// <summary>Font size for all table cell styles (header, body, group header, total, summary).</summary>
        public const int SizeTable    = 9;

        /// <summary>Font size for the page footer.</summary>
        public const int SizeFooter   = 8;

        // ── Spacing constants (points) ───────────────────────────────────────────

        /// <summary>Space after each subtitle paragraph.</summary>
        public const int SpaceSubtitleAfter  = 6;

        /// <summary>Space before a section heading paragraph.</summary>
        public const int SpaceSectionBefore  = 10;

        /// <summary>Space after a section heading paragraph.</summary>
        public const int SpaceSectionAfter   = 4;

        // ── Style registration ───────────────────────────────────────────────────

        /// <summary>
        /// Registers all named styles on the supplied document using the constants defined in this class.
        /// Must be called before any content is added to the document.
        /// </summary>
        /// <param name="document">The MigraDoc document to configure.</param>
        public static void Apply(Document document)
        {
            var styles = document.Styles;

            var normal = styles[StyleNames.Normal];
            normal.Font.Name = FontBody;
            normal.Font.Size = SizeNormal;

            var title = styles.AddStyle(ReportTitle, StyleNames.Normal);
            title.Font.Name  = FontBody;
            title.Font.Size  = SizeTitle;
            title.Font.Bold  = true;
            title.Font.Color = Color.Parse(ColourNavy);

            var subtitle = styles.AddStyle(ReportSubtitle, StyleNames.Normal);
            subtitle.Font.Name  = FontBody;
            subtitle.Font.Size  = SizeSubtitle;
            subtitle.Font.Color = Color.Parse(ColourBlue);
            subtitle.ParagraphFormat.SpaceAfter = SpaceSubtitleAfter;

            var section = styles.AddStyle(SectionHeading, StyleNames.Normal);
            section.Font.Name     = FontBody;
            section.Font.Size     = SizeSection;
            section.Font.Bold     = true;
            section.Font.Color    = Color.Parse(ColourNavy);
            section.ParagraphFormat.SpaceBefore  = SpaceSectionBefore;
            section.ParagraphFormat.SpaceAfter   = SpaceSectionAfter;
            section.ParagraphFormat.KeepWithNext = true;

            var tableHdr = styles.AddStyle(TableHeader, StyleNames.Normal);
            tableHdr.Font.Name  = FontBody;
            tableHdr.Font.Size  = SizeTable;
            tableHdr.Font.Bold  = true;
            tableHdr.Font.Color = Color.Parse(ColourWhite);

            var tableBody = styles.AddStyle(TableBody, StyleNames.Normal);
            tableBody.Font.Name = FontBody;
            tableBody.Font.Size = SizeTable;
            tableBody.ParagraphFormat.WidowControl = true;

            var tableBodyAlt = styles.AddStyle(TableBodyAlt, StyleNames.Normal);
            tableBodyAlt.Font.Name = FontBody;
            tableBodyAlt.Font.Size = SizeTable;
            tableBodyAlt.ParagraphFormat.WidowControl = true;

            var groupHdr = styles.AddStyle(TableGroupHdr, StyleNames.Normal);
            groupHdr.Font.Name  = FontBody;
            groupHdr.Font.Size  = SizeTable;
            groupHdr.Font.Bold  = true;
            groupHdr.Font.Color = Color.Parse(ColourNavy);

            var total = styles.AddStyle(TableTotal, StyleNames.Normal);
            total.Font.Name = FontBody;
            total.Font.Size = SizeTable;
            total.Font.Bold = true;

            var summaryLabel = styles.AddStyle(SummaryLabel, StyleNames.Normal);
            summaryLabel.Font.Name  = FontBody;
            summaryLabel.Font.Size  = SizeTable;
            summaryLabel.Font.Bold  = true;
            summaryLabel.Font.Color = Color.Parse(ColourNavy);

            var summaryValue = styles.AddStyle(SummaryValue, StyleNames.Normal);
            summaryValue.Font.Name = FontBody;
            summaryValue.Font.Size = SizeTable;

            var footer = styles.AddStyle(FooterStyle, StyleNames.Normal);
            footer.Font.Name  = FontBody;
            footer.Font.Size  = SizeFooter;
            footer.Font.Color = Color.Parse(ColourBlue);
        }
    }
}
