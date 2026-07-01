using System;
using System.Collections.Generic;
using System.Linq;
using Reporting.Core.Interfaces;
using Reporting.Core.Models;

namespace Reporting.Core.Templates
{
    /// <summary>
    /// Assembled report model for the Sales Summary report.
    /// Passed to <c>SalesReportBuilder</c> after population by the data service.
    /// Grand totals are display expressions derived from <see cref="Items"/>.
    /// </summary>
    public class SalesReportModel : IReportModel
    {
        /// <summary>Start of the reporting period.</summary>
        public DateTime? DateFrom { get; set; }

        /// <summary>End of the reporting period.</summary>
        public DateTime? DateTo { get; set; }

        /// <summary>Region filter applied to this model. Null or empty means all regions.</summary>
        public string RegionFilter { get; set; }

        /// <summary>Person or team that requested the report, shown in the page header.</summary>
        public string PreparedBy { get; set; }

        /// <summary>All sales lines included in this report.</summary>
        public List<SalesLineItem> Items { get; set; } = new List<SalesLineItem>();

        /// <summary>Display expression: sum of <see cref="SalesLineItem.Revenue"/> across all items.</summary>
        public decimal TotalRevenue => Items?.Sum(i => i.Revenue) ?? 0;

        /// <summary>Display expression: sum of <see cref="SalesLineItem.GrossProfit"/> across all items.</summary>
        public decimal TotalGrossProfit => Items?.Sum(i => i.GrossProfit) ?? 0;
    }
}
