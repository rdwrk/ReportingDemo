using System;
using System.Collections.Generic;
using Reporting.Core.Models;

namespace Reporting.WebFormsDemo.Services
{
    /// <summary>
    /// DAL service: returns per-region aggregated rows derived from seeded sales data.
    /// The caller is responsible for assembling the populated <see cref="Reporting.Core.Templates.RegionSummaryReportModel"/>
    /// from the returned rows.
    /// In production replace this with a database query mapped directly to <see cref="RegionRow"/>.
    /// </summary>
    public class RegionSummaryDataService
    {
        private static readonly string[] Regions =
        {
            "East", "Midlands", "North", "South", "West",
        };

        /// <summary>
        /// Returns one aggregated row per region.
        /// Pass the result to the caller to build a <see cref="Reporting.Core.Templates.RegionSummaryReportModel"/>.
        /// </summary>
        public List<RegionRow> GetRows()
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

            return rows;
        }
    }
}
