using System;
using System.Web.UI;

namespace Reporting.WebFormsDemo
{
    /// <summary>
    /// Code-behind for the Sales Summary Report page.
    /// Populates default filter values on first load and, on postback,
    /// sets the iframe source to <c>SalesPdf.ashx</c> and reveals the inline viewer.
    /// </summary>
    public partial class SalesReport : Page
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
        /// Handles the View PDF button click. Builds the query string for <c>SalesPdf.ashx</c>
        /// from the current filter values and reveals the inline viewer iframe.
        /// </summary>
        protected void btnView_Click(object sender, EventArgs e)
        {
            string url = string.Format(
                "~/SalesPdf.ashx?inline=true&dateFrom={0}&dateTo={1}&filter={2}&preparedBy={3}",
                Uri.EscapeDataString(txtDateFrom.Text),
                Uri.EscapeDataString(txtDateTo.Text),
                Uri.EscapeDataString(ddlRegion.SelectedValue),
                Uri.EscapeDataString(txtPreparedBy.Text.Trim()));

            pdfFrame.Src   = ResolveUrl(url);
            pnlPdf.Visible = true;
        }
    }
}
