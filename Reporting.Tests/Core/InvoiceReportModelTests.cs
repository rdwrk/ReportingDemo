using System;
using Reporting.Core.Models;
using Reporting.Core.Templates;
using Xunit;

namespace Reporting.Tests.Core
{
    /// <summary>
    /// Verifies default values and property assignment for <see cref="InvoiceReportModel"/>.
    /// </summary>
    public class InvoiceReportModelTests
    {
        [Fact]
        public void CustomerGroups_DefaultsToEmptyList()
        {
            var model = new InvoiceReportModel();
            Assert.NotNull(model.CustomerGroups);
            Assert.Empty(model.CustomerGroups);
        }

        [Fact]
        public void Metadata_DefaultsToNonNullInstance()
        {
            var model = new InvoiceReportModel();
            Assert.NotNull(model.Metadata);
        }

        [Fact]
        public void DataPropertiesCanBeSet()
        {
            var model = new InvoiceReportModel
            {
                TotalInvoiceCount = 25,
                GrandTotalNet     = 10_000m,
                GrandTotalVat     = 2_000m,
                GrandTotalGross   = 12_000m,
                TotalOutstanding  = 3_000m,
            };

            Assert.Equal(25,       model.TotalInvoiceCount);
            Assert.Equal(10_000m,  model.GrandTotalNet);
            Assert.Equal(2_000m,   model.GrandTotalVat);
            Assert.Equal(12_000m,  model.GrandTotalGross);
            Assert.Equal(3_000m,   model.TotalOutstanding);
        }

        [Fact]
        public void MetadataContextFields_CanBeSet()
        {
            var from  = new DateTime(2024, 1, 1);
            var to    = new DateTime(2024, 6, 30);
            var model = new InvoiceReportModel
            {
                Metadata = new ReportMetadata
                {
                    PeriodFrom = from,
                    PeriodTo   = to,
                    PreparedBy = "Bob",
                }
            };

            Assert.Equal(from,  model.Metadata.PeriodFrom);
            Assert.Equal(to,    model.Metadata.PeriodTo);
            Assert.Equal("Bob", model.Metadata.PreparedBy);
        }
    }
}
