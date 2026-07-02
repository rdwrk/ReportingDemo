using System.Collections.Generic;
using MigraDoc.DocumentObjectModel;
using Reporting.Pdf.Components;
using Reporting.Pdf.Styles;
using Xunit;

namespace Reporting.Tests.Pdf
{
    /// <summary>
    /// Verifies that <see cref="SummaryPanel.Add"/> renders without error for even and odd item counts
    /// and accepts both the default and an explicit <c>totalWidthCm</c>.
    /// </summary>
    public class SummaryPanelTests
    {
        private static Section CreateSection()
        {
            var doc = new Document();
            ReportStyles.Apply(doc);
            return doc.AddSection();
        }

        [Fact]
        public void Add_WithEvenNumberOfItems_DoesNotThrow()
        {
            var section = CreateSection();
            var items = new Dictionary<string, string>
            {
                ["Label A"] = "Value A",
                ["Label B"] = "Value B",
                ["Label C"] = "Value C",
                ["Label D"] = "Value D",
            };
            SummaryPanel.Add(section, items, 14.0);
        }

        [Fact]
        public void Add_WithOddNumberOfItems_DoesNotThrow()
        {
            // Odd count: last row has only a left-hand pair; right cells remain empty.
            var section = CreateSection();
            var items = new Dictionary<string, string>
            {
                ["Label A"] = "Value A",
                ["Label B"] = "Value B",
                ["Label C"] = "Value C",
            };
            SummaryPanel.Add(section, items, 14.0);
        }

        [Fact]
        public void Add_WithSingleItem_DoesNotThrow()
        {
            var section = CreateSection();
            var items = new Dictionary<string, string> { ["Only"] = "One" };
            SummaryPanel.Add(section, items, 14.0);
        }

        [Fact]
        public void Add_WithEmptyDictionary_DoesNotThrow()
        {
            var section = CreateSection();
            SummaryPanel.Add(section, new Dictionary<string, string>(), 14.0);
        }

        [Fact]
        public void Add_WithDefaultTotalWidthCm_DoesNotThrow()
        {
            // Omitting the optional parameter should use the 14.0 cm default.
            var section = CreateSection();
            var items = new Dictionary<string, string> { ["K"] = "V" };
            SummaryPanel.Add(section, items);
        }

        [Fact]
        public void Add_WithCustomTotalWidthCm_DoesNotThrow()
        {
            var section = CreateSection();
            var items = new Dictionary<string, string> { ["K"] = "V" };
            SummaryPanel.Add(section, items, totalWidthCm: 25.7);
        }
    }
}
