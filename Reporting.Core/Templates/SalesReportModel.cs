using System.Collections.Generic;
using System.Linq;
using Reporting.Core.Interfaces;
using Reporting.Core.Models;

namespace Reporting.Core.Templates
{
    /// <summary>
    /// Report model for the Sales Summary report.
    /// Contains only the raw data entities (<see cref="Items"/>) and derived display totals.
    /// All report context (date range, region filter, prepared-by) belongs in <see cref="Metadata"/>
    /// and is set by the calling code before the model is passed to the renderer.
    /// </summary>
    public class SalesReportModel : IReportModel
    {
        /// <summary>
        /// Report-level context set by the calling code from user input.
        /// The builder reads <see cref="ReportMetadata.PeriodFrom"/>, <see cref="ReportMetadata.PeriodTo"/>,
        /// <see cref="ReportMetadata.Filter"/>, and <see cref="ReportMetadata.PreparedBy"/> to format the page header.
        /// </summary>
        public ReportMetadata Metadata { get; set; } = new ReportMetadata();

        /// <summary>All sales lines included in this report.</summary>
        public List<SalesLineItem> Items { get; set; } = new List<SalesLineItem>();

        /// <summary>Display expression: sum of <see cref="SalesLineItem.Revenue"/> across all items.</summary>
        public decimal TotalRevenue => Items?.Sum(i => i.Revenue) ?? 0;

        /// <summary>Display expression: sum of <see cref="SalesLineItem.GrossProfit"/> across all items.</summary>
        public decimal TotalGrossProfit => Items?.Sum(i => i.GrossProfit) ?? 0;
    }
}
