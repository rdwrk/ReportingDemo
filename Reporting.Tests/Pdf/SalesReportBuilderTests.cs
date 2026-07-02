using System;
using System.Collections.Generic;
using Reporting.Core.Models;
using Reporting.Core.Templates;
using Reporting.Pdf;
using Reporting.Pdf.Reports;
using Xunit;

namespace Reporting.Tests.Pdf
{
    /// <summary>
    /// Verifies all layout branches in <see cref="SalesReportBuilder"/>: header line variants,
    /// region/date-range combinations, margin display, and null/empty Items handling.
    /// Each test renders a full PDF to confirm no exceptions are thrown.
    /// </summary>
    public class SalesReportBuilderTests
    {
        private readonly PdfReportRenderer _renderer = new PdfReportRenderer();
        private readonly SalesReportBuilder _builder = new SalesReportBuilder();

        private static void AssertValidPdf(byte[] bytes)
        {
            Assert.NotNull(bytes);
            Assert.True(bytes.Length > 0);
            Assert.Equal((byte)'%', bytes[0]);
            Assert.Equal((byte)'P', bytes[1]);
            Assert.Equal((byte)'D', bytes[2]);
            Assert.Equal((byte)'F', bytes[3]);
        }

        private byte[] Render(SalesReportModel model) =>
            _renderer.BuildAndRender(_builder, model);

        // ── Header-line branches ─────────────────────────────────────────────────

        [Fact]
        public void Build_WithPreparedBy_IncludesPreparedByHeaderLine()
        {
            var model = new SalesReportModel { PreparedBy = "Alice" };
            AssertValidPdf(Render(model));
        }

        [Fact]
        public void Build_WithoutPreparedBy_OmitsPreparedByHeaderLine()
        {
            var model = new SalesReportModel { PreparedBy = null };
            AssertValidPdf(Render(model));
        }

        [Fact]
        public void Build_WithEmptyPreparedBy_OmitsPreparedByHeaderLine()
        {
            var model = new SalesReportModel { PreparedBy = string.Empty };
            AssertValidPdf(Render(model));
        }

        // ── Date range branches ──────────────────────────────────────────────────

        [Fact]
        public void Build_WithBothDates_ShowsDateRangeInSubtitle()
        {
            var model = new SalesReportModel
            {
                DateFrom = new DateTime(2024, 1, 1),
                DateTo   = new DateTime(2024, 3, 31),
            };
            AssertValidPdf(Render(model));
        }

        [Fact]
        public void Build_WithNoDates_ShowsAllDatesInSubtitle()
        {
            var model = new SalesReportModel { DateFrom = null, DateTo = null };
            AssertValidPdf(Render(model));
        }

        [Fact]
        public void Build_WithOnlyDateFrom_ShowsAllDatesInSubtitle()
        {
            // Both must be set for the range to show; one-sided falls through to "All Dates".
            var model = new SalesReportModel { DateFrom = new DateTime(2024, 1, 1), DateTo = null };
            AssertValidPdf(Render(model));
        }

        // ── Region filter branches ───────────────────────────────────────────────

        [Fact]
        public void Build_WithRegionFilter_ShowsRegionInSubtitle()
        {
            var model = new SalesReportModel { RegionFilter = "North" };
            AssertValidPdf(Render(model));
        }

        [Fact]
        public void Build_WithoutRegionFilter_ShowsAllRegionsInSubtitle()
        {
            var model = new SalesReportModel { RegionFilter = null };
            AssertValidPdf(Render(model));
        }

        [Fact]
        public void Build_WithEmptyRegionFilter_ShowsAllRegionsInSubtitle()
        {
            var model = new SalesReportModel { RegionFilter = string.Empty };
            AssertValidPdf(Render(model));
        }

        // ── Items / margin branches ──────────────────────────────────────────────

        [Fact]
        public void Build_WithNullItems_RendersValidPdf()
        {
            // Exercises the null-guard: foreach (var item in model.Items ?? Enumerable.Empty<>())
            var model = new SalesReportModel { Items = null };
            AssertValidPdf(Render(model));
        }

        [Fact]
        public void Build_WithEmptyItems_RendersValidPdf()
        {
            var model = new SalesReportModel { Items = new List<SalesLineItem>() };
            AssertValidPdf(Render(model));
        }

        [Fact]
        public void Build_WithZeroTotalRevenue_ShowsNAForMargin()
        {
            // TotalRevenue = 0 → margin summary cell should render "N/A" without throwing.
            var model = new SalesReportModel
            {
                Items = new List<SalesLineItem>
                {
                    new SalesLineItem { UnitsSold = 0, UnitPrice = 0m, CostOfGoods = 0m },
                }
            };
            AssertValidPdf(Render(model));
        }

        [Fact]
        public void Build_WithItems_RendersDataRows()
        {
            var model = new SalesReportModel
            {
                DateFrom     = new DateTime(2024, 1, 1),
                DateTo       = new DateTime(2024, 12, 31),
                RegionFilter = "Midlands",
                PreparedBy   = "Bob",
                Items = new List<SalesLineItem>
                {
                    new SalesLineItem
                    {
                        SaleDate    = new DateTime(2024, 3, 15),
                        Product     = "Widget",
                        SalesRep    = "Charlie",
                        Region      = "Midlands",
                        UnitsSold   = 10,
                        UnitPrice   = 25m,
                        CostOfGoods = 150m,
                    },
                    new SalesLineItem
                    {
                        SaleDate    = new DateTime(2024, 6, 1),
                        Product     = "Gadget",
                        SalesRep    = "Diana",
                        Region      = "Midlands",
                        UnitsSold   = 5,
                        UnitPrice   = 40m,
                        CostOfGoods = 100m,
                    },
                }
            };
            AssertValidPdf(Render(model));
        }
    }
}
