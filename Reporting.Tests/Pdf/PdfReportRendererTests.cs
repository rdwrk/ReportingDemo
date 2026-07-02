using System.IO;
using System.Threading.Tasks;
using MigraDoc.DocumentObjectModel;
using Reporting.Core.Templates;
using Reporting.Pdf;
using Reporting.Pdf.Reports;
using Reporting.Pdf.Styles;
using Xunit;

namespace Reporting.Tests.Pdf
{
    /// <summary>
    /// Verifies that <see cref="PdfReportRenderer"/> produces valid PDF output via
    /// <see cref="PdfReportRenderer.Render"/>, <see cref="PdfReportRenderer.BuildAndRender{TModel}"/>,
    /// their stream overloads, and the async convenience wrappers.
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

        private static void AssertValidPdf(MemoryStream ms)
        {
            Assert.True(ms.Length > 0, "Stream must be non-empty.");
            var bytes = ms.ToArray();
            Assert.Equal((byte)'%', bytes[0]);
            Assert.Equal((byte)'P', bytes[1]);
            Assert.Equal((byte)'D', bytes[2]);
            Assert.Equal((byte)'F', bytes[3]);
        }

        // ── Render(Document) ────────────────────────────────────────────────────

        [Fact]
        public void Render_WithMinimalDocument_ReturnsPdfBytes()
        {
            var doc = new Document();
            ReportStyles.Apply(doc);
            doc.AddSection().AddParagraph("Hello PDF");

            var bytes = _renderer.Render(doc);
            AssertValidPdf(bytes);
        }

        // ── Render(Document, Stream) ─────────────────────────────────────────────

        [Fact]
        public void RenderToStream_WithMinimalDocument_WritesPdfToStream()
        {
            var doc = new Document();
            ReportStyles.Apply(doc);
            doc.AddSection().AddParagraph("Hello Stream");

            using (var ms = new MemoryStream())
            {
                _renderer.Render(doc, ms);
                ms.Position = 0;
                AssertValidPdf(ms);
            }
        }

        // ── BuildAndRender ───────────────────────────────────────────────────────

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

        // ── BuildAndRenderToStream ───────────────────────────────────────────────

        [Fact]
        public void BuildAndRenderToStream_WithSalesReportBuilder_WritesPdfToStream()
        {
            var model = new SalesReportModel();
            using (var ms = new MemoryStream())
            {
                _renderer.BuildAndRenderToStream(new SalesReportBuilder(), model, ms);
                ms.Position = 0;
                AssertValidPdf(ms);
            }
        }

        [Fact]
        public void BuildAndRenderToStream_WithInvoiceReportBuilder_WritesPdfToStream()
        {
            var model = new InvoiceReportModel();
            using (var ms = new MemoryStream())
            {
                _renderer.BuildAndRenderToStream(new InvoiceReportBuilder(), model, ms);
                ms.Position = 0;
                AssertValidPdf(ms);
            }
        }

        [Fact]
        public void BuildAndRenderToStream_WithRegionSummaryReportBuilder_WritesPdfToStream()
        {
            var model = new RegionSummaryReportModel();
            using (var ms = new MemoryStream())
            {
                _renderer.BuildAndRenderToStream(new RegionSummaryReportBuilder(), model, ms);
                ms.Position = 0;
                AssertValidPdf(ms);
            }
        }

        // ── Async wrappers ───────────────────────────────────────────────────────

        [Fact]
        public async Task BuildAndRenderAsync_WithSalesReportBuilder_ReturnsPdfBytes()
        {
            var model = new SalesReportModel();
            var bytes = await _renderer.BuildAndRenderAsync(new SalesReportBuilder(), model);
            AssertValidPdf(bytes);
        }

        [Fact]
        public async Task BuildAndRenderAsync_WithInvoiceReportBuilder_ReturnsPdfBytes()
        {
            var model = new InvoiceReportModel();
            var bytes = await _renderer.BuildAndRenderAsync(new InvoiceReportBuilder(), model);
            AssertValidPdf(bytes);
        }

        [Fact]
        public async Task BuildAndRenderAsync_WithRegionSummaryReportBuilder_ReturnsPdfBytes()
        {
            var model = new RegionSummaryReportModel();
            var bytes = await _renderer.BuildAndRenderAsync(new RegionSummaryReportBuilder(), model);
            AssertValidPdf(bytes);
        }

        [Fact]
        public async Task BuildAndRenderToStreamAsync_WithSalesReportBuilder_WritesPdfToStream()
        {
            var model = new SalesReportModel();
            using (var ms = new MemoryStream())
            {
                await _renderer.BuildAndRenderToStreamAsync(new SalesReportBuilder(), model, ms);
                ms.Position = 0;
                AssertValidPdf(ms);
            }
        }

        [Fact]
        public async Task BuildAndRenderToStreamAsync_WithInvoiceReportBuilder_WritesPdfToStream()
        {
            var model = new InvoiceReportModel();
            using (var ms = new MemoryStream())
            {
                await _renderer.BuildAndRenderToStreamAsync(new InvoiceReportBuilder(), model, ms);
                ms.Position = 0;
                AssertValidPdf(ms);
            }
        }

        [Fact]
        public async Task BuildAndRenderToStreamAsync_WithRegionSummaryReportBuilder_WritesPdfToStream()
        {
            var model = new RegionSummaryReportModel();
            using (var ms = new MemoryStream())
            {
                await _renderer.BuildAndRenderToStreamAsync(new RegionSummaryReportBuilder(), model, ms);
                ms.Position = 0;
                AssertValidPdf(ms);
            }
        }
    }
}
