using System;
using System.Collections.Generic;
using System.Linq;
using Reporting.Core.Interfaces;

namespace Reporting.Core.Models
{
    /// <summary>Represents a single line on the Sales Summary report.</summary>
    public class SalesLineItem
    {
        /// <summary>Sales region (e.g. "North", "Midlands").</summary>
        public string Region { get; set; }

        /// <summary>Product or service sold.</summary>
        public string Product { get; set; }

        /// <summary>Name of the sales representative.</summary>
        public string SalesRep { get; set; }

        /// <summary>Number of units sold.</summary>
        public int UnitsSold { get; set; }

        /// <summary>Selling price per unit.</summary>
        public decimal UnitPrice { get; set; }

        /// <summary>Total cost of goods for this line.</summary>
        public decimal CostOfGoods { get; set; }

        /// <summary>Date the sale was recorded.</summary>
        public DateTime SaleDate { get; set; }

        /// <summary>Total revenue: <see cref="UnitsSold"/> × <see cref="UnitPrice"/>.</summary>
        public decimal Revenue => UnitsSold * UnitPrice;

        /// <summary>Gross profit: <see cref="Revenue"/> − <see cref="CostOfGoods"/>.</summary>
        public decimal GrossProfit => Revenue - CostOfGoods;
    }

    /// <summary>
    /// Top-level model for the Sales Summary report.
    /// Aggregated totals are derived from <see cref="Items"/>.
    /// </summary>
    public class SalesReportModel : IReportModel
    {
        /// <summary>Start of the reporting period.</summary>
        public DateTime? DateFrom { get; set; }

        /// <summary>End of the reporting period.</summary>
        public DateTime? DateTo { get; set; }

        /// <summary>Region filter applied to this model. Null or empty means all regions.</summary>
        public string RegionFilter { get; set; }

        /// <summary>Person or team that requested the report, shown in the header.</summary>
        public string PreparedBy { get; set; }

        /// <summary>All sales lines included in this report.</summary>
        public List<SalesLineItem> Items { get; set; } = new List<SalesLineItem>();

        /// <summary>Sum of <see cref="SalesLineItem.Revenue"/> across all items.</summary>
        public decimal TotalRevenue => Items?.Sum(i => i.Revenue) ?? 0;

        /// <summary>Sum of <see cref="SalesLineItem.GrossProfit"/> across all items.</summary>
        public decimal TotalGrossProfit => Items?.Sum(i => i.GrossProfit) ?? 0;
    }
}
