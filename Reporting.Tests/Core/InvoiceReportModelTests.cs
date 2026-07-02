using System;
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
        public void AllPropertiesCanBeSet()
        {
            var from  = new DateTime(2024, 1, 1);
            var to    = new DateTime(2024, 6, 30);
            var model = new InvoiceReportModel
            {
                DateFrom          = from,
                DateTo            = to,
                PreparedBy        = "Bob",
                TotalInvoiceCount = 25,
                GrandTotalNet     = 10_000m,
                GrandTotalVat     = 2_000m,
                GrandTotalGross   = 12_000m,
                TotalOutstanding  = 3_000m,
            };

            Assert.Equal(from,     model.DateFrom);
            Assert.Equal(to,       model.DateTo);
            Assert.Equal("Bob",    model.PreparedBy);
            Assert.Equal(25,       model.TotalInvoiceCount);
            Assert.Equal(10_000m,  model.GrandTotalNet);
            Assert.Equal(2_000m,   model.GrandTotalVat);
            Assert.Equal(12_000m,  model.GrandTotalGross);
            Assert.Equal(3_000m,   model.TotalOutstanding);
        }
    }
}
