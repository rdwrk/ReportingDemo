using MigraDoc.DocumentObjectModel;
using Reporting.Core.Templates;
using Reporting.Pdf;
using Reporting.Pdf.Reports;
using Reporting.Pdf.Styles;
using Xunit;

namespace Reporting.Tests.Pdf
{
    /// <summary>
    /// Verifies that <see cref="PdfReportRenderer"/> produces valid PDF output via both
    /// <see cref="PdfReportRenderer.Render"/> and <see cref="PdfReportRenderer.BuildAndRender{TModel}"/>.
    /// </summary>
    public class PdfReportRendererTests
    {
        private readonly PdfReportRenderer _renderer = new PdfReportRenderer();

        private static void AssertValidPdf(byte[] bytes)
        {
            Assert.NotNull(bytes);
            Assert.True(bytes.Length > 0, "PDF must be non-empty.");
            // Every PDF file begins with the %PDF magic bytes.
            Assert.Equal((byte)'%', bytes[0]);
            Assert.Equal((byte)'P', bytes[1]);
            Assert.Equal((byte)'D', bytes[2]);
            Assert.Equal((byte)'F', bytes[3]);
        }

        [Fact]
        public void Render_WithMinimalDocument_ReturnsPdfBytes()
        {
            var doc = new Document();
            ReportStyles.Apply(doc);
            doc.AddSection().AddParagraph("Hello PDF");

            var bytes = _renderer.Render(doc);
            AssertValidPdf(bytes);
        }

        [Fact]
        public void BuildAndRender_WithSalesReportBuilder_ReturnsPdfBytes()
        {
            var model = new SalesReportModel();
            var bytes = _renderer.BuildAndRender(new SalesReportBuilder(), model);
            AssertValidPdf(bytes);
        }

        [Fact]
        public void BuildAndRender_WithInvoiceReportBuilder_ReturnsPdfBytes()
        {
            var model = new InvoiceReportModel();
            var bytes = _renderer.BuildAndRender(new InvoiceReportBuilder(), model);
            AssertValidPdf(bytes);
        }

        [Fact]
        public void BuildAndRender_WithRegionSummaryReportBuilder_ReturnsPdfBytes()
        {
            var model = new RegionSummaryReportModel();
            var bytes = _renderer.BuildAndRender(new RegionSummaryReportBuilder(), model);
            AssertValidPdf(bytes);
        }
    }
}
