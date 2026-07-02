using Reporting.WebDemo.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddScoped<ReportService>();

var app = builder.Build();
app.UseStaticFiles();
app.MapRazorPages();

app.MapGet("/reports/stream", (
    string report,
    string? dateFrom,
    string? dateTo,
    string? filter,
    string? preparedBy,
    bool inline,
    ReportService svc,
    HttpContext http) =>
{
    if (string.IsNullOrWhiteSpace(report))
        return Results.BadRequest("The 'report' query parameter is required.");

    if (filter     is { Length: > 100 }) filter     = filter[..100];
    if (preparedBy is { Length: > 150 }) preparedBy = preparedBy[..150];

    try
    {
        var (bytes, fileName) = svc.Generate(report, dateFrom, dateTo, filter, preparedBy);
        var disposition = inline
            ? $"inline; filename=\"{fileName}\""
            : $"attachment; filename=\"{fileName}\"";
        http.Response.Headers["Content-Disposition"] = disposition;
        return Results.Bytes(bytes, "application/pdf");
    }
    catch (ArgumentException ex)
    {
        return Results.NotFound(ex.Message);
    }
});

app.Run();
