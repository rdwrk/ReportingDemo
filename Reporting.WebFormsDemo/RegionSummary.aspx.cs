using System;
using System.Web.UI;

namespace Reporting.WebFormsDemo
{
    /// <summary>
    /// Displays the Region Performance Overview PDF inline on page load.
    /// No form or postback is required — the PDF is rendered automatically via <c>RegionPdf.ashx</c>.
    /// </summary>
    public partial class RegionSummary : Page
    {
        /// <summary>Points the iframe at <c>RegionPdf.ashx</c> on every GET.</summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            pdfFrame.Src = ResolveUrl("~/RegionPdf.ashx");
        }
    }
}
