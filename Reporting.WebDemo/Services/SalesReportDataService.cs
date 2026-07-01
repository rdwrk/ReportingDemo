using System;
using System.Collections.Generic;
using Reporting.Core.Models;
using Reporting.Core.Templates;

namespace Reporting.WebDemo.Services
{
    /// <summary>
    /// Demo data service: generates 120 seeded random sales lines.
    /// In production replace this with a database query and map the results
    /// directly into <see cref="SalesReportModel"/>.
    /// </summary>
    public class SalesReportDataService
    {
        private static readonly string[] Products =
        {
            "Enterprise CRM License", "Analytics Suite Pro", "Cloud Backup 500GB",
            "Security Audit Package", "API Integration Module", "Data Warehouse Connector",
            "Mobile App Builder", "Reporting Engine", "SSO Gateway", "DevOps Toolkit",
        };

        private static readonly string[] Reps =
        {
            "Alice Thompson", "Bob Patel", "Carol Edwards", "David Kim", "Emma Walsh",
        };

        private static readonly string[] Regions =
        {
            "North", "South", "East", "West", "Midlands",
        };

        /// <summary>Returns a populated <see cref="SalesReportModel"/> for the given parameters.</summary>
        public SalesReportModel GetModel(DateTime? dateFrom, DateTime? dateTo, string region = null, string preparedBy = null)
        {
            var rng  = new Random(42);
            var from = dateFrom ?? DateTime.Today.AddMonths(-3);
            var to   = dateTo   ?? DateTime.Today;
            var span = (to - from).TotalDays;

            var items = new List<SalesLineItem>();

            for (int i = 0; i < 120; i++)
            {
                var regionValue = Regions[rng.Next(Regions.Length)];
                if (!string.IsNullOrEmpty(region) &&
                    !regionValue.Equals(region, StringComparison.OrdinalIgnoreCase))
                    continue;

                decimal unitPrice = Math.Round((decimal)(rng.NextDouble() * 1800 + 200), 2);
                int     units     = rng.Next(1, 25);

                items.Add(new SalesLineItem
                {
                    Region      = regionValue,
                    Product     = Products[rng.Next(Products.Length)],
                    SalesRep    = Reps[rng.Next(Reps.Length)],
                    UnitsSold   = units,
                    UnitPrice   = unitPrice,
                    CostOfGoods = Math.Round(unitPrice * (decimal)(rng.NextDouble() * 0.4 + 0.3), 2) * units,
                    SaleDate    = from.AddDays(rng.NextDouble() * span),
                });
            }

            return new SalesReportModel
            {
                DateFrom     = from,
                DateTo       = to,
                RegionFilter = region,
                PreparedBy   = preparedBy ?? "System",
                Items        = items,
            };
        }
    }
}
