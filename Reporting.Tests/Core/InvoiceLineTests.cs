using System;
using Reporting.Core.Models;
using Xunit;

namespace Reporting.Tests.Core
{
    /// <summary>
    /// Verifies <see cref="InvoiceLine"/> computed expression and property assignment.
    /// </summary>
    public class InvoiceLineTests
    {
        [Fact]
        public void GrossAmount_IsNetPlusVat()
        {
            var line = new InvoiceLine { NetAmount = 100m, VatAmount = 20m };
            Assert.Equal(120m, line.GrossAmount);
        }

        [Fact]
        public void GrossAmount_IsZeroWhenBothAmountsAreZero()
        {
            var line = new InvoiceLine { NetAmount = 0m, VatAmount = 0m };
            Assert.Equal(0m, line.GrossAmount);
        }

        [Fact]
        public void AllPropertiesCanBeSet()
        {
            var invDate = new DateTime(2024, 1, 1);
            var dueDate = new DateTime(2024, 1, 31);
            var line    = new InvoiceLine
            {
                InvoiceNumber = "INV-001",
                InvoiceDate   = invDate,
                DueDate       = dueDate,
                Description   = "Consulting services",
                NetAmount     = 500m,
                VatAmount     = 100m,
                Status        = "Paid",
            };

            Assert.Equal("INV-001",              line.InvoiceNumber);
            Assert.Equal(invDate,                line.InvoiceDate);
            Assert.Equal(dueDate,                line.DueDate);
            Assert.Equal("Consulting services",  line.Description);
            Assert.Equal(500m,                   line.NetAmount);
            Assert.Equal(100m,                   line.VatAmount);
            Assert.Equal("Paid",                 line.Status);
            Assert.Equal(600m,                   line.GrossAmount);
        }
    }
}
