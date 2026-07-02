using Reporting.Core.Models;
using Xunit;

namespace Reporting.Tests.Core
{
    /// <summary>
    /// Verifies <see cref="RegionRow"/> margin expression and property assignment.
    /// </summary>
    public class RegionRowTests
    {
        [Fact]
        public void Margin_IsGrossProfitDividedByRevenue()
        {
            var row = new RegionRow { Revenue = 1000m, GrossProfit = 250m };
            Assert.Equal(0.25m, row.Margin);
        }

        [Fact]
        public void Margin_IsZeroWhenRevenueIsZero()
        {
            var row = new RegionRow { Revenue = 0m, GrossProfit = 100m };
            Assert.Equal(0m, row.Margin);
        }

        [Fact]
        public void Margin_IsZeroWhenBothValuesAreZero()
        {
            var row = new RegionRow { Revenue = 0m, GrossProfit = 0m };
            Assert.Equal(0m, row.Margin);
        }

        [Fact]
        public void AllPropertiesCanBeSet()
        {
            var row = new RegionRow
            {
                Region      = "South",
                LineCount   = 42,
                Revenue     = 5000m,
                GrossProfit = 1500m,
            };

            Assert.Equal("South", row.Region);
            Assert.Equal(42,      row.LineCount);
            Assert.Equal(5000m,   row.Revenue);
            Assert.Equal(1500m,   row.GrossProfit);
        }
    }
}
