using MigraDoc.DocumentObjectModel;

namespace Reporting.Pdf.Styles
{
    /// <summary>
    /// Defines all named MigraDoc styles and colour constants used across every report.
    /// Call <see cref="Apply"/> once per document immediately after creation.
    /// </summary>
    public static class ReportStyles
    {
        // ── Style name constants ─────────────────────────────────────────────────

        /// <summary>Large bold navy title in the page header.</summary>
        public const string ReportTitle    = "ReportTitle";

        /// <summary>Smaller blue subtitle line beneath the main title.</summary>
        public const string ReportSubtitle = "ReportSubtitle";

        /// <summary>Bold navy heading that introduces a content section. Always keeps with the next paragraph.</summary>
        public const string SectionHeading = "SectionHeading";

        /// <summary>White bold text on a navy background for table column headers.</summary>
        public const string TableHeader    = "TableHeader";

        /// <summary>Standard 9pt body text for even data rows.</summary>
        public const string TableBody      = "TableBody";

        /// <summary>Standard 9pt body text for odd (alternate) data rows.</summary>
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

        // ── Colour constants (hex with # prefix for MigraDoc Color.Parse) ───────

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

        // ── Style registration ───────────────────────────────────────────────────

        /// <summary>
        /// Registers all named styles on the supplied document.
        /// Must be called before any content is added to the document.
        /// </summary>
        /// <param name="document">The MigraDoc document to configure.</param>
        public static void Apply(Document document)
        {
            var styles = document.Styles;

            var normal = styles[MigraDoc.DocumentObjectModel.StyleNames.Normal];
            normal.Font.Name = "Calibri";
            normal.Font.Size = 10;

            var title = styles.AddStyle(ReportTitle, MigraDoc.DocumentObjectModel.StyleNames.Normal);
            title.Font.Name  = "Calibri";
            title.Font.Size  = 18;
            title.Font.Bold  = true;
            title.Font.Color = Color.Parse(ColourNavy);

            var subtitle = styles.AddStyle(ReportSubtitle, MigraDoc.DocumentObjectModel.StyleNames.Normal);
            subtitle.Font.Name  = "Calibri";
            subtitle.Font.Size  = 11;
            subtitle.Font.Color = Color.Parse(ColourBlue);
            subtitle.ParagraphFormat.SpaceAfter = 6;

            var section = styles.AddStyle(SectionHeading, MigraDoc.DocumentObjectModel.StyleNames.Normal);
            section.Font.Name     = "Calibri";
            section.Font.Size     = 12;
            section.Font.Bold     = true;
            section.Font.Color    = Color.Parse(ColourNavy);
            section.ParagraphFormat.SpaceBefore  = 10;
            section.ParagraphFormat.SpaceAfter   = 4;
            section.ParagraphFormat.KeepWithNext = true;

            var tableHdr = styles.AddStyle(TableHeader, MigraDoc.DocumentObjectModel.StyleNames.Normal);
            tableHdr.Font.Name  = "Calibri";
            tableHdr.Font.Size  = 9;
            tableHdr.Font.Bold  = true;
            tableHdr.Font.Color = Color.Parse(ColourWhite);

            var tableBody = styles.AddStyle(TableBody, MigraDoc.DocumentObjectModel.StyleNames.Normal);
            tableBody.Font.Name = "Calibri";
            tableBody.Font.Size = 9;
            tableBody.ParagraphFormat.WidowControl = true;

            var tableBodyAlt = styles.AddStyle(TableBodyAlt, MigraDoc.DocumentObjectModel.StyleNames.Normal);
            tableBodyAlt.Font.Name = "Calibri";
            tableBodyAlt.Font.Size = 9;
            tableBodyAlt.ParagraphFormat.WidowControl = true;

            var groupHdr = styles.AddStyle(TableGroupHdr, MigraDoc.DocumentObjectModel.StyleNames.Normal);
            groupHdr.Font.Name  = "Calibri";
            groupHdr.Font.Size  = 9;
            groupHdr.Font.Bold  = true;
            groupHdr.Font.Color = Color.Parse(ColourNavy);

            var total = styles.AddStyle(TableTotal, MigraDoc.DocumentObjectModel.StyleNames.Normal);
            total.Font.Name = "Calibri";
            total.Font.Size = 9;
            total.Font.Bold = true;

            var summaryLabel = styles.AddStyle(SummaryLabel, MigraDoc.DocumentObjectModel.StyleNames.Normal);
            summaryLabel.Font.Name  = "Calibri";
            summaryLabel.Font.Size  = 9;
            summaryLabel.Font.Bold  = true;
            summaryLabel.Font.Color = Color.Parse(ColourNavy);

            var summaryValue = styles.AddStyle(SummaryValue, MigraDoc.DocumentObjectModel.StyleNames.Normal);
            summaryValue.Font.Name = "Calibri";
            summaryValue.Font.Size = 9;

            var footer = styles.AddStyle(FooterStyle, MigraDoc.DocumentObjectModel.StyleNames.Normal);
            footer.Font.Name  = "Calibri";
            footer.Font.Size  = 8;
            footer.Font.Color = Color.Parse(ColourBlue);
        }
    }
}
