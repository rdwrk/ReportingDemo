using System;
using System.Collections.Generic;
using Reporting.Core.Interfaces;
using Reporting.Core.Models;

namespace Reporting.WebDemo.Services
{
    /// <summary>
    /// Generates seeded random invoice data for the Invoice Summary report.
    /// Produces 5 customer groups with 8–20 invoices each.
    /// </summary>
    public class InvoiceReportDataService : IReportDataService<InvoiceReportModel>
    {
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

        /// <inheritdoc />
        public InvoiceReportModel GetModel(ReportRequest request)
        {
            var rng      = new Random(99);
            var dateFrom = request.DateFrom ?? DateTime.Today.AddMonths(-3);
            var dateTo   = request.DateTo   ?? DateTime.Today;

            request.Parameters.TryGetValue("preparedBy", out var preparedBy);

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
                        InvoiceDate   = dateFrom.AddDays(rng.NextDouble() * 60),
                        DueDate       = dateFrom.AddDays(rng.NextDouble() * 60 + 30),
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

                grandNet   += subNet;
                grandVat   += subVat;
                grandGross += subGross;
                totalInvoices += count;
            }

            return new InvoiceReportModel
            {
                DateFrom          = dateFrom,
                DateTo            = dateTo,
                PreparedBy        = preparedBy ?? "System",
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
