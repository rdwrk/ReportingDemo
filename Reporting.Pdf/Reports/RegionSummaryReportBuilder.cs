using System.Collections.Generic;
using MigraDoc.DocumentObjectModel;
using Reporting.Core.Models;
using Reporting.Pdf.Assets;
using Reporting.Pdf.Components;
using Reporting.Pdf.Styles;
using Reporting.Pdf.Templates;

namespace Reporting.Pdf.Reports
{
    /// <summary>
    /// Builds the Region Performance Overview report — a single-page portrait A4 summary
    /// of sales aggregated by region. No filter parameters are required; data is always
    /// generated for the full seeded dataset.
    /// </summary>
    public class RegionSummaryReportBuilder : MasterReportTemplate<RegionSummaryReportModel>
    {
        protected override Orientation Orientation => Orientation.Portrait;

        protected override ReportMetadata GetMetadata(RegionSummaryReportModel model)
        {
            return new ReportMetadata
            {
                ReportTitle = "Region Performance Overview",
                LogoPath    = LogoProvider.GetPath(),
                LogoWidthCm = 3.5,
            };
        }

        protected override void BuildContent(Document document, Section section, RegionSummaryReportModel model)
        {
            var summary = new Dictionary<string, string>
            {
                ["Regions"]       = model.Rows.Count.ToString("N0"),
                ["Total Lines"]   = model.GrandLineCount.ToString("N0"),
                ["Total Revenue"] = $"£{model.GrandRevenue:N2}",
                ["Overall Margin"] = $"{model.GrandMargin:P1}",
            };
            SummaryPanel.Add(section, summary, GetContentWidthCm());

            var heading = section.AddParagraph("Sales by Region");
            heading.Style = ReportStyles.SectionHeading;

            string[] headers = { "Region", "Lines", "Revenue £", "Gross Profit £", "Margin %" };
            double[] weights = { 2.0, 1.0, 2.0, 2.0, 1.5 };
            var alignments = new[]
            {
                ParagraphAlignment.Left,
                ParagraphAlignment.Right,
                ParagraphAlignment.Right,
                ParagraphAlignment.Right,
                ParagraphAlignment.Right,
            };

            var builder = ReportTableBuilder.Create(section, GetContentWidthCm(), weights, headers, alignments);

            foreach (var row in model.Rows)
            {
                builder.AddRow(new[]
                {
                    row.Region,
                    row.LineCount.ToString("N0"),
                    $"{row.Revenue:N2}",
                    $"{row.GrossProfit:N2}",
                    $"{row.Margin:P1}",
                }, alignments);
            }

            builder.AddTotalRow("Grand Total", new[]
            {
                model.GrandLineCount.ToString("N0"),
                $"{model.GrandRevenue:N2}",
                $"{model.GrandGrossProfit:N2}",
                $"{model.GrandMargin:P1}",
            }, labelColIndex: 0);
        }
    }
}
