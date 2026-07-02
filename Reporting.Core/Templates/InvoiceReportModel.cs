using System.Collections.Generic;
using Reporting.Core.Interfaces;
using Reporting.Core.Models;

namespace Reporting.Core.Templates
{
    /// <summary>
    /// Report model for the Invoice Summary report.
    /// Contains only the raw data entities (<see cref="CustomerGroups"/> and grand totals).
    /// All report context (date range, prepared-by) belongs in <see cref="Metadata"/>
    /// and is set by the calling code before the model is passed to the renderer.
    /// Grand totals are pre-calculated by the DAL service and stored directly,
    /// as the source lines are grouped and the flat total is not re-derivable cheaply.
    /// </summary>
    public class InvoiceReportModel : IReportModel
    {
        /// <summary>
        /// Report-level context set by the calling code from user input.
        /// The builder reads <see cref="ReportMetadata.PeriodFrom"/>, <see cref="ReportMetadata.PeriodTo"/>,
        /// and <see cref="ReportMetadata.PreparedBy"/> to format the page header.
        /// </summary>
        public ReportMetadata Metadata { get; set; } = new ReportMetadata();

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
