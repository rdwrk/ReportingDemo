using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Reporting.WebDemo.Services;

namespace Reporting.WebDemo.Pages
{
    /// <summary>
    /// Page model for the Invoice Summary Report page.
    /// Handles the filter form and sets <see cref="PdfUrl"/> so the Razor view
    /// can embed the generated PDF in an inline iframe.
    /// </summary>
    public class InvoiceReportModel : PageModel
    {
        private readonly ReportService _svc;

        /// <summary>Initialises the page model with the injected report service.</summary>
        public InvoiceReportModel(ReportService svc) => _svc = svc;

        /// <summary>Start of the reporting period in yyyy-MM-dd format. Defaults to three months ago.</summary>
        public string DateFrom   { get; set; } = DateTime.Today.AddMonths(-3).ToString("yyyy-MM-dd");

        /// <summary>End of the reporting period in yyyy-MM-dd format. Defaults to today.</summary>
        public string DateTo     { get; set; } = DateTime.Today.ToString("yyyy-MM-dd");

        /// <summary>Name or team shown in the report header.</summary>
        public string PreparedBy { get; set; } = "Finance Team";

        /// <summary>
        /// URL of the generated PDF stream. Set after a POST; null on initial GET
        /// so the iframe is hidden until the user requests the report.
        /// </summary>
        public string? PdfUrl    { get; set; }

        /// <summary>Renders the page with default filter values on initial load.</summary>
        public void OnGet() { }

        /// <summary>
        /// Handles the View PDF form submission. Builds the PDF stream URL from the
        /// submitted filter values and returns the page with the iframe populated.
        /// </summary>
        public IActionResult OnPost(string dateFrom, string dateTo, string preparedBy)
        {
            DateFrom   = dateFrom;
            DateTo     = dateTo;
            PreparedBy = preparedBy;
            PdfUrl     = $"/reports/stream?report=invoice&inline=true&dateFrom={dateFrom}&dateTo={dateTo}&preparedBy={Uri.EscapeDataString(preparedBy ?? "")}";
            return Page();
        }
    }
}
