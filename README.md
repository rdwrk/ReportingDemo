# ReportingDemo

A PDF reporting solution built with **MigraDoc + PDFsharp 6.2.4**. The core PDF generation library targets `netstandard2.0` and is shared between two web front-ends: a modern ASP.NET Core 8 Razor Pages application and a classic ASP.NET Web Forms application targeting .NET Framework 4.8.

---

## Solution Structure

```
ReportingDemo.sln
├── Reporting.Core           (netstandard2.0)  – Contracts and models
├── Reporting.Pdf            (netstandard2.0)  – PDF builders and renderer
├── Reporting.WebDemo        (net8.0)          – ASP.NET Core Razor Pages host
└── Reporting.WebFormsDemo   (net4.8)          – ASP.NET Web Forms host
```

---

## Projects

### Reporting.Core

Defines the shared contracts used across all projects.

| Type | Purpose |
|------|---------|
| `IReportModel` | Marker interface for all report model classes |
| `IReportBuilder<TModel>` | Converts a model into a MigraDoc `Document` |
| `IReportRenderer` | Renders a `Document` to a `byte[]` PDF |
| `IReportDataService<TModel>` | Fetches or generates data for a given `ReportRequest` |
| `ReportRequest` | Carries date range, filter, and arbitrary key/value parameters |
| `ReportMetadata` | Title, subtitle, reference, classification, prepared-by, timestamp |
| `SalesReportModel` / `SalesLineItem` | Sales summary report data |
| `InvoiceReportModel` / `CustomerInvoiceGroup` / `InvoiceLine` | Invoice report data |

### Reporting.Pdf

Implements PDF generation on top of MigraDoc/PDFsharp.

| Type | Purpose |
|------|---------|
| `PdfReportRenderer` | Renders a `Document` → `byte[]`; exposes `BuildAndRender<TModel>()` so callers never reference MigraDoc types directly |
| `WindowsFontResolver` | `IFontResolver` implementation; maps Calibri, Arial and Courier New (regular/bold/italic) to TTF files in `C:\Windows\Fonts` |
| `MasterReportTemplate<TModel>` | Abstract base for report builders; handles page setup, repeating header, footer (timestamp + page numbers) |
| `ReportStyles` | Centralised MigraDoc style definitions (colours, fonts, paragraph styles) |
| `ReportTableBuilder` | Fluent builder for data tables: alternating row shading, group headers, total rows, repeating column headers on page breaks |
| `SummaryPanel` | Renders a key/value summary block as a two-pair-per-row table |
| `SalesReportBuilder` | Landscape A4 report: summary panel + paginated detail table |
| `InvoiceReportBuilder` | Portrait A4 report: grouped invoices per customer with subtotals |

### Reporting.WebDemo

ASP.NET Core 8 Razor Pages web application hosted on Kestrel.

| Type | Purpose |
|------|---------|
| `Program.cs` | Registers services, maps Razor Pages, exposes `/reports/stream` minimal API endpoint |
| `ReportService` | Orchestrates data retrieval and PDF rendering; returns `(byte[], fileName)` |
| `SalesReportDataService` | Generates 120 seeded random sales lines |
| `InvoiceReportDataService` | Generates 5 customer groups with 8–20 invoices each |
| `SalesReport.cshtml` | Filter form (date range, region, prepared-by) + inline PDF iframe |
| `InvoiceReport.cshtml` | Filter form (date range, prepared-by) + inline PDF iframe |

### Reporting.WebFormsDemo

ASP.NET Web Forms application targeting .NET Framework 4.8, hosted on IIS Express.

| Type | Purpose |
|------|---------|
| `StreamPdf.ashx` | Generic HTTP handler — streams PDF bytes with `Content-Disposition: inline` or `attachment` |
| `ReportService` | Orchestrates data retrieval and PDF rendering; returns a `ReportResult` value object |
| `SalesReportDataService` | Generates 120 seeded random sales lines |
| `InvoiceReportDataService` | Generates 5 customer groups with 8–20 invoices each |
| `SalesReport.aspx` | Filter form (date range, region, prepared-by) + inline PDF iframe |
| `InvoiceReport.aspx` | Filter form (date range, prepared-by) + inline PDF iframe |
| `Site.Master` | Master page with navy nav bar and custom CSS |

---

## Prerequisites

