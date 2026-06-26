using System.Collections.Generic;
using MigraDoc.DocumentObjectModel;
using Reporting.Core.Models;
using Reporting.Pdf.Components;
using Reporting.Pdf.Styles;
using Reporting.Pdf.Templates;

namespace Reporting.Pdf.Reports
{
    public class InvoiceReportBuilder : MasterReportTemplate<InvoiceReportModel>
    {
        protected override Orientation Orientation => Orientation.Landscape;

        protected override ReportMetadata GetMetadata(InvoiceReportModel model)
        {
            string subtitle = model.DateFrom.HasValue && model.DateTo.HasValue
                ? $"By Customer — {model.DateFrom:dd MMM yyyy} to {model.DateTo:dd MMM yyyy}"
                : "By Customer — All Dates";

            return new ReportMetadata
            {
                ReportTitle     = "Invoice Summary Report",
                Subtitle        = subtitle,
                ReportReference = "RPT-INV-001",
                PreparedBy      = model.PreparedBy,
                Classification  = "Finance — Confidential",
            };
        }

        protected override void BuildContent(Document document, Section section, InvoiceReportModel model)
        {
            var summary = new Dictionary<string, string>
            {
                ["Customers"]   = (model.CustomerGroups?.Count ?? 0).ToString("N0"),
                ["Invoices"]    = model.TotalInvoiceCount.ToString("N0"),
                ["Grand Total"] = $"£{model.GrandTotalGross:N2}",
                ["Outstanding"] = $"£{model.TotalOutstanding:N2}",
            };
            SummaryPanel.Add(section, summary);

            var heading = section.AddParagraph("Invoice Lines by Customer");
            heading.Style = ReportStyles.SectionHeading;

            double[] widths  = { 2.5, 2.5, 2.5, 5.5, 2.5, 2.0, 2.5, 2.5 };
            string[] headers = { "Invoice No", "Invoice Date", "Due Date", "Description", "Net £", "VAT £", "Gross £", "Status" };
            var alignments = new[]
            {
                ParagraphAlignment.Left,
                ParagraphAlignment.Left,
                ParagraphAlignment.Left,
                ParagraphAlignment.Left,
                ParagraphAlignment.Right,
                ParagraphAlignment.Right,
                ParagraphAlignment.Right,
                ParagraphAlignment.Center,
            };

            var builder = ReportTableBuilder.Create(section, widths, headers, alignments);

            foreach (var group in model.CustomerGroups)
            {
                builder.AddGroupHeaderRow($"{group.CustomerName}  ({group.CustomerRef})");

                foreach (var line in group.Lines)
                {
                    var row = builder.AddRow(new[]
                    {
                        line.InvoiceNumber,
                        line.InvoiceDate.ToString("dd/MM/yyyy"),
                        line.DueDate.ToString("dd/MM/yyyy"),
                        line.Description,
                        $"{line.NetAmount:N2}",
                        $"{line.VatAmount:N2}",
                        $"{line.GrossAmount:N2}",
                        line.Status,
                    }, alignments);

                    if (line.Status == "Overdue")
                    {
                        var statusCell = row.Cells[7];
                        statusCell.Shading.Color = Color.Parse(ReportStyles.ColourOverdue);
                        if (statusCell.Elements[0] is Paragraph statusPara)
                            statusPara.Format.Font.Color = Color.Parse(ReportStyles.ColourRed);
                    }
                }

                var custTotals = new string[6];
                custTotals[2] = $"{group.SubtotalNet:N2}";
                custTotals[3] = $"{group.SubtotalVat:N2}";
                custTotals[4] = $"{group.SubtotalGross:N2}";
                builder.AddTotalRow($"Subtotal — {group.CustomerName}", custTotals, labelColIndex: 0);
            }

            var grandTotals = new string[6];
            grandTotals[2] = $"{model.GrandTotalNet:N2}";
            grandTotals[3] = $"{model.GrandTotalVat:N2}";
            grandTotals[4] = $"{model.GrandTotalGross:N2}";
            builder.AddTotalRow("Grand Total", grandTotals, labelColIndex: 0);
        }
    }
}
