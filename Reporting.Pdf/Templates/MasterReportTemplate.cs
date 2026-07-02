using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using Reporting.Core.Interfaces;
using Reporting.Core.Models;
using Reporting.Pdf.Styles;

namespace Reporting.Pdf.Templates
{
    /// <summary>
    /// Base class for all report builders.
    /// Handles page setup, the master header (title, subtitle, reference, divider line)
    /// and the master footer (generation timestamp, classification, page numbers).
    /// Subclasses implement <see cref="GetMetadata"/> and <see cref="BuildContent"/>.
    /// </summary>
    /// <typeparam name="TModel">The report model type this builder accepts.</typeparam>
    public abstract class MasterReportTemplate<TModel> : IReportBuilder<TModel>
        where TModel : IReportModel
    {
        /// <summary>Page size. Defaults to A4.</summary>
        protected virtual PageFormat  PageFormat     => PageFormat.A4;

        /// <summary>Page orientation. Defaults to Portrait.</summary>
        protected virtual Orientation Orientation    => Orientation.Portrait;

        /// <summary>Top margin in centimetres (space reserved for the page header). Defaults to 2.8 cm.</summary>
        protected virtual double TopMarginCm    => 2.8;

        /// <summary>Bottom margin in centimetres. Defaults to 2.5 cm.</summary>
        protected virtual double BottomMarginCm => 2.5;

        /// <summary>Left margin in centimetres. Defaults to 2.0 cm.</summary>
        protected virtual double LeftMarginCm   => 2.0;

        /// <summary>Right margin in centimetres. Defaults to 2.0 cm.</summary>
        protected virtual double RightMarginCm  => 2.0;

        /// <summary>
        /// Returns the metadata (title, subtitle, classification, etc.) for this report instance.
        /// Called once per <see cref="Build"/> invocation.
        /// </summary>
        protected abstract ReportMetadata GetMetadata(TModel model);

        /// <summary>
        /// Adds all content-specific elements (summary panels, tables, etc.) to the document section.
        /// Called after the master header and footer have been applied.
        /// </summary>
        protected abstract void BuildContent(Document document, Section section, TModel model);

        /// <summary>
        /// Builds the complete MigraDoc document: applies styles, configures the page,
        /// adds the master header and footer, then delegates to <see cref="BuildContent"/>.
        /// </summary>
        /// <param name="model">Populated report model.</param>
        /// <returns>A ready-to-render MigraDoc <see cref="Document"/>.</returns>
        public Document Build(TModel model)
        {
            var metadata = GetMetadata(model);
            var document = new Document();
            ReportStyles.Apply(document);

            var section = document.AddSection();
            ConfigurePageSetup(section, metadata);
            BuildMasterHeader(section, metadata);
            BuildMasterFooter(section, metadata, GetContentWidthCm());
            BuildContent(document, section, model);

            return document;
        }

        /// <summary>
        /// Applies page size, orientation and margins to the section.
        /// The top margin grows by 0.5 cm per extra <see cref="ReportMetadata.HeaderLines"/> entry
        /// so reports with more header info never overlap their content.
        /// </summary>
        private void ConfigurePageSetup(Section section, ReportMetadata metadata)
        {
            int    extraLines = metadata.HeaderLines?.Count ?? 0;
            double topMargin  = TopMarginCm + extraLines * 0.5;

            var ps          = section.PageSetup;
            ps.PageFormat   = PageFormat;
            ps.Orientation  = Orientation;
            ps.TopMargin    = Unit.FromCentimeter(topMargin);
            ps.BottomMargin = Unit.FromCentimeter(BottomMarginCm);
            ps.LeftMargin   = Unit.FromCentimeter(LeftMarginCm);
            ps.RightMargin  = Unit.FromCentimeter(RightMarginCm);
        }

        /// <summary>
        /// Builds the primary page header: report title on the left, optional company logo
        /// on the right, followed by a navy divider line. The title cell uses the same 4 pt
        /// left padding as data table cells so all content aligns vertically.
        /// </summary>
        private void BuildMasterHeader(Section section, ReportMetadata metadata)
        {
            var header = section.Headers.Primary;
            var table  = header.AddTable();
            table.Borders.Width  = 0;
            table.LeftPadding    = Unit.FromPoint(4);
            table.RightPadding   = Unit.FromPoint(4);

            double contentW = GetContentWidthCm();
            double logoW    = string.IsNullOrEmpty(metadata.LogoPath) ? 0 : metadata.LogoWidthCm;
            double textW    = contentW - logoW;

            table.AddColumn(Unit.FromCentimeter(textW));
            if (logoW > 0)
                table.AddColumn(Unit.FromCentimeter(logoW));

            var row = table.AddRow();
            row.VerticalAlignment = VerticalAlignment.Center;

            var titleCell = row.Cells[0];
            titleCell.AddParagraph(metadata.ReportTitle ?? string.Empty).Style = ReportStyles.ReportTitle;

            if (metadata.HeaderLines != null)
            {
                foreach (var line in metadata.HeaderLines)
                    titleCell.AddParagraph(line).Style = ReportStyles.ReportSubtitle;
            }

            if (logoW > 0)
            {
                var logoCell = row.Cells[1];
                logoCell.VerticalAlignment = VerticalAlignment.Center;
                var logoPara = logoCell.AddParagraph();
                logoPara.Format.Alignment = ParagraphAlignment.Right;
                var img = logoPara.AddImage(metadata.LogoPath);
                img.Width = Unit.FromCentimeter(logoW);
            }

        }

        /// <summary>
        /// Builds the primary page footer: a blue divider line above a three-column row.
        /// Left: application name and version. Centre: page number. Right: generation date.
        /// Uses the same 4 pt cell padding as data tables for consistent left-edge alignment.
        /// </summary>
        private void BuildMasterFooter(Section section, ReportMetadata metadata, double contentWidthCm)
        {
            var footer = section.Footers.Primary;

            var divider = footer.AddParagraph();
            divider.Format.Borders.Top.Width = 0.75;
            divider.Format.Borders.Top.Color = Color.Parse(ReportStyles.ColourBlue);
            divider.Format.SpaceBefore       = 2;

            var table = footer.AddTable();
            table.Borders.Width = 0;
            table.LeftPadding   = Unit.FromPoint(4);
            table.RightPadding  = Unit.FromPoint(4);

            double sideW   = contentWidthCm * 0.40;
            double centreW = contentWidthCm * 0.20;
            table.AddColumn(Unit.FromCentimeter(sideW));
            table.AddColumn(Unit.FromCentimeter(centreW));
            table.AddColumn(Unit.FromCentimeter(sideW));

            var row = table.AddRow();

            var leftPara = row.Cells[0].AddParagraph();
            leftPara.Style = ReportStyles.FooterStyle;
            leftPara.AddText($"{metadata.AppName} v{metadata.AppVersion}");

            var centrePara = row.Cells[1].AddParagraph();
            centrePara.Style            = ReportStyles.FooterStyle;
            centrePara.Format.Alignment = ParagraphAlignment.Center;
            centrePara.AddText("Page ");
            centrePara.AddPageField();
            centrePara.AddText(" of ");
            centrePara.AddNumPagesField();

            var rightPara = row.Cells[2].AddParagraph();
            rightPara.Style            = ReportStyles.FooterStyle;
            rightPara.Format.Alignment = ParagraphAlignment.Right;
            rightPara.AddText($"Generated: {metadata.GeneratedDate:dd MMM yyyy HH:mm}");
        }

        /// <summary>
        /// Returns the usable content width in centimetres based on the page size and margins.
        /// A4 landscape = 29.7 cm; A4 portrait = 21.0 cm.
        /// </summary>
        // Assumes A4 page size (29.7 × 21.0 cm). Update if a subclass overrides PageFormat to non-A4.
        protected double GetContentWidthCm()
        {
            bool landscape = Orientation == Orientation.Landscape;
            double pageW   = landscape ? 29.7 : 21.0;
            return pageW - LeftMarginCm - RightMarginCm;
        }
    }
}
