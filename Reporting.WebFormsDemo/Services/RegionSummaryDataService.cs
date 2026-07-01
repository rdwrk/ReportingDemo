using System;
using System.Collections.Generic;
using Reporting.Core.Interfaces;
using Reporting.Core.Models;
using Reporting.Core.Templates;

namespace Reporting.WebFormsDemo.Services
{
    /// <summary>
    /// Generates the Region Performance Overview by aggregating the same seeded
    /// sales data used by <see cref="SalesReportDataService"/> into per-region buckets.
    /// No request parameters are used — the report always covers the full dataset.
    /// </summary>
    public class RegionSummaryDataService : IReportDataService<RegionSummaryReportModel>
    {
        private static readonly string[] Regions =
        {
            "East", "Midlands", "North", "South", "West",
        };

        /// <inheritdoc />
        public RegionSummaryReportModel GetModel(ReportRequest request)
        {
            var rng = new Random(42);

            var revenue     = new decimal[Regions.Length];
            var grossProfit = new decimal[Regions.Length];
            var lineCount   = new int[Regions.Length];

            for (int i = 0; i < 120; i++)
            {
                int     ri    = rng.Next(Regions.Length);
                decimal price = Math.Round((decimal)(rng.NextDouble() * 1800 + 200), 2);
                int     units = rng.Next(1, 25);
                decimal cogs  = Math.Round(price * (decimal)(rng.NextDouble() * 0.4 + 0.3), 2) * units;
                decimal rev   = price * units;

                revenue[ri]     += rev;
                grossProfit[ri] += rev - cogs;
                lineCount[ri]   += 1;
            }

            var rows = new List<RegionRow>();
            for (int i = 0; i < Regions.Length; i++)
            {
                rows.Add(new RegionRow
                {
                    Region      = Regions[i],
                    LineCount   = lineCount[i],
                    Revenue     = revenue[i],
                    GrossProfit = grossProfit[i],
                });
            }

            return new RegionSummaryReportModel { Rows = rows };
        }
    }
}
