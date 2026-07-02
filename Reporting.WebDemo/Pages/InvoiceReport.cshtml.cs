using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Reporting.Core.Models;
using Reporting.WebDemo.Services;
using CoreModel = Reporting.Core.Templates.InvoiceReportModel;

namespace Reporting.WebDemo.Pages
{
    /// <summary>
    /// Page model for the Invoice Summary Report page.
    /// On GET: renders the filter form with default values.
    /// On POST: fetches pre-aggregated customer groups from the DAL service, assembles a <see cref="CoreModel"/>
    /// with a populated <see cref="ReportMetadata"/>, and streams the rendered PDF directly
    /// into the page's named iframe.
    /// </summary>
    public class InvoiceReportModel : PageModel
    {
        private readonly ReportService             _svc;
        private readonly InvoiceReportDataService  _dataSvc;

        /// <summary>Initialises the page model with the injected services.</summary>
        public InvoiceReportModel(ReportService svc, InvoiceReportDataService dataSvc)
        {
            _svc     = svc;
            _dataSvc = dataSvc;
        }

        /// <summary>Start of the reporting period in yyyy-MM-dd format. Defaults to three months ago.</summary>
        public string DateFrom   { get; set; } = DateTime.Today.AddMonths(-3).ToString("yyyy-MM-dd");

        /// <summary>End of the reporting period in yyyy-MM-dd format. Defaults to today.</summary>
        public string DateTo     { get; set; } = DateTime.Today.ToString("yyyy-MM-dd");

        /// <summary>Name or team shown in the report header.</summary>
        public string PreparedBy { get; set; } = "Finance Team";

        /// <summary>Renders the page with default filter values on initial load.</summary>
        public void OnGet() { }

        /// <summary>
        /// Handles the View PDF form submission. Fetches pre-aggregated customer groups from the DAL service,
        /// assembles the Core model with a <see cref="ReportMetadata"/> populated from the submitted
        /// form values, and streams the rendered PDF into the named iframe.
        /// </summary>
        public IActionResult OnPost(string dateFrom, string dateTo, string preparedBy)
        {
            var from = TryParse(dateFrom);
            var to   = TryParse(dateTo);

            var data = _dataSvc.GetCustomerGroups(from, to);

            var model = new CoreModel
            {
                Metadata = new ReportMetadata
                {
                    PeriodFrom = from,
                    PeriodTo   = to,
                    PreparedBy = string.IsNullOrWhiteSpace(preparedBy) ? "Finance Team" : preparedBy,
                },
                CustomerGroups    = data.CustomerGroups,
                TotalInvoiceCount = data.TotalInvoiceCount,
                GrandTotalNet     = data.GrandTotalNet,
                GrandTotalVat     = data.GrandTotalVat,
                GrandTotalGross   = data.GrandTotalGross,
                TotalOutstanding  = data.TotalOutstanding,
            };

            var (bytes, fileName) = _svc.GenerateInvoice(model);
            Response.Headers["Content-Disposition"] = $"inline; filename=\"{fileName}\"";
            return File(bytes, "application/pdf");
        }

        private static DateTime? TryParse(string value) =>
            DateTime.TryParse(value, out var d) ? d : (DateTime?)null;
    }
}
