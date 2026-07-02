using System.Collections.Generic;
using System.Linq;
using Reporting.Core.Interfaces;
using Reporting.Core.Models;

namespace Reporting.Core.Templates
{
    /// <summary>
    /// Report model for the Region Performance Overview.
    /// Contains only the raw data entities (<see cref="Rows"/>) and derived display totals.
    /// No filter parameters are required — the report always covers the full dataset.
    /// <see cref="Metadata"/> is available for setting AppName, AppVersion, or other footer overrides
    /// but requires no context fields (no date range or prepared-by for this report type).
    /// </summary>
    public class RegionSummaryReportModel : IReportModel
    {
        /// <summary>
        /// Report-level context available to the builder. No context fields are required
        /// for this report type; may be used to override <see cref="ReportMetadata.AppName"/> or
        /// <see cref="ReportMetadata.AppVersion"/> if needed.
        /// </summary>
        public ReportMetadata Metadata { get; set; } = new ReportMetadata();

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
