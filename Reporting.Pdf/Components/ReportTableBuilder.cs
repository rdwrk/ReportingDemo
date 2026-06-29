using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using Reporting.Pdf.Styles;

namespace Reporting.Pdf.Components
{
    /// <summary>
    /// Fluent builder for styled MigraDoc data tables.
    /// Handles alternating row shading, group header rows, and total rows.
    /// Column widths are specified as relative weights and scaled proportionally
    /// to the available content width, so tables always fill the page exactly.
    /// Obtain an instance via <see cref="Create"/>.
    /// </summary>
    public class ReportTableBuilder
    {
        private readonly Table _table;
        private bool _altRow;

        private ReportTableBuilder(Table table)
        {
            _table = table;
        }

        /// <summary>
        /// Creates a new table in <paramref name="section"/>, derives absolute column widths
        /// by scaling <paramref name="colWeights"/> proportionally to <paramref name="totalWidthCm"/>,
        /// and adds a repeating heading row styled with <see cref="ReportStyles.TableHeader"/>.
        /// </summary>
        /// <param name="section">Section to add the table to.</param>
        /// <param name="totalWidthCm">
        /// Total available width in centimetres. Pass <c>GetContentWidthCm()</c> from the
        /// report builder so the table always spans the full content area regardless of page
        /// size, orientation, or margin changes.
        /// </param>
        /// <param name="colWeights">
        /// Relative weight for each column. Values are proportional to one another —
        /// a weight of 2 produces a column twice as wide as a weight of 1.
        /// The weights do not need to sum to any particular number.
        /// </param>
        /// <param name="headers">Header label for each column.</param>
        /// <param name="alignments">Optional per-column paragraph alignment; defaults to left.</param>
        /// <returns>A configured <see cref="ReportTableBuilder"/> ready to receive data rows.</returns>
        public static ReportTableBuilder Create(Section section, double totalWidthCm, double[] colWeights,
            string[] headers, ParagraphAlignment[] alignments = null)
        {
            double totalWeight = 0;
            foreach (var w in colWeights) totalWeight += w;

            var table = section.AddTable();
            table.Borders.Width = 0;
            table.Borders.Color = Color.Parse(ReportStyles.ColourMidGrey);
            table.BottomPadding = Unit.FromPoint(2);
            table.TopPadding    = Unit.FromPoint(2);
            table.LeftPadding   = Unit.FromPoint(4);
            table.RightPadding  = Unit.FromPoint(4);
            table.Style         = ReportStyles.TableBody;
            table.KeepTogether  = false;

            foreach (var w in colWeights)
                table.AddColumn(Unit.FromCentimeter(w / totalWeight * totalWidthCm))
                     .Format.Alignment = ParagraphAlignment.Left;

            var hdrRow = table.AddRow();
            hdrRow.HeadingFormat = true;
            hdrRow.Shading.Color = Color.Parse(ReportStyles.ColourNavy);
            hdrRow.TopPadding    = Unit.FromPoint(4);
            hdrRow.BottomPadding = Unit.FromPoint(4);

            for (int i = 0; i < headers.Length; i++)
            {
                var p = hdrRow.Cells[i].AddParagraph(headers[i]);
                p.Style = ReportStyles.TableHeader;
                if (alignments != null)
                    p.Format.Alignment = alignments[i];
            }

            return new ReportTableBuilder(table);
        }

        /// <summary>
        /// Adds a data row with alternating white / light-grey shading.
        /// </summary>
        /// <param name="values">Cell values in column order. Null entries render as empty.</param>
        /// <param name="alignments">Optional per-cell alignment; defaults to the column default.</param>
        /// <returns>The newly added <see cref="Row"/> for further customisation if needed.</returns>
        public Row AddRow(string[] values, ParagraphAlignment[] alignments = null)
        {
            var row = _table.AddRow();
            row.Shading.Color = _altRow
                ? Color.Parse(ReportStyles.ColourGrey)
                : Color.Parse(ReportStyles.ColourWhite);
            _altRow = !_altRow;

            for (int i = 0; i < values.Length; i++)
            {
                var p = row.Cells[i].AddParagraph(values[i] ?? string.Empty);
                p.Style = _altRow ? ReportStyles.TableBodyAlt : ReportStyles.TableBody;
                if (alignments != null)
                    p.Format.Alignment = alignments[i];
            }
            return row;
        }

        /// <summary>
        /// Adds a full-width group header row spanning all columns, styled with
        /// <see cref="ReportStyles.TableGroupHdr"/> and a mid-grey background.
        /// Sets <see cref="Row.KeepWith"/> = 2 to prevent the header orphaning at a page break.
        /// Resets the alternating row counter so the first data row after this is always white.
        /// </summary>
        /// <param name="label">Text to display in the group header.</param>
        public void AddGroupHeaderRow(string label)
        {
            var row = _table.AddRow();
            row.Shading.Color = Color.Parse(ReportStyles.ColourMidGrey);
            row.TopPadding    = Unit.FromPoint(4);
            row.BottomPadding = Unit.FromPoint(4);
            row.KeepWith      = 2;

            var cell = row.Cells[0];
            cell.MergeRight = _table.Columns.Count - 1;
            cell.AddParagraph(label).Style = ReportStyles.TableGroupHdr;

            _altRow = false;
        }

        /// <summary>
        /// Adds a subtotal or grand-total row with a blue background and white bold text.
        /// Resets the alternating row counter.
        /// </summary>
        /// <param name="label">Label text placed in the cell at <paramref name="labelColIndex"/>.</param>
        /// <param name="values">
        /// Values for the remaining columns in left-to-right order, skipping <paramref name="labelColIndex"/>.
        /// Null entries leave the cell blank.
        /// </param>
        /// <param name="labelColIndex">Zero-based index of the column that receives the label.</param>
        public void AddTotalRow(string label, string[] values, int labelColIndex = 0)
        {
            var row = _table.AddRow();
            row.Shading.Color = Color.Parse(ReportStyles.ColourBlue);
            row.TopPadding    = Unit.FromPoint(4);
            row.BottomPadding = Unit.FromPoint(4);

            var lp = row.Cells[labelColIndex].AddParagraph(label);
            lp.Style             = ReportStyles.TableTotal;
            lp.Format.Font.Color = Color.Parse(ReportStyles.ColourWhite);

            int valIdx = 0;
            for (int i = 0; i < _table.Columns.Count; i++)
            {
                if (i == labelColIndex) continue;
                if (valIdx < values.Length && values[valIdx] != null)
                {
                    var vp = row.Cells[i].AddParagraph(values[valIdx]);
                    vp.Style             = ReportStyles.TableTotal;
                    vp.Format.Alignment  = ParagraphAlignment.Right;
                    vp.Format.Font.Color = Color.Parse(ReportStyles.ColourWhite);
                }
                valIdx++;
            }
            _altRow = false;
        }
    }
}
