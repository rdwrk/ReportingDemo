using Reporting.Core.Models;
using Xunit;

namespace Reporting.Tests.Core
{
    /// <summary>
    /// Verifies default values and property assignment for <see cref="CustomerInvoiceGroup"/>.
    /// </summary>
    public class CustomerInvoiceGroupTests
    {
        [Fact]
        public void Lines_DefaultsToEmptyList()
        {
            var group = new CustomerInvoiceGroup();
            Assert.NotNull(group.Lines);
            Assert.Empty(group.Lines);
        }

        [Fact]
        public void Lines_CanBePopulated()
        {
            var group = new CustomerInvoiceGroup();
            group.Lines.Add(new InvoiceLine { InvoiceNumber = "INV-001" });
            Assert.Single(group.Lines);
            Assert.Equal("INV-001", group.Lines[0].InvoiceNumber);
        }

        [Fact]
        public void AllPropertiesCanBeSet()
        {
            var group = new CustomerInvoiceGroup
            {
                CustomerName  = "Acme Ltd",
                CustomerRef   = "CUST-001",
                SubtotalNet   = 1000m,
                SubtotalVat   = 200m,
                SubtotalGross = 1200m,
            };

            Assert.Equal("Acme Ltd",  group.CustomerName);
            Assert.Equal("CUST-001",  group.CustomerRef);
            Assert.Equal(1000m,       group.SubtotalNet);
            Assert.Equal(200m,        group.SubtotalVat);
            Assert.Equal(1200m,       group.SubtotalGross);
        }
    }
}
