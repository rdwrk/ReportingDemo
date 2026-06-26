using System.Collections.Generic;
using MigraDoc.DocumentObjectModel;
using Reporting.Core.Models;
using Reporting.Pdf.Components;
using Reporting.Pdf.Styles;
using Reporting.Pdf.Templates;

namespace Reporting.Pdf.Reports
{
    public class SalesReportBuilder : MasterReportTemplate<SalesReportModel>
    {
        protected override Orientation Orientation => Orientation.Landscape;

        protected override ReportMetadata GetMetadata(SalesReportModel model)
        {
            string dateRange = model.DateFrom.HasValue && model.DateTo.HasValue
                ? $"{model.DateFrom:dd MMM yyyy} to {model.DateTo:dd MMM yyyy}"
                : "All Dates";
            string region = string.IsNullOrEmpty(model.RegionFilter) ? "All Regions" : model.RegionFilter;

            return new ReportMetadata
            {
                ReportTitle     = "Sales Summary Report",
                Subtitle        = $"{region} — {dateRange}",
                ReportReference = "RPT-SLS-001",
                PreparedBy      = model.PreparedBy,
                Classification  = "Commercial — Confidential",
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
            SummaryPanel.Add(section, summary);

            var heading = section.AddParagraph("Sales Detail");
            heading.Style = ReportStyles.SectionHeading;

            double[] widths  = { 2.5, 5.0, 4.0, 2.5, 2.5, 2.5, 2.5, 2.5 };
            string[] headers = { "Date", "Product", "Sales Rep", "Region", "Units", "Unit Price", "Revenue £", "Gross Profit £" };
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

            var builder = ReportTableBuilder.Create(section, widths, headers, alignments);

            foreach (var item in model.Items)
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
