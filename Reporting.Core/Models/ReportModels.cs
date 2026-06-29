using System;
using System.Collections.Generic;
using System.Linq;
using Reporting.Core.Interfaces;

namespace Reporting.Core.Models
{
    // ── Sales Report ────────────────────────────────────────────────────────────

    /// <summary>
    /// Represents a single line on the sales summary report.
    /// Revenue and gross profit are derived from the other properties.
    /// </summary>
    public class SalesLineItem
    {
        /// <summary>Sales region (e.g. "North", "South").</summary>
        public string Region { get; set; }

        /// <summary>Product or service sold.</summary>
        public string Product { get; set; }

        /// <summary>Name of the sales representative.</summary>
        public string SalesRep { get; set; }

        /// <summary>Number of units sold.</summary>
        public int UnitsSold { get; set; }

        /// <summary>Selling price per unit (excluding VAT).</summary>
        public decimal UnitPrice { get; set; }

        /// <summary>Total revenue for this line: <see cref="UnitsSold"/> × <see cref="UnitPrice"/>.</summary>
        public decimal Revenue => UnitsSold * UnitPrice;

        /// <summary>Total cost of goods for this line.</summary>
        public decimal CostOfGoods { get; set; }

        /// <summary>Gross profit: <see cref="Revenue"/> − <see cref="CostOfGoods"/>.</summary>
        public decimal GrossProfit => Revenue - CostOfGoods;

        /// <summary>Date the sale was recorded.</summary>
        public DateTime SaleDate { get; set; }
    }

    /// <summary>
    /// Top-level model for the Sales Summary report.
    /// Aggregated totals are computed from <see cref="Items"/>.
    /// </summary>
    public class SalesReportModel : IReportModel
    {
        /// <summary>Start of the reporting period.</summary>
        public DateTime? DateFrom { get; set; }

        /// <summary>End of the reporting period.</summary>
        public DateTime? DateTo { get; set; }

        /// <summary>Region filter applied when building this model. Null or empty means all regions.</summary>
        public string RegionFilter { get; set; }

        /// <summary>Person or team that requested the report.</summary>
        public string PreparedBy { get; set; }

        /// <summary>All sales lines included in this report.</summary>
        public List<SalesLineItem> Items { get; set; } = new List<SalesLineItem>();

        /// <summary>Sum of <see cref="SalesLineItem.Revenue"/> across all items.</summary>
        public decimal TotalRevenue => Items?.Sum(i => i.Revenue) ?? 0;

        /// <summary>Sum of <see cref="SalesLineItem.GrossProfit"/> across all items.</summary>
        public decimal TotalGrossProfit => Items?.Sum(i => i.GrossProfit) ?? 0;
    }

    // ── Invoice Report ───────────────────────────────────────────────────────────

    /// <summary>
    /// A single invoice line within a customer group on the invoice report.
    /// Gross amount is derived from net + VAT.
    /// </summary>
    public class InvoiceLine
    {
        /// <summary>Unique invoice reference number (e.g. "INV-00042").</summary>
        public string InvoiceNumber { get; set; }

        /// <summary>Date the invoice was raised.</summary>
        public DateTime InvoiceDate { get; set; }

        /// <summary>Payment due date.</summary>
        public DateTime DueDate { get; set; }

        /// <summary>Description of the goods or services invoiced.</summary>
        public string Description { get; set; }

        /// <summary>Net amount excluding VAT.</summary>
        public decimal NetAmount { get; set; }

        /// <summary>VAT amount.</summary>
        public decimal VatAmount { get; set; }

        /// <summary>Gross amount: <see cref="NetAmount"/> + <see cref="VatAmount"/>.</summary>
        public decimal GrossAmount => NetAmount + VatAmount;

        /// <summary>Payment status: "Paid", "Due", or "Overdue".</summary>
        public string Status { get; set; }
    }

