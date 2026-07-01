using System;
using System.Collections.Generic;
using Reporting.Core.Interfaces;
using Reporting.Core.Models;

namespace Reporting.Core.Templates
{
    /// <summary>
    /// Assembled report model for the Invoice Summary report.
    /// Passed to <c>InvoiceReportBuilder</c> after population by the data service.
    /// Grand totals are pre-calculated by the data service and stored directly,
    /// as the source lines are grouped and the flat total is not re-derivable cheaply.
    /// </summary>
    public class InvoiceReportModel : IReportModel
    {
        /// <summary>Start of the reporting period.</summary>
        public DateTime? DateFrom { get; set; }

        /// <summary>End of the reporting period.</summary>
        public DateTime? DateTo { get; set; }

        /// <summary>Person or team that requested the report, shown in the page header.</summary>
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
