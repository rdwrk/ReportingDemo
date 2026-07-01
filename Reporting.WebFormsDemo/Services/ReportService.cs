using System;
using Reporting.Core.Models;
using Reporting.Core.Templates;
using Reporting.Pdf;
using Reporting.Pdf.Reports;

namespace Reporting.WebFormsDemo.Services
{
    /// <summary>
    /// Coordinates data retrieval and PDF generation. Called by <see cref="Reporting.WebFormsDemo.StreamPdf"/>.
    /// </summary>
    public class ReportService
    {
        private readonly PdfReportRenderer _renderer = new PdfReportRenderer();

        /// <summary>
        /// Generates a PDF for the named report.
        /// </summary>
        /// <param name="reportName">Case-insensitive key: "sales" or "invoice".</param>
        /// <param name="dateFrom">ISO date string for period start.</param>
        /// <param name="dateTo">ISO date string for period end.</param>
        /// <param name="filter">Optional region filter (sales report only).</param>
        /// <param name="preparedBy">Name shown in the report header.</param>
        /// <returns>A <see cref="ReportResult"/> with raw PDF bytes and a suggested filename.</returns>
        public ReportResult Generate(string reportName, string dateFrom, string dateTo,
                                     string filter, string preparedBy)
        {
            var request = new ReportRequest
            {
                DateFrom = TryParse(dateFrom),
                DateTo   = TryParse(dateTo),
                Filter   = filter,
            };

            if (!string.IsNullOrEmpty(preparedBy))
                request.Parameters["preparedBy"] = preparedBy;
            if (!string.IsNullOrEmpty(filter))
                request.Parameters["region"] = filter;

            switch (reportName.ToLowerInvariant())
            {
                case "sales":
                    byte[] salesBytes = _renderer.BuildAndRender(
                        new SalesReportBuilder(),
                        new SalesReportDataService().GetModel(request));
                    return new ReportResult(salesBytes,
                        string.Format("SalesSummary_{0:yyyyMMdd}.pdf", DateTime.Today));

                case "invoice":
                    byte[] invBytes = _renderer.BuildAndRender(
                        new InvoiceReportBuilder(),
                        new InvoiceReportDataService().GetModel(request));
                    return new ReportResult(invBytes,
                        string.Format("InvoiceSummary_{0:yyyyMMdd}.pdf", DateTime.Today));

                case "region-summary":
                    byte[] rgnBytes = _renderer.BuildAndRender(
                        new RegionSummaryReportBuilder(),
                        new RegionSummaryDataService().GetModel(request));
                    return new ReportResult(rgnBytes,
                        string.Format("RegionPerformance_{0:yyyyMMdd}.pdf", DateTime.Today));

                default:
                    throw new ArgumentException("Unknown report: " + reportName);
            }
        }

        private static DateTime? TryParse(string value)
        {
            DateTime d;
            return DateTime.TryParse(value, out d) ? d : (DateTime?)null;
        }
    }

    /// <summary>Holds the output of a PDF generation request.</summary>
    public sealed class ReportResult
    {
        /// <summary>Raw PDF bytes ready to write to an HTTP response.</summary>
        public byte[] Bytes { get; private set; }

        /// <summary>Suggested filename for the Content-Disposition header.</summary>
        public string FileName { get; private set; }

        /// <summary>Initialises a new <see cref="ReportResult"/>.</summary>
        public ReportResult(byte[] bytes, string fileName)
        {
            Bytes    = bytes;
            FileName = fileName;
        }
    }
}
