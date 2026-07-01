using System.Collections.Generic;

namespace Reporting.Core.Models
{
    /// <summary>
    /// Groups invoice lines belonging to a single customer.
    /// Subtotals are pre-calculated by the data service when assembling the report.
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
}
