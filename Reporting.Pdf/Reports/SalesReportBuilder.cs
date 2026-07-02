using System.Collections.Generic;
using System.Linq;
using MigraDoc.DocumentObjectModel;
using Reporting.Core.Models;
using Reporting.Core.Templates;
using Reporting.Pdf.Assets;
using Reporting.Pdf.Components;
using Reporting.Pdf.Styles;
using Reporting.Pdf.Templates;

namespace Reporting.Pdf.Reports
{
    /// <summary>
    /// Builds the Sales Summary Report as a landscape A4 PDF.
    /// Produces a summary panel (total lines, revenue, gross profit, margin) followed by
    /// a paginated detail table of individual sales lines grouped by the current filter.
    /// Renders no data rows when <see cref="SalesReportModel.Items"/> is null or empty.
    /// </summary>
    public class SalesReportBuilder : MasterReportTemplate<SalesReportModel>
    {
        protected override Orientation Orientation => Orientation.Landscape;

        protected override ReportMetadata GetMetadata(SalesReportModel model)
        {
            string dateRange = model.DateFrom.HasValue && model.DateTo.HasValue
                ? $"{model.DateFrom:dd MMM yyyy} to {model.DateTo:dd MMM yyyy}"
                : "All Dates";
            string region = string.IsNullOrEmpty(model.RegionFilter) ? "All Regions" : model.RegionFilter;

            var lines = new List<string> { $"{region} — {dateRange}" };
            if (!string.IsNullOrEmpty(model.PreparedBy))
                lines.Add($"Prepared by: {model.PreparedBy}   |   Ref: RPT-SLS-001   |   Commercial — Confidential");

            return new ReportMetadata
            {
                ReportTitle  = "Sales Summary Report",
                HeaderLines  = lines,
                LogoPath     = LogoProvider.GetPath(),
                LogoWidthCm  = 3.5,
            };
        }

        protected override void BuildContent(Document document, Section section, SalesReportModel model)
        {
            var summary = new Dictionary<string, string>
            {
                ["Total Lines"]   = (model.Items?.Count ?? 0).ToString("N0"),
                ["Total Revenue"] = $"£{model.TotalRevenue:N2}",
                ["Gross Profit"]  = $"£{model.TotalGrossProfit:N2}",
                ["Margin"]        = model.TotalRevenue > 0
                    ? $"{model.TotalGrossProfit / model.TotalRevenue:P1}"
                    : "N/A",
            };
            SummaryPanel.Add(section, summary, GetContentWidthCm());

            var heading = section.AddParagraph("Sales Detail");
            heading.Style = ReportStyles.SectionHeading;

            string[] headers = { "Date", "Product", "Sales Rep", "Region", "Units", "Unit Price", "Revenue £", "Gross Profit £" };
            double[] weights = { 1.0, 2.2, 1.7, 1.0, 0.8, 1.2, 1.2, 1.2 };
            var alignments = new[]
            {
                ParagraphAlignment.Left,
                ParagraphAlignment.Left,
                ParagraphAlignment.Left,
                ParagraphAlignment.Left,
                ParagraphAlignment.Right,
                ParagraphAlignment.Right,
                ParagraphAlignment.Right,
                ParagraphAlignment.Right,
            };

            var builder = ReportTableBuilder.Create(section, GetContentWidthCm(), weights, headers, alignments);

            foreach (var item in model.Items ?? Enumerable.Empty<SalesLineItem>())
            {
                builder.AddRow(new[]
                {
                    item.SaleDate.ToString("dd/MM/yyyy"),
                    item.Product,
                    item.SalesRep,
                    item.Region,
                    item.UnitsSold.ToString("N0"),
                    $"{item.UnitPrice:N2}",
                    $"{item.Revenue:N2}",
                    $"{item.GrossProfit:N2}",
                }, alignments);
            }

            var totals = new string[6];
            totals[4] = $"{model.TotalRevenue:N2}";
            totals[5] = $"{model.TotalGrossProfit:N2}";
            builder.AddTotalRow("Grand Total", totals, labelColIndex: 0);
        }
    }
}
