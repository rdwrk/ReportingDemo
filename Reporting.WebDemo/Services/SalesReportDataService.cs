using System;
using System.Collections.Generic;
using Reporting.Core.Interfaces;
using Reporting.Core.Models;

namespace Reporting.WebDemo.Services
{
    /// <summary>
    /// Generates 120 seeded random sales lines for the Sales Summary report.
    /// The seed is fixed so generated data is consistent across requests.
    /// </summary>
    public class SalesReportDataService : IReportDataService<SalesReportModel>
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

        /// <inheritdoc />
        public SalesReportModel GetModel(ReportRequest request)
        {
            var rng      = new Random(42);
            var dateFrom = request.DateFrom ?? DateTime.Today.AddMonths(-3);
            var dateTo   = request.DateTo   ?? DateTime.Today;
            var span     = (dateTo - dateFrom).TotalDays;

            request.Parameters.TryGetValue("region",     out var region);
            request.Parameters.TryGetValue("preparedBy", out var preparedBy);

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
                    SaleDate    = dateFrom.AddDays(rng.NextDouble() * span),
                });
            }

            return new SalesReportModel
            {
                DateFrom     = dateFrom,
                DateTo       = dateTo,
                RegionFilter = region,
                PreparedBy   = preparedBy ?? "System",
                Items        = items,
            };
        }
    }
}
