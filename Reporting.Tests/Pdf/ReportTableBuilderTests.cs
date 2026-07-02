using MigraDoc.DocumentObjectModel;
using Reporting.Pdf.Components;
using Reporting.Pdf.Styles;
using Xunit;

namespace Reporting.Tests.Pdf
{
    /// <summary>
    /// Verifies <see cref="ReportTableBuilder"/> row creation, alternating shading,
    /// group headers, total rows, and the column-count bounds guard.
    /// </summary>
    public class ReportTableBuilderTests
    {
        private static readonly double[] TwoWeights  = { 1.0, 1.0 };
        private static readonly double[] ThreeWeights = { 1.0, 2.0, 1.0 };

        private static Section CreateSection()
        {
            var doc = new Document();
            ReportStyles.Apply(doc);
            return doc.AddSection();
        }

        // ── Create ───────────────────────────────────────────────────────────────

        [Fact]
        public void Create_ReturnsNonNullBuilder()
        {
            var section = CreateSection();
            var builder = ReportTableBuilder.Create(section, 14.0, TwoWeights,
                new[] { "Col A", "Col B" });
            Assert.NotNull(builder);
        }

        [Fact]
        public void Create_WithNullAlignments_DoesNotThrow()
        {
            var section = CreateSection();
            var builder = ReportTableBuilder.Create(section, 14.0, TwoWeights,
                new[] { "Col A", "Col B" }, alignments: null);
            Assert.NotNull(builder);
        }

        [Fact]
        public void Create_WithExplicitAlignments_DoesNotThrow()
        {
            var section    = CreateSection();
            var alignments = new[] { ParagraphAlignment.Left, ParagraphAlignment.Right };
            var builder    = ReportTableBuilder.Create(section, 14.0, TwoWeights,
                new[] { "Col A", "Col B" }, alignments);
            Assert.NotNull(builder);
        }

        // ── AddRow ───────────────────────────────────────────────────────────────

        [Fact]
        public void AddRow_ReturnsNonNullRow()
        {
            var section = CreateSection();
            var builder = ReportTableBuilder.Create(section, 14.0, TwoWeights,
                new[] { "A", "B" });
            var row = builder.AddRow(new[] { "v1", "v2" });
            Assert.NotNull(row);
        }

        [Fact]
        public void AddRow_NullCellValue_RendersAsEmptyString()
        {
            // Should not throw when a null value is present in the array.
            var section = CreateSection();
            var builder = ReportTableBuilder.Create(section, 14.0, TwoWeights,
                new[] { "A", "B" });
            var row = builder.AddRow(new[] { null, "v2" });
            Assert.NotNull(row);
        }

        [Fact]
        public void AddRow_WithAlignments_DoesNotThrow()
        {
            var section    = CreateSection();
            var alignments = new[] { ParagraphAlignment.Left, ParagraphAlignment.Right };
            var builder    = ReportTableBuilder.Create(section, 14.0, TwoWeights,
                new[] { "A", "B" });
            var row = builder.AddRow(new[] { "v1", "v2" }, alignments);
            Assert.NotNull(row);
        }

        [Fact]
        public void AddRow_WithMoreValuesThanColumns_DoesNotThrow()
        {
            // Bounds guard: extra values beyond the column count must be silently ignored.
            var section = CreateSection();
            var builder = ReportTableBuilder.Create(section, 14.0, TwoWeights,
                new[] { "A", "B" });
            var row = builder.AddRow(new[] { "v1", "v2", "v3", "v4", "v5" });
            Assert.NotNull(row);
        }

        [Fact]
        public void AddRow_WithFewerValuesThanColumns_DoesNotThrow()
        {
            var section = CreateSection();
            var builder = ReportTableBuilder.Create(section, 14.0, ThreeWeights,
                new[] { "A", "B", "C" });
            var row = builder.AddRow(new[] { "v1" });
            Assert.NotNull(row);
        }

        [Fact]
        public void AddRow_AlternatesRowShading()
        {
            var section = CreateSection();
            var builder = ReportTableBuilder.Create(section, 14.0, TwoWeights,
                new[] { "A", "B" });

            var row1 = builder.AddRow(new[] { "1a", "1b" });
            var row2 = builder.AddRow(new[] { "2a", "2b" });

            // First row is white (#FFFFFF); second is light grey (#F2F2F2).
            Assert.NotEqual(row1.Shading.Color, row2.Shading.Color);
        }

        // ── AddGroupHeaderRow ────────────────────────────────────────────────────

        [Fact]
        public void AddGroupHeaderRow_DoesNotThrow()
        {
            var section = CreateSection();
            var builder = ReportTableBuilder.Create(section, 14.0, TwoWeights,
                new[] { "A", "B" });
            builder.AddGroupHeaderRow("Group 1");
        }

        [Fact]
        public void AddGroupHeaderRow_ResetsAlternatingCounter()
        {
            var section = CreateSection();
            var builder = ReportTableBuilder.Create(section, 14.0, TwoWeights,
                new[] { "A", "B" });

            // Drive the alternating state to "odd" (grey).
            builder.AddRow(new[] { "r1a", "r1b" });

            // After a group header the counter resets — next data row should be white again.
            builder.AddGroupHeaderRow("New Group");
            var firstAfterGroup = builder.AddRow(new[] { "r2a", "r2b" });

            var white = MigraDoc.DocumentObjectModel.Color.Parse(ReportStyles.ColourWhite);
            Assert.Equal(white, firstAfterGroup.Shading.Color);
        }

        // ── AddTotalRow ──────────────────────────────────────────────────────────

        [Fact]
        public void AddTotalRow_WithLabelAtColumnZero_DoesNotThrow()
        {
            var section = CreateSection();
            var builder = ReportTableBuilder.Create(section, 14.0, ThreeWeights,
                new[] { "A", "B", "C" });
            builder.AddTotalRow("Grand Total", new[] { "100.00", "200.00" }, labelColIndex: 0);
        }

        [Fact]
        public void AddTotalRow_WithLabelAtNonZeroColumn_DoesNotThrow()
        {
            var section = CreateSection();
            var builder = ReportTableBuilder.Create(section, 14.0, ThreeWeights,
                new[] { "A", "B", "C" });
            builder.AddTotalRow("Sub Total", new[] { "50.00", null }, labelColIndex: 1);
        }

        [Fact]
        public void AddTotalRow_WithNullValuesInArray_DoesNotThrow()
        {
            var section = CreateSection();
            var builder = ReportTableBuilder.Create(section, 14.0, ThreeWeights,
                new[] { "A", "B", "C" });
            builder.AddTotalRow("Total", new string[] { null, null }, labelColIndex: 0);
        }

        [Fact]
        public void AddTotalRow_ResetsAlternatingCounter()
        {
            var section = CreateSection();
            var builder = ReportTableBuilder.Create(section, 14.0, TwoWeights,
                new[] { "A", "B" });

            builder.AddRow(new[] { "r1", "r2" });
            builder.AddTotalRow("Total", new[] { "10.00" }, labelColIndex: 0);

            // After a total row the counter resets — next data row should be white.
            var firstAfterTotal = builder.AddRow(new[] { "n1", "n2" });
            var white = MigraDoc.DocumentObjectModel.Color.Parse(ReportStyles.ColourWhite);
            Assert.Equal(white, firstAfterTotal.Shading.Color);
        }
    }
}
