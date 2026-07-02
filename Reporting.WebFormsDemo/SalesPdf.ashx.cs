using System;
using System.Web;
using Reporting.Core.Models;
using Reporting.Core.Templates;
using Reporting.WebFormsDemo.Services;

namespace Reporting.WebFormsDemo
{
    /// <summary>
    /// HTTP handler for the Sales Summary Report.
    /// Reads date range, region filter, and prepared-by from the query string,
    /// fetches raw sales lines from <see cref="SalesReportDataService"/>,
    /// assembles a <see cref="SalesReportModel"/> with a populated <see cref="ReportMetadata"/>,
    /// and streams the rendered PDF to the browser.
    /// </summary>
    public class SalesPdf : IHttpHandler
    {
        /// <summary>
        /// Fetches sales lines, assembles the report model, renders the PDF, and writes
        /// the bytes to the HTTP response with the appropriate <c>Content-Disposition</c> header.
        /// </summary>
        public void ProcessRequest(HttpContext context)
        {
            var    qs         = context.Request.QueryString;
            string dateFrom   = qs["dateFrom"]   ?? string.Empty;
            string dateTo     = qs["dateTo"]     ?? string.Empty;
            string filter     = qs["filter"]     ?? string.Empty;
            string preparedBy = qs["preparedBy"] ?? string.Empty;
            bool   inline     = string.Equals(qs["inline"], "true", StringComparison.OrdinalIgnoreCase);

            if (filter.Length     > 100) filter     = filter.Substring(0, 100);
            if (preparedBy.Length > 150) preparedBy = preparedBy.Substring(0, 150);

            var from  = TryParse(dateFrom);
            var to    = TryParse(dateTo);
            var items = new SalesReportDataService().GetLines(from, to, filter);

            var model = new SalesReportModel
            {
                Metadata = new ReportMetadata
                {
                    PeriodFrom = from,
                    PeriodTo   = to,
                    Filter     = string.IsNullOrEmpty(filter) ? null : filter,
                    PreparedBy = string.IsNullOrEmpty(preparedBy) ? "System" : preparedBy,
                },
                Items = items,
            };

            var result = new ReportService().GenerateSales(model);

            string disp = inline
                ? string.Format("inline; filename=\"{0}\"", result.FileName)
                : string.Format("attachment; filename=\"{0}\"", result.FileName);

            context.Response.ContentType = "application/pdf";
            context.Response.AddHeader("Content-Disposition", disp);
            context.Response.BinaryWrite(result.Bytes);
            context.Response.End();
        }

        private static DateTime? TryParse(string value)
        {
            DateTime d;
            return DateTime.TryParse(value, out d) ? d : (DateTime?)null;
        }

        /// <summary>Returns <c>false</c> — a new handler instance is created per request.</summary>
        public bool IsReusable { get { return false; } }
    }
}
