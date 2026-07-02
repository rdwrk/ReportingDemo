using System.Collections.Generic;
using Reporting.Core.Models;
using Reporting.Core.Templates;
using Reporting.Pdf;
using Reporting.Pdf.Reports;
using Xunit;

namespace Reporting.Tests.Pdf
{
    /// <summary>
    /// Verifies all layout branches in <see cref="RegionSummaryReportBuilder"/>:
    /// null/empty Rows handling and a populated model with a positive margin.
    /// Each test renders a full PDF to confirm no exceptions are thrown.
    /// </summary>
    public class RegionSummaryReportBuilderTests
    {
        private readonly PdfReportRenderer          _renderer = new PdfReportRenderer();
        private readonly RegionSummaryReportBuilder _builder  = new RegionSummaryReportBuilder();

        private static void AssertValidPdf(byte[] bytes)
        {
            Assert.NotNull(bytes);
            Assert.True(bytes.Length > 0);
            Assert.Equal((byte)'%', bytes[0]);
            Assert.Equal((byte)'P', bytes[1]);
            Assert.Equal((byte)'D', bytes[2]);
            Assert.Equal((byte)'F', bytes[3]);
        }

        private byte[] Render(RegionSummaryReportModel model) =>
            _renderer.BuildAndRender(_builder, model);

        // ── Null/empty Rows branches ─────────────────────────────────────────────

        [Fact]
        public void Build_WithNullRows_RendersValidPdf()
        {
            // Exercises the null-guard on model.Rows and model.Rows?.Count
            var model = new RegionSummaryReportModel { Rows = null };
            AssertValidPdf(Render(model));
        }

        [Fact]
        public void Build_WithEmptyRows_RendersValidPdf()
        {
            var model = new RegionSummaryReportModel { Rows = new List<RegionRow>() };
            AssertValidPdf(Render(model));
        }

        // ── Populated data branches ──────────────────────────────────────────────

        [Fact]
        public void Build_WithRows_RendersDataAndGrandTotalRows()
        {
            var model = new RegionSummaryReportModel
            {
                Rows = new List<RegionRow>
                {
                    new RegionRow { Region = "North",    LineCount = 40, Revenue = 20_000m, GrossProfit = 6_000m  },
                    new RegionRow { Region = "South",    LineCount = 30, Revenue = 15_000m, GrossProfit = 4_500m  },
                    new RegionRow { Region = "Midlands", LineCount = 50, Revenue = 25_000m, GrossProfit = 10_000m },
                }
            };
            AssertValidPdf(Render(model));
        }

        [Fact]
        public void Build_WithZeroRevenueRow_RendersZeroMarginWithoutThrowing()
        {
            // RegionRow.Margin returns 0 when Revenue is 0 — no divide-by-zero.
            var model = new RegionSummaryReportModel
            {
                Rows = new List<RegionRow>
                {
                    new RegionRow { Region = "East", LineCount = 0, Revenue = 0m, GrossProfit = 0m },
                }
            };
            AssertValidPdf(Render(model));
        }
    }
}
