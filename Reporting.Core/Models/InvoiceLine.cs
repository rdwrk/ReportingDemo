using System;

namespace Reporting.Core.Models
{
    /// <summary>
    /// A single invoice line as returned from the data source.
    /// <see cref="GrossAmount"/> is a display expression derived from the raw fields.
    /// </summary>
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

        /// <summary>Display expression: <see cref="NetAmount"/> + <see cref="VatAmount"/>.</summary>
        public decimal GrossAmount => NetAmount + VatAmount;
    }
}
