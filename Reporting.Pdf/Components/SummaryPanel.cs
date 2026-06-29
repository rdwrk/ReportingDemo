using System.Collections.Generic;
using MigraDoc.DocumentObjectModel;
using Reporting.Pdf.Styles;

namespace Reporting.Pdf.Components
{
    /// <summary>
    /// Renders a key/value summary panel as a shaded two-pair-per-row table.
    /// Typically placed immediately below the section heading and above the main data table.
    /// </summary>
    public static class SummaryPanel
    {
        /// <summary>
        /// Adds a summary panel to the section containing the supplied key/value pairs.
        /// Pairs are laid out two per row (label | value | label | value).
        /// </summary>
        /// <param name="section">The MigraDoc section to append the panel to.</param>
        /// <param name="items">Ordered dictionary of label → value strings to display.</param>
        /// <param name="totalWidthCm">Total width of the panel in centimetres; should match the page content width so the panel aligns with the data table.</param>
        public static void Add(Section section, Dictionary<string, string> items, double totalWidthCm = 14.0)
        {
            double colW = totalWidthCm / 4.0;

            var table = section.AddTable();
            table.Borders.Width = 0;
            table.BottomPadding = 0;
            table.TopPadding    = 0;
            table.LeftPadding   = Unit.FromPoint(4);
            table.RightPadding  = Unit.FromPoint(4);

            table.AddColumn(Unit.FromCentimeter(colW));
            table.AddColumn(Unit.FromCentimeter(colW));
            table.AddColumn(Unit.FromCentimeter(colW));
            table.AddColumn(Unit.FromCentimeter(colW));

            var keys   = new List<string>(items.Keys);
            var values = new List<string>(items.Values);

            for (int i = 0; i < keys.Count; i += 2)
            {
                var row = table.AddRow();
                row.Shading.Color = Color.Parse(ReportStyles.ColourGrey);
                row.TopPadding    = Unit.FromPoint(3);
                row.BottomPadding = Unit.FromPoint(3);

                row.Cells[0].AddParagraph(keys[i]).Style   = ReportStyles.SummaryLabel;
                row.Cells[1].AddParagraph(values[i]).Style = ReportStyles.SummaryValue;

                if (i + 1 < keys.Count)
                {
                    row.Cells[2].AddParagraph(keys[i + 1]).Style   = ReportStyles.SummaryLabel;
                    row.Cells[3].AddParagraph(values[i + 1]).Style = ReportStyles.SummaryValue;
                }
            }

            section.AddParagraph();
        }
    }
}
