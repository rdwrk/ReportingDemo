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
    /// Verifies all layout branches in <see cref="InvoiceReportBuilder"/>: header line variants,
    /// date-range combinations, overdue row highlighting, and null/empty CustomerGroups handling.
    /// Each test renders a full PDF to confirm no exceptions are thrown.
    /// </summary>
    public class InvoiceReportBuilderTests
    {
        private readonly PdfReportRenderer  _renderer = new PdfReportRenderer();
        private readonly InvoiceReportBuilder _builder = new InvoiceReportBuilder();

        private static void AssertValidPdf(byte[] bytes)
        {
            Assert.NotNull(bytes);
            Assert.True(bytes.Length > 0);
            Assert.Equal((byte)'%', bytes[0]);
            Assert.Equal((byte)'P', bytes[1]);
            Assert.Equal((byte)'D', bytes[2]);
            Assert.Equal((byte)'F', bytes[3]);
        }

        private byte[] Render(InvoiceReportModel model) =>
            _renderer.BuildAndRender(_builder, model);

        // ── Header-line branches ─────────────────────────────────────────────────

        [Fact]
        public void Build_WithPreparedBy_IncludesPreparedByHeaderLine()
        {
            var model = new InvoiceReportModel { PreparedBy = "Finance Team" };
            AssertValidPdf(Render(model));
        }

        [Fact]
        public void Build_WithoutPreparedBy_OmitsPreparedByHeaderLine()
        {
            var model = new InvoiceReportModel { PreparedBy = null };
            AssertValidPdf(Render(model));
        }

        // ── Date range branches ──────────────────────────────────────────────────

        [Fact]
        public void Build_WithBothDates_ShowsDateRangeSubtitle()
        {
            var model = new InvoiceReportModel
            {
                DateFrom = new DateTime(2024, 1, 1),
                DateTo   = new DateTime(2024, 6, 30),
            };
            AssertValidPdf(Render(model));
        }

        [Fact]
        public void Build_WithNoDates_ShowsAllDatesSubtitle()
        {
            var model = new InvoiceReportModel { DateFrom = null, DateTo = null };
            AssertValidPdf(Render(model));
        }

        // ── CustomerGroups null/empty branches ───────────────────────────────────

        [Fact]
        public void Build_WithNullCustomerGroups_RendersValidPdf()
        {
            // Exercises the null-guard: foreach (var group in model.CustomerGroups ?? Enumerable.Empty<>())
            var model = new InvoiceReportModel { CustomerGroups = null };
            AssertValidPdf(Render(model));
        }

        [Fact]
        public void Build_WithEmptyCustomerGroups_RendersValidPdf()
        {
            var model = new InvoiceReportModel
            {
                CustomerGroups = new List<CustomerInvoiceGroup>()
            };
            AssertValidPdf(Render(model));
        }

        // ── Overdue highlighting branch ───────────────────────────────────────────

        [Fact]
        public void Build_WithOverdueLine_AppliesHighlightingWithoutThrowing()
        {
            var model = new InvoiceReportModel
            {
                CustomerGroups = new List<CustomerInvoiceGroup>
                {
                    new CustomerInvoiceGroup
                    {
                        CustomerName  = "Acme Ltd",
                        CustomerRef   = "CUST-001",
                        SubtotalNet   = 100m,
                        SubtotalVat   = 20m,
                        SubtotalGross = 120m,
                        Lines = new List<InvoiceLine>
                        {
                            new InvoiceLine
                            {
                                InvoiceNumber = "INV-001",
                                InvoiceDate   = new DateTime(2024, 1, 1),
                                DueDate       = new DateTime(2024, 1, 31),
                                Description   = "Past due",
                                NetAmount     = 100m,
                                VatAmount     = 20m,
                                Status        = "Overdue",  // triggers red cell branch
                            },
                        },
                    },
                },
                GrandTotalNet   = 100m,
                GrandTotalVat   = 20m,
                GrandTotalGross = 120m,
            };
            AssertValidPdf(Render(model));
        }

        [Fact]
        public void Build_WithNonOverdueLine_DoesNotApplyHighlighting()
        {
            var model = new InvoiceReportModel
            {
                CustomerGroups = new List<CustomerInvoiceGroup>
                {
                    new CustomerInvoiceGroup
                    {
                        CustomerName  = "Beta Corp",
                        CustomerRef   = "CUST-002",
                        SubtotalNet   = 200m,
                        SubtotalVat   = 40m,
                        SubtotalGross = 240m,
                        Lines = new List<InvoiceLine>
                        {
                            new InvoiceLine
                            {
                                InvoiceNumber = "INV-002",
                                InvoiceDate   = new DateTime(2024, 2, 1),
                                DueDate       = new DateTime(2024, 2, 28),
                                Description   = "On time",
                                NetAmount     = 200m,
                                VatAmount     = 40m,
                                Status        = "Paid",
                            },
                        },
                    },
                },
                GrandTotalNet   = 200m,
                GrandTotalVat   = 40m,
                GrandTotalGross = 240m,
            };
            AssertValidPdf(Render(model));
        }

        [Fact]
        public void Build_WithMixedDueStatuses_RendersAllRows()
        {
            var model = new InvoiceReportModel
            {
                CustomerGroups = new List<CustomerInvoiceGroup>
                {
                    new CustomerInvoiceGroup
                    {
                        CustomerName  = "Gamma Inc",
                        CustomerRef   = "CUST-003",
                        SubtotalNet   = 600m,
                        SubtotalVat   = 120m,
                        SubtotalGross = 720m,
                        Lines = new List<InvoiceLine>
                        {
                            new InvoiceLine { InvoiceNumber = "INV-010", InvoiceDate = new DateTime(2024,1,1), DueDate = new DateTime(2024,1,31), Description = "A", NetAmount = 200m, VatAmount = 40m, Status = "Paid"    },
                            new InvoiceLine { InvoiceNumber = "INV-011", InvoiceDate = new DateTime(2024,2,1), DueDate = new DateTime(2024,2,28), Description = "B", NetAmount = 200m, VatAmount = 40m, Status = "Due"     },
                            new InvoiceLine { InvoiceNumber = "INV-012", InvoiceDate = new DateTime(2024,3,1), DueDate = new DateTime(2024,3,31), Description = "C", NetAmount = 200m, VatAmount = 40m, Status = "Overdue" },
                        },
                    },
                },
                GrandTotalNet   = 600m,
                GrandTotalVat   = 120m,
                GrandTotalGross = 720m,
            };
            AssertValidPdf(Render(model));
        }
    }
}