    /// <summary>
    /// Groups invoice lines belonging to a single customer.
    /// Subtotals are pre-calculated by the data service.
    /// </summary>
    public class CustomerInvoiceGroup
    {
        /// <summary>Customer display name.</summary>
        public string CustomerName { get; set; }

        /// <summary>Internal customer reference code.</summary>
        public string CustomerRef { get; set; }

        /// <summary>All invoice lines for this customer.</summary>
        public List<InvoiceLine> Lines { get; set; } = new List<InvoiceLine>();

        /// <summary>Sum of net amounts across all lines.</summary>
        public decimal SubtotalNet { get; set; }

        /// <summary>Sum of VAT amounts across all lines.</summary>
        public decimal SubtotalVat { get; set; }

        /// <summary>Sum of gross amounts across all lines.</summary>
        public decimal SubtotalGross { get; set; }
    }

    /// <summary>
    /// Top-level model for the Invoice Summary report.
    /// Contains one <see cref="CustomerInvoiceGroup"/> per customer,
    /// plus grand totals pre-calculated by the data service.
    /// </summary>
    public class InvoiceReportModel : IReportModel
    {
        /// <summary>Start of the reporting period.</summary>
        public DateTime? DateFrom { get; set; }

        /// <summary>End of the reporting period.</summary>
        public DateTime? DateTo { get; set; }

        /// <summary>Person or team that requested the report.</summary>
        public string PreparedBy { get; set; }

        /// <summary>One group per customer, ordered as returned by the data service.</summary>
        public List<CustomerInvoiceGroup> CustomerGroups { get; set; } = new List<CustomerInvoiceGroup>();

        /// <summary>Total number of invoice lines across all customers.</summary>
        public int TotalInvoiceCount { get; set; }

        /// <summary>Grand total of all net amounts.</summary>
        public decimal GrandTotalNet { get; set; }

        /// <summary>Grand total of all VAT amounts.</summary>
        public decimal GrandTotalVat { get; set; }

        /// <summary>Grand total of all gross amounts.</summary>
        public decimal GrandTotalGross { get; set; }

        /// <summary>Sum of gross amounts for lines with status "Due" or "Overdue".</summary>
        public decimal TotalOutstanding { get; set; }
    }

    // ── Region Summary Report ────────────────────────────────────────────────────

    /// <summary>Aggregated sales performance for a single region.</summary>
    public class RegionRow
    {
        /// <summary>Region name (e.g. "North").</summary>
        public string Region { get; set; }

        /// <summary>Number of individual sales lines in this region.</summary>
        public int LineCount { get; set; }

        /// <summary>Total revenue across all lines.</summary>
        public decimal Revenue { get; set; }

        /// <summary>Total gross profit across all lines.</summary>
        public decimal GrossProfit { get; set; }

        /// <summary>Gross profit margin: <see cref="GrossProfit"/> / <see cref="Revenue"/>.</summary>
        public decimal Margin => Revenue > 0 ? GrossProfit / Revenue : 0;
    }

    /// <summary>
    /// Top-level model for the Region Performance Overview report.
    /// Aggregates all sales data by region — no date or filter parameters required.
    /// Grand totals are derived from <see cref="Rows"/>.
    /// </summary>
    public class RegionSummaryReportModel : IReportModel
    {
        /// <summary>One row per region, ordered alphabetically by region name.</summary>
        public List<RegionRow> Rows { get; set; } = new List<RegionRow>();

        /// <summary>Total sales lines across all regions.</summary>
        public int GrandLineCount => Rows?.Sum(r => r.LineCount) ?? 0;

        /// <summary>Total revenue across all regions.</summary>
        public decimal GrandRevenue => Rows?.Sum(r => r.Revenue) ?? 0;

        /// <summary>Total gross profit across all regions.</summary>
        public decimal GrandGrossProfit => Rows?.Sum(r => r.GrossProfit) ?? 0;

        /// <summary>Overall gross profit margin.</summary>
        public decimal GrandMargin => GrandRevenue > 0 ? GrandGrossProfit / GrandRevenue : 0;
    }
}
