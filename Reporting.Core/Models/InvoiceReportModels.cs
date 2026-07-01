using System;
using System.Collections.Generic;
using Reporting.Core.Interfaces;

namespace Reporting.Core.Models
{
    /// <summary>A single invoice line within a customer group on the Invoice Summary report.</summary>
    public class InvoiceLine
    {
        /// <summary>Unique invoice reference (e.g. "INV-00042").</summary>
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

        /// <summary>Payment status: "Paid", "Due", or "Overdue".</summary>
        public string Status { get; set; }

        /// <summary>Gross amount: <see cref="NetAmount"/> + <see cref="VatAmount"/>.</summary>
        public decimal GrossAmount => NetAmount + VatAmount;
    }

    /// <summary>Groups invoice lines belonging to a single customer, with pre-calculated subtotals.</summary>
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
    /// Contains one <see cref="CustomerInvoiceGroup"/> per customer plus pre-calculated grand totals.
    /// </summary>
    public class InvoiceReportModel : IReportModel
    {
        /// <summary>Start of the reporting period.</summary>
        public DateTime? DateFrom { get; set; }

        /// <summary>End of the reporting period.</summary>
        public DateTime? DateTo { get; set; }

        /// <summary>Person or team that requested the report, shown in the header.</summary>
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
}
