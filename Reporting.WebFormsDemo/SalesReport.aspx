<%@ Page Title="Sales Report" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
         CodeBehind="SalesReport.aspx.cs" Inherits="Reporting.WebFormsDemo.SalesReport" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="rd-card">
        <h2>Sales Summary Report</h2>

        <div class="rd-form-row">
            <div class="rd-form-group">
                <label for="txtDateFrom">Date From</label>
                <asp:TextBox ID="txtDateFrom" runat="server" TextMode="Date" />
            </div>
            <div class="rd-form-group">
                <label for="txtDateTo">Date To</label>
                <asp:TextBox ID="txtDateTo" runat="server" TextMode="Date" />
            </div>
            <div class="rd-form-group">
                <label for="ddlRegion">Region</label>
                <asp:DropDownList ID="ddlRegion" runat="server">
                    <asp:ListItem Value="" Text="All Regions" />
                    <asp:ListItem Value="North" Text="North" />
                    <asp:ListItem Value="South" Text="South" />
                    <asp:ListItem Value="East" Text="East" />
                    <asp:ListItem Value="West" Text="West" />
                    <asp:ListItem Value="Midlands" Text="Midlands" />
                </asp:DropDownList>
            </div>
            <div class="rd-form-group">
                <label for="txtPreparedBy">Prepared By</label>
                <asp:TextBox ID="txtPreparedBy" runat="server" />
            </div>
        </div>

        <asp:Button ID="btnView" runat="server" Text="View PDF" CssClass="rd-btn rd-btn-primary"
                    OnClick="btnView_Click" />
    </div>

    <asp:Panel ID="pnlPdf" runat="server" Visible="false">
        <iframe id="pdfFrame" runat="server" class="rd-pdf-viewer"></iframe>
    </asp:Panel>

</asp:Content>
