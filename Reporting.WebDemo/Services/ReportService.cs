using System;
using Reporting.Core.Templates;
using Reporting.Pdf;
using Reporting.Pdf.Reports;

namespace Reporting.WebDemo.Services
{
    /// <summary>
    /// Application-layer service that renders a pre-populated report model to PDF bytes.
    /// The calling code (page handler or endpoint) is responsible for fetching data from
    /// the appropriate DAL service and assembling the Core model before calling these methods.
    /// </summary>
    public class ReportService
    {
        private readonly PdfReportRenderer _renderer = new PdfReportRenderer();

        /// <summary>Renders a <see cref="SalesReportModel"/> to PDF bytes.</summary>
        /// <returns>Raw PDF bytes and a suggested download filename.</returns>
        public (byte[] bytes, string fileName) GenerateSales(SalesReportModel model)
        {
            var bytes = _renderer.BuildAndRender(new SalesReportBuilder(), model);
            return (bytes, $"SalesSummary_{DateTime.Today:yyyyMMdd}.pdf");
        }

        /// <summary>Renders an <see cref="InvoiceReportModel"/> to PDF bytes.</summary>
        /// <returns>Raw PDF bytes and a suggested download filename.</returns>
        public (byte[] bytes, string fileName) GenerateInvoice(InvoiceReportModel model)
        {
            var bytes = _renderer.BuildAndRender(new InvoiceReportBuilder(), model);
            return (bytes, $"InvoiceSummary_{DateTime.Today:yyyyMMdd}.pdf");
        }

        /// <summary>Renders a <see cref="RegionSummaryReportModel"/> to PDF bytes.</summary>
        /// <returns>Raw PDF bytes and a suggested download filename.</returns>
        public (byte[] bytes, string fileName) GenerateRegionSummary(RegionSummaryReportModel model)
        {
            var bytes = _renderer.BuildAndRender(new RegionSummaryReportBuilder(), model);
            return (bytes, $"RegionPerformance_{DateTime.Today:yyyyMMdd}.pdf");
        }
    }
}
