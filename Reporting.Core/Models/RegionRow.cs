namespace Reporting.Core.Models
{
    /// <summary>
    /// Aggregated sales performance for a single region.
    /// <see cref="Margin"/> is a display expression derived from the aggregated totals.
    /// </summary>
    public class RegionRow
    {
        /// <summary>Region name (e.g. "North").</summary>
        public string Region { get; set; }

        /// <summary>Number of individual sales lines in this region.</summary>
        public int LineCount { get; set; }

        /// <summary>Total revenue across all lines in this region.</summary>
        public decimal Revenue { get; set; }

        /// <summary>Total gross profit across all lines in this region.</summary>
        public decimal GrossProfit { get; set; }

        /// <summary>Display expression: <see cref="GrossProfit"/> ÷ <see cref="Revenue"/>.</summary>
        public decimal Margin => Revenue > 0 ? GrossProfit / Revenue : 0;
    }
}
