using System.Collections.Generic;
using Reporting.Core.Models;
using Reporting.Core.Templates;
using Xunit;

namespace Reporting.Tests.Core
{
    /// <summary>
    /// Verifies <see cref="RegionSummaryReportModel"/> grand-total expressions and null-safety guards.
    /// </summary>
    public class RegionSummaryReportModelTests
    {
        [Fact]
        public void Rows_DefaultsToEmptyList()
        {
            var model = new RegionSummaryReportModel();
            Assert.NotNull(model.Rows);
            Assert.Empty(model.Rows);
        }

        [Fact]
        public void GrandLineCount_SumsRowLineCounts()
        {
            var model = new RegionSummaryReportModel
            {
                Rows = new List<RegionRow>
                {
                    new RegionRow { LineCount = 10 },
                    new RegionRow { LineCount = 25 },
                }
            };
            Assert.Equal(35, model.GrandLineCount);
        }

        [Fact]
        public void GrandLineCount_IsZeroWhenRowsIsNull()
        {
            var model = new RegionSummaryReportModel { Rows = null };
            Assert.Equal(0, model.GrandLineCount);
        }

        [Fact]
        public void GrandRevenue_SumsRowRevenues()
        {
            var model = new RegionSummaryReportModel
            {
                Rows = new List<RegionRow>
                {
                    new RegionRow { Revenue = 500m  },
                    new RegionRow { Revenue = 1500m },
                }
            };
            Assert.Equal(2000m, model.GrandRevenue);
        }

        [Fact]
        public void GrandRevenue_IsZeroWhenRowsIsNull()
        {
            var model = new RegionSummaryReportModel { Rows = null };
            Assert.Equal(0m, model.GrandRevenue);
        }

        [Fact]
        public void GrandGrossProfit_SumsRowGrossProfits()
        {
            var model = new RegionSummaryReportModel
            {
                Rows = new List<RegionRow>
                {
                    new RegionRow { GrossProfit = 200m },
                    new RegionRow { GrossProfit = 300m },
                }
            };
            Assert.Equal(500m, model.GrandGrossProfit);
        }

        [Fact]
        public void GrandGrossProfit_IsZeroWhenRowsIsNull()
        {
            var model = new RegionSummaryReportModel { Rows = null };
            Assert.Equal(0m, model.GrandGrossProfit);
        }

        [Fact]
        public void GrandMargin_IsGrossProfitOverRevenue()
        {
            var model = new RegionSummaryReportModel
            {
                Rows = new List<RegionRow>
                {
                    new RegionRow { Revenue = 1000m, GrossProfit = 400m },
                }
            };
            Assert.Equal(0.4m, model.GrandMargin);
        }

        [Fact]
        public void GrandMargin_IsZeroWhenGrandRevenueIsZero()
        {
            Assert.Equal(0m, new RegionSummaryReportModel().GrandMargin);
        }
    }
}
