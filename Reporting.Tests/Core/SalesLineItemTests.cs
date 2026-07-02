using System;
using Reporting.Core.Models;
using Xunit;

namespace Reporting.Tests.Core
{
    /// <summary>
    /// Verifies <see cref="SalesLineItem"/> computed expressions and property assignment.
    /// </summary>
    public class SalesLineItemTests
    {
        [Fact]
        public void Revenue_IsUnitsSoldTimesUnitPrice()
        {
            var item = new SalesLineItem { UnitsSold = 10, UnitPrice = 5.50m };
            Assert.Equal(55.00m, item.Revenue);
        }

        [Fact]
        public void Revenue_IsZeroWhenUnitsSoldIsZero()
        {
            var item = new SalesLineItem { UnitsSold = 0, UnitPrice = 9.99m };
            Assert.Equal(0m, item.Revenue);
        }

        [Fact]
        public void GrossProfit_IsRevenueMinusCostOfGoods()
        {
            var item = new SalesLineItem { UnitsSold = 10, UnitPrice = 5.50m, CostOfGoods = 30m };
            Assert.Equal(25m, item.GrossProfit);
        }

        [Fact]
        public void GrossProfit_CanBeNegativeWhenCostExceedsRevenue()
        {
            var item = new SalesLineItem { UnitsSold = 2, UnitPrice = 5m, CostOfGoods = 20m };
            Assert.Equal(-10m, item.GrossProfit);
        }

        [Fact]
        public void AllPropertiesCanBeSet()
        {
            var date = new DateTime(2024, 6, 1);
            var item = new SalesLineItem
            {
                Region      = "North",
                Product     = "Widget",
                SalesRep    = "Jane Smith",
                UnitsSold   = 5,
                UnitPrice   = 12m,
                CostOfGoods = 40m,
                SaleDate    = date,
            };

            Assert.Equal("North",      item.Region);
            Assert.Equal("Widget",     item.Product);
            Assert.Equal("Jane Smith", item.SalesRep);
            Assert.Equal(5,            item.UnitsSold);
            Assert.Equal(12m,          item.UnitPrice);
            Assert.Equal(40m,          item.CostOfGoods);
            Assert.Equal(date,         item.SaleDate);
        }
    }
}
