using System;
using System.Collections.Generic;
using Reporting.Core.Models;

namespace Reporting.WebDemo.Services
{
    /// <summary>
    /// DAL service: returns pre-aggregated customer invoice groups with per-customer subtotals
    /// and report-level grand totals for the given date range.
    /// The caller is responsible for assembling the populated <see cref="Reporting.Core.Templates.InvoiceReportModel"/>
    /// from the returned <see cref="InvoiceData"/> and any additional metadata (prepared-by, date range, etc.).
    /// In production replace this with a database query and GROUP BY aggregation.
    /// </summary>
    public class InvoiceReportDataService
    {
        /// <summary>Carries the raw aggregated data returned by <see cref="GetCustomerGroups"/>.</summary>
        public sealed class InvoiceData
        {
            /// <summary>Invoice lines grouped by customer, each with pre-calculated subtotals.</summary>
            public List<CustomerInvoiceGroup> CustomerGroups    { get; set; } = new List<CustomerInvoiceGroup>();

            /// <summary>Total number of individual invoice lines across all customers.</summary>
            public int     TotalInvoiceCount { get; set; }

            /// <summary>Sum of all net amounts across all customers.</summary>
            public decimal GrandTotalNet     { get; set; }

            /// <summary>Sum of all VAT amounts across all customers.</summary>
            public decimal GrandTotalVat     { get; set; }

            /// <summary>Sum of all gross amounts across all customers.</summary>
            public decimal GrandTotalGross   { get; set; }

            /// <summary>Sum of gross amounts for lines with status "Due" or "Overdue".</summary>
            public decimal TotalOutstanding  { get; set; }
        }

        private static readonly string[] Customers =
        {
            "Acme Corporation", "Globex Industries", "Initech Solutions",
            "Umbrella Systems", "Soylent Technologies",
        };

        private static readonly string[] CustomerRefs =
        {
            "CUST-001", "CUST-002", "CUST-003", "CUST-004", "CUST-005",
        };

        private static readonly string[] Statuses =
        {
            "Paid", "Paid", "Paid", "Due", "Overdue",
        };

        private static readonly string[] Descriptions =
        {
            "Annual software licence", "Professional services — Q1", "Cloud hosting — monthly",
            "Support contract renewal", "Implementation services", "Training package",
            "Data migration services", "API access — tier 2",
        };

        /// <summary>
        /// Returns pre-aggregated customer invoice data for the given period.
        /// Pass the result to the caller to build a <see cref="Reporting.Core.Templates.InvoiceReportModel"/>.
        /// </summary>
        /// <param name="dateFrom">Start of the reporting period; defaults to three months ago if null.</param>
        /// <param name="dateTo">End of the reporting period; defaults to today if null.</param>
        public InvoiceData GetCustomerGroups(DateTime? dateFrom, DateTime? dateTo)
        {
            var rng  = new Random(99);
            var from = dateFrom ?? DateTime.Today.AddMonths(-3);
            var to   = dateTo   ?? DateTime.Today;

            var     groups        = new List<CustomerInvoiceGroup>();
            decimal grandNet      = 0, grandVat = 0, grandGross = 0, outstanding = 0;
            int     totalInvoices = 0;

            for (int c = 0; c < Customers.Length; c++)
            {
                var     lines  = new List<InvoiceLine>();
                decimal subNet = 0, subVat = 0, subGross = 0;
                int     count  = rng.Next(8, 20);

                for (int i = 0; i < count; i++)
                {
                    decimal net    = Math.Round((decimal)(rng.NextDouble() * 4000 + 500), 2);
                    decimal vat    = Math.Round(net * 0.20m, 2);
                    string  status = Statuses[rng.Next(Statuses.Length)];

                    var line = new InvoiceLine
                    {
                        InvoiceNumber = $"INV-{(c * 100 + i + 1):D5}",
                        InvoiceDate   = from.AddDays(rng.NextDouble() * 60),
                        DueDate       = from.AddDays(rng.NextDouble() * 60 + 30),
                        Description   = Descriptions[rng.Next(Descriptions.Length)],
                        NetAmount     = net,
                        VatAmount     = vat,
                        Status        = status,
                    };

                    lines.Add(line);
                    subNet   += net;
                    subVat   += vat;
                    subGross += line.GrossAmount;

                    if (status == "Due" || status == "Overdue")
                        outstanding += line.GrossAmount;
                }

                groups.Add(new CustomerInvoiceGroup
                {
                    CustomerName  = Customers[c],
                    CustomerRef   = CustomerRefs[c],
                    Lines         = lines,
                    SubtotalNet   = subNet,
                    SubtotalVat   = subVat,
                    SubtotalGross = subGross,
                });

                grandNet      += subNet;
                grandVat      += subVat;
                grandGross    += subGross;
                totalInvoices += count;
            }

            return new InvoiceData
            {
                CustomerGroups    = groups,
                TotalInvoiceCount = totalInvoices,
                GrandTotalNet     = grandNet,
                GrandTotalVat     = grandVat,
                GrandTotalGross   = grandGross,
                TotalOutstanding  = outstanding,
            };
        }
    }
}
