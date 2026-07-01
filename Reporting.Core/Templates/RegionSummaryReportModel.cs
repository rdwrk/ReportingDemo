using System.Collections.Generic;
using System.Linq;
using Reporting.Core.Interfaces;
using Reporting.Core.Models;

namespace Reporting.Core.Templates
{
    /// <summary>
    /// Assembled report model for the Region Performance Overview.
    /// Passed to <c>RegionSummaryReportBuilder</c> after population by the data service.
    /// Grand totals are display expressions derived from <see cref="Rows"/>.
    /// No request parameters are used — the report always covers the full dataset.
    /// </summary>
    public class RegionSummaryReportModel : IReportModel
    {
        /// <summary>One row per region, ordered alphabetically.</summary>
        public List<RegionRow> Rows { get; set; } = new List<RegionRow>();

        /// <summary>Display expression: total sales lines across all regions.</summary>
        public int GrandLineCount => Rows?.Sum(r => r.LineCount) ?? 0;

        /// <summary>Display expression: total revenue across all regions.</summary>
        public decimal GrandRevenue => Rows?.Sum(r => r.Revenue) ?? 0;

        /// <summary>Display expression: total gross profit across all regions.</summary>
        public decimal GrandGrossProfit => Rows?.Sum(r => r.GrossProfit) ?? 0;

        /// <summary>Display expression: overall gross profit margin.</summary>
        public decimal GrandMargin => GrandRevenue > 0 ? GrandGrossProfit / GrandRevenue : 0;
    }
}
