using System;

namespace Reporting.Core.Models
{
    /// <summary>
    /// Represents a single sales transaction as returned from the data source.
    /// <see cref="Revenue"/> and <see cref="GrossProfit"/> are display expressions
    /// derived from the raw fields and do not require separate storage.
    /// </summary>
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

        /// <summary>Display expression: <see cref="UnitsSold"/> × <see cref="UnitPrice"/>.</summary>
        public decimal Revenue => UnitsSold * UnitPrice;

        /// <summary>Display expression: <see cref="Revenue"/> − <see cref="CostOfGoods"/>.</summary>
        public decimal GrossProfit => Revenue - CostOfGoods;
    }
}
