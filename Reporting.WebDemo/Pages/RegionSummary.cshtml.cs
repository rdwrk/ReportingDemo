using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Reporting.WebDemo.Pages
{
    /// <summary>
    /// Displays the Region Performance Overview report directly in an inline PDF viewer.
    /// No form or user input is required — the report generates automatically on page load.
    /// </summary>
    public class RegionSummaryModel : PageModel
    {
        /// <summary>URL that the PDF viewer iframe points to. Set on every GET request.</summary>
        public string PdfUrl { get; private set; }

        public void OnGet()
        {
            PdfUrl = "/reports/stream?report=region-summary&inline=true";
        }
    }
}
