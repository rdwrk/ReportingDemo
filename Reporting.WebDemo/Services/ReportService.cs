using System;
using Reporting.Core.Templates;
using Reporting.Pdf;
using Reporting.Pdf.Reports;

namespace Reporting.WebDemo.Services
{
    /// <summary>
    /// Application-layer service that coordinates data retrieval and PDF generation.
    /// Called by the <c>/reports/stream</c> endpoint in Program.cs.
    /// To add a new report: add a case to <see cref="Generate"/> and register the
    /// corresponding data service and builder classes.
    /// </summary>
    public class ReportService
    {
        private readonly PdfReportRenderer _renderer = new PdfReportRenderer();

        /// <summary>
        /// Generates a PDF for the named report using the supplied parameters.
        /// </summary>
        /// <param name="reportName">Case-insensitive report key: "sales", "invoice", or "region-summary".</param>
        /// <param name="dateFrom">ISO date string for the start of the reporting period (nullable).</param>
        /// <param name="dateTo">ISO date string for the end of the reporting period (nullable).</param>
        /// <param name="filter">Optional free-text filter (used as region filter for the sales report).</param>
        /// <param name="preparedBy">Name or team shown in the report header.</param>
        /// <returns>A tuple of the raw PDF bytes and a suggested download filename.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="reportName"/> is not recognised.</exception>
        public (byte[] bytes, string fileName) Generate(
            string reportName, string? dateFrom, string? dateTo,
            string? filter, string? preparedBy)
        {
            var from = TryParse(dateFrom);
            var to   = TryParse(dateTo);

            switch (reportName.ToLowerInvariant())
            {
                case "sales":
                    var salesModel = new SalesReportDataService().GetModel(from, to, filter, preparedBy);
                    return (_renderer.BuildAndRender(new SalesReportBuilder(), salesModel),
                            $"SalesSummary_{DateTime.Today:yyyyMMdd}.pdf");

                case "invoice":
                    var invModel = new InvoiceReportDataService().GetModel(from, to, preparedBy);
                    return (_renderer.BuildAndRender(new InvoiceReportBuilder(), invModel),
                            $"InvoiceSummary_{DateTime.Today:yyyyMMdd}.pdf");

                case "region-summary":
                    var rgnModel = new RegionSummaryDataService().GetModel();
                    return (_renderer.BuildAndRender(new RegionSummaryReportBuilder(), rgnModel),
                            $"RegionPerformance_{DateTime.Today:yyyyMMdd}.pdf");

                default:
                    throw new ArgumentException($"Unknown report: {reportName}");
            }
        }

        /// <summary>Parses an ISO date string, returning null if the string is null or unparseable.</summary>
        private static DateTime? TryParse(string? value) =>
            DateTime.TryParse(value, out var d) ? d : null;
    }
}
