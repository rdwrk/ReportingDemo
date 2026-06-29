using Reporting.Core.Models;
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
        /// <param name="reportName">Case-insensitive report key: "sales" or "invoice".</param>
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
                    var salesModel = new SalesReportDataService().GetModel(request);
                    return (_renderer.BuildAndRender(new SalesReportBuilder(), salesModel),
                            $"SalesSummary_{DateTime.Today:yyyyMMdd}.pdf");

                case "invoice":
                    var invModel = new InvoiceReportDataService().GetModel(request);
                    return (_renderer.BuildAndRender(new InvoiceReportBuilder(), invModel),
                            $"InvoiceSummary_{DateTime.Today:yyyyMMdd}.pdf");

                case "region-summary":
                    var rgnModel = new RegionSummaryDataService().GetModel(request);
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
