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
    /// Builds the Invoice Summary Report as a landscape A4 PDF.
    /// Groups invoice lines by customer, with a per-customer subtotal row and a grand total row.
    /// Overdue invoice lines receive a pale-orange cell background and red status text.
    /// Renders no data rows when <see cref="InvoiceReportModel.CustomerGroups"/> is null or empty.
    /// </summary>
    public class InvoiceReportBuilder : MasterReportTemplate<InvoiceReportModel>
    {
        protected override Orientation Orientation => Orientation.Landscape;

        protected override ReportMetadata GetMetadata(InvoiceReportModel model)
        {
            string subtitle = model.DateFrom.HasValue && model.DateTo.HasValue
                ? $"By Customer — {model.DateFrom:dd MMM yyyy} to {model.DateTo:dd MMM yyyy}"
                : "By Customer — All Dates";

            var lines = new List<string> { subtitle };
            if (!string.IsNullOrEmpty(model.PreparedBy))
                lines.Add($"Prepared by: {model.PreparedBy}   |   Ref: RPT-INV-001   |   Finance — Confidential");

            return new ReportMetadata
            {
                ReportTitle = "Invoice Summary Report",
                HeaderLines = lines,
                LogoPath    = LogoProvider.GetPath(),
                LogoWidthCm = 3.5,
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
            SummaryPanel.Add(section, summary, GetContentWidthCm());

            var heading = section.AddParagraph("Invoice Lines by Customer");
            heading.Style = ReportStyles.SectionHeading;

            string[] headers = { "Invoice No", "Invoice Date", "Due Date", "Description", "Net £", "VAT £", "Gross £", "Status" };
            double[] weights = { 1.1, 1.0, 1.0, 3.1, 1.0, 0.8, 1.0, 0.9 };
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

            var builder = ReportTableBuilder.Create(section, GetContentWidthCm(), weights, headers, alignments);

            foreach (var group in model.CustomerGroups ?? Enumerable.Empty<CustomerInvoiceGroup>())
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