| Requirement | Notes |
|-------------|-------|
| Visual Studio 2022 / 2026 | **ASP.NET and web development** workload required |
| .NET SDK 8.0+ | Required for `Reporting.WebDemo` |
| .NET Framework 4.8 | Required for `Reporting.WebFormsDemo` (pre-installed on Windows 10/11) |
| IIS Express | Installed automatically with the ASP.NET workload — required for `Reporting.WebFormsDemo` |
| Windows OS | Required — `WindowsFontResolver` reads fonts from `C:\Windows\Fonts` |

NuGet restore handles all package dependencies including `PDFsharp-MigraDoc 6.2.4`.

---

## Getting Started

### Reporting.WebDemo (ASP.NET Core)

1. Open `ReportingDemo.sln` in Visual Studio.
2. Right-click **Reporting.WebDemo** → **Set as Startup Project**.
3. Press **F5** — the app starts on `http://localhost:5000`.

Or from the CLI:

```bash
cd Reporting.WebDemo
dotnet run
```

Then open `http://localhost:5000/SalesReport` or `http://localhost:5000/InvoiceReport`.

### Reporting.WebFormsDemo (Web Forms / .NET 4.8)

1. Right-click **Reporting.WebFormsDemo** → **Set as Startup Project**.
2. Press **F5** — IIS Express launches and opens the home page.
3. Navigate to **Sales Report** or **Invoice Report** in the nav bar.

---

## PDF Stream Endpoints

Both web projects expose equivalent endpoints that accept the same query-string parameters.

| Project | Endpoint |
|---------|----------|
| WebDemo | `GET /reports/stream` |
| WebFormsDemo | `GET /StreamPdf.ashx` |

| Parameter | Values | Description |
|-----------|--------|-------------|
| `report` | `sales`, `invoice` | Which report to generate |
| `inline` | `true`, `false` | `true` = display in browser; `false` = force download |
| `dateFrom` | `yyyy-MM-dd` | Start of reporting period |
| `dateTo` | `yyyy-MM-dd` | End of reporting period |
| `filter` | string | Region filter (sales report only) |
| `preparedBy` | string | Name shown in the report header |

Response: `Content-Type: application/pdf` with `Content-Disposition: inline` or `attachment`.

---

## Available Reports

### Sales Summary Report

- **Orientation:** Landscape A4
- **Reference:** RPT-SLS-001
- **Summary panel:** Total lines, total revenue, gross profit, margin %
- **Detail table:** Date, product, sales rep, region, units, unit price, revenue, gross profit — paginated across multiple pages with repeating column headers

### Invoice Summary Report

- **Orientation:** Portrait A4
- **Reference:** RPT-INV-001
- **Summary panel:** Total customers, total invoices, total value, outstanding value
- **Detail table:** Grouped by customer with invoice number, date, due date, description, amount and status — per-customer subtotals and a grand total row

---

## Adding a New Report

1. **Define the model** in `Reporting.Core\Models\` — implement `IReportModel`.
2. **Create a builder** in `Reporting.Pdf\Reports\` — extend `MasterReportTemplate<TModel>`.
3. **Create a data service** in the web project's `Services\` folder — implement `IReportDataService<TModel>`.
4. **Register the report** — add a `case` to `ReportService.Generate()` in each web project.
5. **Add a page** — a Razor Page (`.cshtml`) in WebDemo or an `.aspx` page in WebFormsDemo.

---

## Architecture Notes

- `Reporting.Core` and `Reporting.Pdf` target `netstandard2.0`, making them compatible with both .NET Framework 4.8 and .NET 8 without modification.
- `WindowsFontResolver` is registered once in `PdfReportRenderer`'s static constructor. PDFsharp on .NET Core does not read GDI+ system fonts automatically, so this is required.
- `PdfReportRenderer.BuildAndRender<TModel>()` keeps web layers fully decoupled from MigraDoc — neither web project needs to reference the `Document` type directly.
- `Reporting.WebFormsDemo` must reference `PDFsharp-MigraDoc` directly (in addition to the project references) because old-style `.csproj` files do not propagate transitive NuGet dependencies from `netstandard2.0` projects.
- Table column headers repeat on every page break (`HeadingFormat = true`).
- Group header rows use `KeepWith = 2` to prevent orphaned headers at page boundaries.
