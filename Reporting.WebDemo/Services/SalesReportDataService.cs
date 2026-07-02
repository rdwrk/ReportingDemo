using System;
using System.Collections.Generic;
using Reporting.Core.Models;

namespace Reporting.WebDemo.Services
{
    /// <summary>
    /// DAL service: returns raw sales line items for a given date range and optional region filter.
    /// The caller is responsible for assembling the populated <see cref="Reporting.Core.Templates.SalesReportModel"/>
    /// from the returned lines and any additional metadata (prepared-by, date range, etc.).
    /// In production replace this with a database query mapped directly to <see cref="SalesLineItem"/>.
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

        /// <summary>
        /// Returns raw sales line items for the given period and optional region filter.
        /// Pass the result to the caller to build a <see cref="Reporting.Core.Templates.SalesReportModel"/>.
        /// </summary>
        /// <param name="dateFrom">Start of the reporting period; defaults to three months ago if null.</param>
        /// <param name="dateTo">End of the reporting period; defaults to today if null.</param>
        /// <param name="region">Optional region filter; null or empty returns all regions.</param>
        public List<SalesLineItem> GetLines(DateTime? dateFrom, DateTime? dateTo, string? region = null)
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

            return items;
        }
    }
}
