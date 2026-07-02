using System;
using System.Web;
using Reporting.WebFormsDemo.Services;

namespace Reporting.WebFormsDemo
{
    /// <summary>
    /// Generic HTTP handler that generates a PDF report and streams it to the browser.
    /// Accepts the following query-string parameters:
    /// <list type="bullet">
    ///   <item><term>report</term><description>Case-insensitive report key: "sales" or "invoice".</description></item>
    ///   <item><term>inline</term><description>"true" to display in the browser; "false" to force download.</description></item>
    ///   <item><term>dateFrom</term><description>ISO date string (yyyy-MM-dd) for the period start.</description></item>
    ///   <item><term>dateTo</term><description>ISO date string (yyyy-MM-dd) for the period end.</description></item>
    ///   <item><term>filter</term><description>Optional region filter (sales report only).</description></item>
    ///   <item><term>preparedBy</term><description>Name or team shown in the report header.</description></item>
    /// </list>
    /// </summary>
    public class StreamPdf : IHttpHandler
    {
        /// <summary>
        /// Reads the query-string parameters, delegates to <see cref="ReportService"/>,
        /// and writes the resulting PDF bytes to the HTTP response with the correct
        /// <c>Content-Disposition</c> header.
        /// </summary>
        public void ProcessRequest(HttpContext context)
        {
            var    qs         = context.Request.QueryString;
            string report     = qs["report"]     ?? string.Empty;
            string dateFrom   = qs["dateFrom"]   ?? string.Empty;
            string dateTo     = qs["dateTo"]     ?? string.Empty;
            string filter     = qs["filter"]     ?? string.Empty;
            string preparedBy = qs["preparedBy"] ?? string.Empty;
            bool   inline     = string.Equals(qs["inline"], "true", StringComparison.OrdinalIgnoreCase);

            if (string.IsNullOrWhiteSpace(report))
            {
                context.Response.StatusCode  = 400;
                context.Response.ContentType = "text/plain";
                context.Response.Write("The 'report' query parameter is required.");
                context.Response.End();
                return;
            }

            if (filter.Length     > 100) filter     = filter.Substring(0, 100);
            if (preparedBy.Length > 150) preparedBy = preparedBy.Substring(0, 150);

            try
            {
                var result = new ReportService().Generate(report, dateFrom, dateTo, filter, preparedBy);

                string disp = inline
                    ? string.Format("inline; filename=\"{0}\"", result.FileName)
                    : string.Format("attachment; filename=\"{0}\"", result.FileName);

                context.Response.ContentType = "application/pdf";
                context.Response.AddHeader("Content-Disposition", disp);
                context.Response.BinaryWrite(result.Bytes);
                context.Response.End();
            }
            catch (ArgumentException ex)
            {
                context.Response.StatusCode  = 400;
                context.Response.ContentType = "text/plain";
                context.Response.Write(ex.Message);
                context.Response.End();
            }
        }

        /// <summary>
        /// Returns <c>false</c> — a new handler instance is created per request
        /// because <see cref="ReportService"/> holds a <see cref="Reporting.Pdf.PdfReportRenderer"/> instance.
        /// </summary>
        public bool IsReusable { get { return false; } }
    }
}
