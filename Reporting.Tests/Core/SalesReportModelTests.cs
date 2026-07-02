using System;
using System.Collections.Generic;
using Reporting.Core.Models;
using Reporting.Core.Templates;
using Xunit;

namespace Reporting.Tests.Core
{
    /// <summary>
    /// Verifies <see cref="SalesReportModel"/> default values, computed totals, and null-safety guards.
    /// </summary>
    public class SalesReportModelTests
    {
        [Fact]
        public void Items_DefaultsToEmptyList()
        {
            var model = new SalesReportModel();
            Assert.NotNull(model.Items);
            Assert.Empty(model.Items);
        }

        [Fact]
        public void Metadata_DefaultsToNonNullInstance()
        {
            var model = new SalesReportModel();
            Assert.NotNull(model.Metadata);
        }

        [Fact]
        public void TotalRevenue_SumsItemRevenues()
        {
            var model = new SalesReportModel
            {
                Items = new List<SalesLineItem>
                {
                    new SalesLineItem { UnitsSold = 2, UnitPrice = 10m },
                    new SalesLineItem { UnitsSold = 3, UnitPrice = 20m },
                }
            };
            Assert.Equal(80m, model.TotalRevenue);
        }

        [Fact]
        public void TotalRevenue_IsZeroForEmptyList()
        {
            Assert.Equal(0m, new SalesReportModel().TotalRevenue);
        }

        [Fact]
        public void TotalRevenue_IsZeroWhenItemsIsNull()
        {
            var model = new SalesReportModel { Items = null };
            Assert.Equal(0m, model.TotalRevenue);
        }

        [Fact]
        public void TotalGrossProfit_SumsItemGrossProfits()
        {
            var model = new SalesReportModel
            {
                Items = new List<SalesLineItem>
                {
                    new SalesLineItem { UnitsSold = 2, UnitPrice = 10m, CostOfGoods = 5m  },
                    new SalesLineItem { UnitsSold = 3, UnitPrice = 20m, CostOfGoods = 10m },
                }
            };
            // Revenue = 20 + 60 = 80; CoGS = 5 + 10 = 15; GP = 15 + 50 = 65
            Assert.Equal(65m, model.TotalGrossProfit);
        }

        [Fact]
        public void TotalGrossProfit_IsZeroWhenItemsIsNull()
        {
            var model = new SalesReportModel { Items = null };
            Assert.Equal(0m, model.TotalGrossProfit);
        }

        [Fact]
        public void MetadataContextFields_CanBeSet()
        {
            var from  = new DateTime(2024, 1, 1);
            var to    = new DateTime(2024, 3, 31);
            var model = new SalesReportModel
            {
                Metadata = new ReportMetadata
                {
                    PeriodFrom = from,
                    PeriodTo   = to,
                    Filter     = "North",
                    PreparedBy = "Alice",
                }
            };

            Assert.Equal(from,    model.Metadata.PeriodFrom);
            Assert.Equal(to,      model.Metadata.PeriodTo);
            Assert.Equal("North", model.Metadata.Filter);
            Assert.Equal("Alice", model.Metadata.PreparedBy);
        }
    }
}
