using System.Collections.Generic;
using System.Linq;
using Reporting.Core.Interfaces;

namespace Reporting.Core.Models
{
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

        /// <summary>Gross profit margin: <see cref="GrossProfit"/> ÷ <see cref="Revenue"/>.</summary>
        public decimal Margin => Revenue > 0 ? GrossProfit / Revenue : 0;
    }

    /// <summary>
    /// Top-level model for the Region Performance Overview report.
    /// Aggregates all sales by region — no date or filter parameters are required.
    /// Grand totals are derived from <see cref="Rows"/>.
    /// </summary>
    public class RegionSummaryReportModel : IReportModel
    {
        /// <summary>One row per region, ordered alphabetically.</summary>
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
