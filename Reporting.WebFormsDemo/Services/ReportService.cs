using System;
using Reporting.Core.Templates;
using Reporting.Pdf;
using Reporting.Pdf.Reports;

namespace Reporting.WebFormsDemo.Services
{
    /// <summary>
    /// Application-layer service that renders a pre-populated report model to PDF bytes.
    /// The calling code (<see cref="Reporting.WebFormsDemo.StreamPdf"/>) is responsible for
    /// fetching data from the appropriate DAL service and assembling the Core model before
    /// calling these methods.
    /// </summary>
    public class ReportService
    {
        private readonly PdfReportRenderer _renderer = new PdfReportRenderer();

        /// <summary>Renders a <see cref="SalesReportModel"/> to PDF bytes.</summary>
        /// <returns>A <see cref="ReportResult"/> with raw PDF bytes and a suggested filename.</returns>
        public ReportResult GenerateSales(SalesReportModel model)
        {
            byte[] bytes = _renderer.BuildAndRender(new SalesReportBuilder(), model);
            return new ReportResult(bytes, string.Format("SalesSummary_{0:yyyyMMdd}.pdf", DateTime.Today));
        }

        /// <summary>Renders an <see cref="InvoiceReportModel"/> to PDF bytes.</summary>
        /// <returns>A <see cref="ReportResult"/> with raw PDF bytes and a suggested filename.</returns>
        public ReportResult GenerateInvoice(InvoiceReportModel model)
        {
            byte[] bytes = _renderer.BuildAndRender(new InvoiceReportBuilder(), model);
            return new ReportResult(bytes, string.Format("InvoiceSummary_{0:yyyyMMdd}.pdf", DateTime.Today));
        }

        /// <summary>Renders a <see cref="RegionSummaryReportModel"/> to PDF bytes.</summary>
        /// <returns>A <see cref="ReportResult"/> with raw PDF bytes and a suggested filename.</returns>
        public ReportResult GenerateRegionSummary(RegionSummaryReportModel model)
        {
            byte[] bytes = _renderer.BuildAndRender(new RegionSummaryReportBuilder(), model);
            return new ReportResult(bytes, string.Format("RegionPerformance_{0:yyyyMMdd}.pdf", DateTime.Today));
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
