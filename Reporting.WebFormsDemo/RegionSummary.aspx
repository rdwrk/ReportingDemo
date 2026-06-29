<%@ Page Title="Region Summary" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
         CodeBehind="RegionSummary.aspx.cs" Inherits="Reporting.WebFormsDemo.RegionSummary" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="rd-card">
        <h2>Region Performance Overview</h2>
        <p class="rd-muted">Live report — aggregated across all regions, no filters required.</p>
    </div>

    <iframe id="pdfFrame" runat="server" class="rd-pdf-viewer"></iframe>

</asp:Content>
