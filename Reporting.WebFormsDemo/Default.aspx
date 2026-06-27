<%@ Page Title="Home" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
         CodeBehind="Default.aspx.cs" Inherits="Reporting.WebFormsDemo._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="rd-card">
        <h1>Reporting Demo</h1>
        <p style="color:#555;margin-bottom:0;">Select a report to generate a paginated PDF and view it inline.</p>
    </div>

    <div class="rd-report-grid">
        <div class="rd-report-card">
            <h3>&#128200; Sales Summary Report</h3>
            <p>Landscape A4 report showing sales lines by region with revenue, unit pricing, and gross profit across multiple pages.</p>
            <a href="SalesReport.aspx" class="rd-btn rd-btn-primary">Open Report &raquo;</a>
        </div>
        <div class="rd-report-card">
            <h3>&#128203; Invoice Summary Report</h3>
            <p>Portrait A4 report showing invoices grouped by customer with VAT breakdown, per-customer subtotals, and a grand total.</p>
            <a href="InvoiceReport.aspx" class="rd-btn rd-btn-primary">Open Report &raquo;</a>
        </div>
    </div>

</asp:Content>
