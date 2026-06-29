using System;
using System.Web.UI;

namespace Reporting.WebFormsDemo
{
    /// <summary>
    /// Displays the Region Performance Overview PDF inline on page load.
    /// No form or postback is required — the PDF is rendered automatically.
    /// </summary>
    public partial class RegionSummary : Page
    {
        /// <summary>Points the iframe at StreamPdf.ashx on every GET.</summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            pdfFrame.Src = ResolveUrl("~/StreamPdf.ashx?report=region-summary&inline=true");
        }
    }
}
