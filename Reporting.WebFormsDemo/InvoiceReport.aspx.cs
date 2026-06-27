using System;
using System.Web.UI;
using Reporting.WebFormsDemo.Services;

namespace Reporting.WebFormsDemo
{
    /// <summary>
    /// Code-behind for the Invoice Summary Report page.
    /// Populates default filter values on first load and, on postback,
    /// builds a PDF stream URL and reveals the inline viewer iframe.
    /// </summary>
    public partial class InvoiceReport : Page
    {
        /// <summary>
        /// Sets default date range and prepared-by values when the page first loads.
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtDateFrom.Text   = DateTime.Today.AddMonths(-3).ToString("yyyy-MM-dd");
                txtDateTo.Text     = DateTime.Today.ToString("yyyy-MM-dd");
                txtPreparedBy.Text = "Finance Team";
            }
        }

        /// <summary>
        /// Handles the View PDF button click. Assembles the query string for
        /// <c>StreamPdf.ashx</c> from the current filter values and sets the
        /// iframe source, making the viewer panel visible.
        /// </summary>
        protected void btnView_Click(object sender, EventArgs e)
        {
            string url = string.Format(
                "~/StreamPdf.ashx?report=invoice&inline=true&dateFrom={0}&dateTo={1}&preparedBy={2}",
                Uri.EscapeDataString(txtDateFrom.Text),
                Uri.EscapeDataString(txtDateTo.Text),
                Uri.EscapeDataString(txtPreparedBy.Text.Trim()));

            pdfFrame.Src   = ResolveUrl(url);
            pnlPdf.Visible = true;
        }
    }
}
