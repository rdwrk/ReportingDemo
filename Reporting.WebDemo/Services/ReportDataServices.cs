using Reporting.Core.Interfaces;
using Reporting.Core.Models;

namespace Reporting.WebDemo.Services
{
    public class SalesReportDataService : IReportDataService<SalesReportModel>
    {
        private static readonly string[] Products = {
            "Enterprise CRM License", "Analytics Suite Pro", "Cloud Backup 500GB",
            "Security Audit Package", "API Integration Module", "Data Warehouse Connector",
            "Mobile App Builder", "Reporting Engine", "SSO Gateway", "DevOps Toolkit"
        };
        private static readonly string[] Reps    = { "Alice Thompson", "Bob Patel", "Carol Edwards", "David Kim", "Emma Walsh" };
        private static readonly string[] Regions = { "North", "South", "East", "West", "Midlands" };

        public SalesReportModel GetModel(ReportRequest request)
        {
            var rng      = new Random(42);
            var dateFrom = request.DateFrom ?? DateTime.Today.AddMonths(-3);
            var dateTo   = request.DateTo   ?? DateTime.Today;
            request.Parameters.TryGetValue("region", out var region);
            request.Parameters.TryGetValue("preparedBy", out var preparedBy);

            var items = new List<SalesLineItem>();
            var span  = (dateTo - dateFrom).TotalDays;

            for (int i = 0; i < 120; i++)
            {
                var r = Regions[rng.Next(Regions.Length)];
                if (!string.IsNullOrEmpty(region) && !r.Equals(region, StringComparison.OrdinalIgnoreCase))
                    continue;

                decimal unitPrice = Math.Round((decimal)(rng.NextDouble() * 1800 + 200), 2);
                int units = rng.Next(1, 25);

                items.Add(new SalesLineItem
                {
                    Region      = r,
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

    public class InvoiceReportDataService : IReportDataService<InvoiceReportModel>
    {
        private static readonly string[] Customers    = { "Acme Corporation", "Globex Industries", "Initech Solutions", "Umbrella Systems", "Soylent Technologies" };
        private static readonly string[] CustomerRefs = { "CUST-001", "CUST-002", "CUST-003", "CUST-004", "CUST-005" };
        private static readonly string[] Statuses     = { "Paid", "Paid", "Paid", "Due", "Overdue" };
        private static readonly string[] Descs        = {
            "Annual software licence", "Professional services — Q1", "Cloud hosting — monthly",
            "Support contract renewal", "Implementation services", "Training package",
            "Data migration services", "API access — tier 2"
        };

        public InvoiceReportModel GetModel(ReportRequest request)
        {
            var rng      = new Random(99);
            var dateFrom = request.DateFrom ?? DateTime.Today.AddMonths(-3);
            var dateTo   = request.DateTo   ?? DateTime.Today;
            request.Parameters.TryGetValue("preparedBy", out var preparedBy);

            var groups = new List<CustomerInvoiceGroup>();
            decimal grandNet = 0, grandVat = 0, grandGross = 0, outstanding = 0;
            int totalInvoices = 0;

            for (int c = 0; c < Customers.Length; c++)
            {
                var lines = new List<InvoiceLine>();
                decimal subNet = 0, subVat = 0, subGross = 0;
                int count = rng.Next(8, 20);

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
                        Description   = Descs[rng.Next(Descs.Length)],
                        NetAmount     = net,
                        VatAmount     = vat,
                        Status        = status,
                    };
                    lines.Add(line);
                    subNet += net; subVat += vat; subGross += line.GrossAmount;
                    if (status == "Due" || status == "Overdue") outstanding += line.GrossAmount;
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
                grandNet += subNet; grandVat += subVat; grandGross += subGross;
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

    /// <summary>Generates the Region Performance Overview by aggregating the same seeded sales data.</summary>
    public class RegionSummaryDataService : IReportDataService<RegionSummaryReportModel>
    {
        private static readonly string[] Products = {
            "Enterprise CRM License", "Analytics Suite Pro", "Cloud Backup 500GB",
            "Security Audit Package", "API Integration Module", "Data Warehouse Connector",
            "Mobile App Builder", "Reporting Engine", "SSO Gateway", "DevOps Toolkit"
        };
        private static readonly string[] Reps    = { "Alice Thompson", "Bob Patel", "Carol Edwards", "David Kim", "Emma Walsh" };
        private static readonly string[] Regions = { "East", "Midlands", "North", "South", "West" };

        public RegionSummaryReportModel GetModel(ReportRequest request)
        {
            var rng  = new Random(42);
            var from = DateTime.Today.AddMonths(-3);
            var to   = DateTime.Today;
            var span = (to - from).TotalDays;

            var buckets = new Dictionary<string, (int lines, decimal rev, decimal gp)>();
            foreach (var r in Regions) buckets[r] = (0, 0, 0);

            for (int i = 0; i < 120; i++)
            {
                var region    = Regions[rng.Next(Regions.Length)];
                decimal price = Math.Round((decimal)(rng.NextDouble() * 1800 + 200), 2);
                int units     = rng.Next(1, 25);
                decimal cogs  = Math.Round(price * (decimal)(rng.NextDouble() * 0.4 + 0.3), 2) * units;
                decimal rev   = price * units;
                decimal gp    = rev - cogs;
                var (l, r2, g) = buckets[region];
                buckets[region] = (l + 1, r2 + rev, g + gp);
            }

            var rows = new List<RegionRow>();
            foreach (var kvp in buckets)
                rows.Add(new RegionRow { Region = kvp.Key, LineCount = kvp.Value.lines, Revenue = kvp.Value.rev, GrossProfit = kvp.Value.gp });

            return new RegionSummaryReportModel { Rows = rows };
        }
    }
}
