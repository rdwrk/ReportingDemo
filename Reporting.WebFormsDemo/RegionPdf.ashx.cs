using System.Web;
using Reporting.Core.Templates;
using Reporting.WebFormsDemo.Services;

namespace Reporting.WebFormsDemo
{
    /// <summary>
    /// HTTP handler for the Region Performance Overview report.
    /// Fetches per-region rows from <see cref="RegionSummaryDataService"/>,
    /// assembles a <see cref="RegionSummaryReportModel"/>, and streams the rendered PDF to the browser.
    /// No query-string parameters are required — the report always covers the full dataset.
    /// </summary>
    public class RegionPdf : IHttpHandler
    {
        /// <summary>
        /// Fetches region rows, assembles the report model, renders the PDF, and writes
        /// the bytes to the HTTP response with an inline <c>Content-Disposition</c> header.
        /// </summary>
        public void ProcessRequest(HttpContext context)
        {
            var rows  = new RegionSummaryDataService().GetRows();
            var model = new RegionSummaryReportModel { Rows = rows };
            var result = new ReportService().GenerateRegionSummary(model);

            context.Response.ContentType = "application/pdf";
            context.Response.AddHeader("Content-Disposition",
                string.Format("inline; filename=\"{0}\"", result.FileName));
            context.Response.BinaryWrite(result.Bytes);
            context.Response.End();
        }

        /// <summary>Returns <c>false</c> — a new handler instance is created per request.</summary>
        public bool IsReusable { get { return false; } }
    }
}
