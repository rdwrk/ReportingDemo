using Reporting.Core.Templates;
using Reporting.Pdf;
using Reporting.Pdf.Reports;
using Reporting.WebDemo.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddScoped<ReportService>();
builder.Services.AddScoped<SalesReportDataService>();
builder.Services.AddScoped<InvoiceReportDataService>();
builder.Services.AddScoped<RegionSummaryDataService>();

var app = builder.Build();
app.UseStaticFiles();
app.MapRazorPages();

// Parameter-free report: fetches data and streams the PDF as a GET response so
// RegionSummary.cshtml can point its iframe src here without a form submission.
app.MapGet("/reports/region-summary", (RegionSummaryDataService dataSvc, HttpContext http) =>
{
    var rows  = dataSvc.GetRows();
    var model = new RegionSummaryReportModel { Rows = rows };
    var bytes = new PdfReportRenderer().BuildAndRender(new RegionSummaryReportBuilder(), model);
    var fileName = $"RegionPerformance_{DateTime.Today:yyyyMMdd}.pdf";
    http.Response.Headers["Content-Disposition"] = $"inline; filename=\"{fileName}\"";
    return Results.Bytes(bytes, "application/pdf");
});

app.Run();
