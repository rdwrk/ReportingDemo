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

        /// <summary>Top margin in centimetres (large to accommodate the header). Defaults to 4.5 cm.</summary>
        protected virtual double TopMarginCm    => 4.5;

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
            ConfigurePageSetup(section);
            BuildMasterHeader(section, metadata);
            BuildMasterFooter(section, metadata, GetContentWidthCm());
            BuildContent(document, section, model);

            return document;
        }

        /// <summary>Applies page size, orientation and margins to the section.</summary>
        private void ConfigurePageSetup(Section section)
        {
            var ps          = section.PageSetup;
            ps.PageFormat   = PageFormat;
            ps.Orientation  = Orientation;
            ps.TopMargin    = Unit.FromCentimeter(TopMarginCm);
            ps.BottomMargin = Unit.FromCentimeter(BottomMarginCm);
            ps.LeftMargin   = Unit.FromCentimeter(LeftMarginCm);
            ps.RightMargin  = Unit.FromCentimeter(RightMarginCm);
        }

        /// <summary>
        /// Builds the primary page header: two-column table with the title block on the left
        /// and an optional logo on the right, followed by a navy divider line.
        /// </summary>
        private void BuildMasterHeader(Section section, ReportMetadata metadata)
        {
            var header = section.Headers.Primary;
            var table  = header.AddTable();
            table.Borders.Width = 0;

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

            if (!string.IsNullOrEmpty(metadata.Subtitle))
                titleCell.AddParagraph(metadata.Subtitle).Style = ReportStyles.ReportSubtitle;

            var metaLine = titleCell.AddParagraph();
            metaLine.Style = ReportStyles.ReportSubtitle;
            if (!string.IsNullOrEmpty(metadata.ReportReference))
                metaLine.AddText($"Ref: {metadata.ReportReference}   ");
            if (!string.IsNullOrEmpty(metadata.PreparedBy))
                metaLine.AddText($"Prepared by: {metadata.PreparedBy}   ");
            if (!string.IsNullOrEmpty(metadata.Classification))
                metaLine.AddText($"[{metadata.Classification}]");

            var divider = header.AddParagraph();
            divider.Format.Borders.Bottom.Width = 1.5;
            divider.Format.Borders.Bottom.Color = Color.Parse(ReportStyles.ColourNavy);
            divider.Format.SpaceAfter = 4;
        }

        /// <summary>
        /// Builds the primary page footer: a blue divider line above a two-column row
        /// showing the generation timestamp on the left and the page number on the right.
        /// </summary>
        private void BuildMasterFooter(Section section, ReportMetadata metadata, double contentWidthCm)
        {
            var footer = section.Footers.Primary;

            var divider = footer.AddParagraph();
            divider.Format.Borders.Top.Width = 0.75;
            divider.Format.Borders.Top.Color = Color.Parse(ReportStyles.ColourBlue);
            divider.Format.SpaceBefore = 2;

            var table = footer.AddTable();
            table.Borders.Width = 0;
            table.AddColumn(Unit.FromCentimeter(contentWidthCm / 2));
            table.AddColumn(Unit.FromCentimeter(contentWidthCm / 2));

            var row = table.AddRow();

            var leftPara = row.Cells[0].AddParagraph();
            leftPara.Style = ReportStyles.FooterStyle;
            leftPara.AddText($"Generated: {metadata.GeneratedDate:dd MMM yyyy HH:mm}");
            if (!string.IsNullOrEmpty(metadata.Classification))
                leftPara.AddText($"   |   {metadata.Classification}");

            var rightPara = row.Cells[1].AddParagraph();
            rightPara.Style           = ReportStyles.FooterStyle;
            rightPara.Format.Alignment = ParagraphAlignment.Right;
            rightPara.AddText("Page ");
            rightPara.AddPageField();
            rightPara.AddText(" of ");
            rightPara.AddNumPagesField();
        }

        /// <summary>
        /// Returns the usable content width in centimetres based on the page size and margins.
        /// A4 landscape = 29.7 cm; A4 portrait = 21.0 cm.
        /// </summary>
        protected double GetContentWidthCm()
        {
            bool landscape = Orientation == Orientation.Landscape;
            double pageW   = landscape ? 29.7 : 21.0;
            return pageW - LeftMarginCm - RightMarginCm;
        }
    }
}
