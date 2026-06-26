# ReportingDemo

A PDF reporting solution built with **MigraDoc + PDFsharp 6.2.4**, hosted in an ASP.NET Core 8 Razor Pages web application. Reports are generated server-side and displayed inline in the browser via an embedded PDF viewer.

---

## Solution Structure

```
ReportingDemo.sln
├── Reporting.Core          (netstandard2.0)  – Contracts and models
├── Reporting.Pdf           (netstandard2.0)  – PDF builders and renderer
└── Reporting.WebDemo       (net8.0)          – ASP.NET Core web host
```

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
| `WindowsFontResolver` | `IFontResolver` implementation required on .NET 8; maps Calibri, Arial and Courier New (regular/bold/italic) to TTF files in `C:\Windows\Fonts` |
| `MasterReportTemplate<TModel>` | Abstract base for report builders; handles page setup, repeating header, footer (timestamp + page numbers) |
| `ReportStyles` | Centralised MigraDoc style definitions (colours, fonts, paragraph styles) |
| `ReportTableBuilder` | Fluent builder for data tables: alternating row shading, group headers, total rows, repeating column headers on page breaks |
| `SummaryPanel` | Renders a key/value summary block as a two-pair-per-row table |
| `SalesReportBuilder` | Landscape A4 report: summary panel + paginated detail table |
| `InvoiceReportBuilder` | Portrait A4 report: grouped invoices per customer with subtotals |

### Reporting.WebDemo

ASP.NET Core 8 Razor Pages web application.

| Type | Purpose |
|------|---------|
| `Program.cs` | Registers services, maps Razor Pages, exposes `/reports/stream` minimal API endpoint |
| `ReportService` | Orchestrates data retrieval and PDF rendering; returns `(byte[], fileName)` |
| `SalesReportDataService` | Generates 120 seeded random sales lines |
| `InvoiceReportDataService` | Generates 5 customer groups with 8–20 invoices each |
| `SalesReport.cshtml` | Filter form (date range, region, prepared-by) + inline PDF iframe |
| `InvoiceReport.cshtml` | Filter form (date range, prepared-by) + inline PDF iframe |

---

## Prerequisites

| Requirement | Version |
|-------------|---------|
| Visual Studio | 2022 / 2026 with **ASP.NET and web development** workload |
| .NET SDK | 8.0+ |
| OS | Windows (required for `WindowsFontResolver` — reads fonts from `C:\Windows\Fonts`) |

No additional installs are needed. NuGet restore pulls `PDFsharp-MigraDoc 6.2.4` automatically.

---

## Getting Started

1. Open `ReportingDemo.sln` in Visual Studio.
2. Set **Reporting.WebDemo** as the startup project.
3. Press **F5** (or **Ctrl+F5**) — the app starts on `http://localhost:5000` and opens the Sales Report page.
4. Select filters and click **View PDF** to generate and display the report inline.

To run from the CLI:

```bash
cd Reporting.WebDemo
dotnet run
```

Then navigate to `http://localhost:5000/SalesReport` or `http://localhost:5000/InvoiceReport`.

---

## Report Endpoint

PDF bytes are streamed by a minimal API endpoint:

```
GET /reports/stream?report={name}&inline={true|false}&dateFrom={date}&dateTo={date}&filter={value}&preparedBy={name}
```

| Parameter | Values | Description |
|-----------|--------|-------------|
| `report` | `sales`, `invoice` | Which report to generate |
| `inline` | `true`, `false` | `true` = display in browser; `false` = force download |
| `dateFrom` | ISO date (`yyyy-MM-dd`) | Start of reporting period |
| `dateTo` | ISO date (`yyyy-MM-dd`) | End of reporting period |
| `filter` | string | Region filter (sales report only) |
| `preparedBy` | string | Name shown in the report header |

Response: `Content-Type: application/pdf` with `Content-Disposition: inline` or `attachment`.

---

## Available Reports

### Sales Summary Report

- **Orientation:** Landscape A4
- **Reference:** RPT-SLS-001
- **Summary panel:** Total lines, total revenue, gross profit, margin %
- **Detail table:** Date, product, sales rep, region, units, unit price, revenue, gross profit — paginated across multiple pages

### Invoice Summary Report

- **Orientation:** Portrait A4
- **Reference:** RPT-INV-001
- **Summary panel:** Total customers, total invoices, total value, outstanding value
- **Detail table:** Grouped by customer, showing invoice number, date, due date, description, amount, status — with per-customer subtotals and a grand total

---

## Adding a New Report

1. **Define the model** in `Reporting.Core\Models\` — implement `IReportModel`.
2. **Create a data service** in `Reporting.WebDemo\Services\` — implement `IReportDataService<TModel>`.
3. **Create a builder** in `Reporting.Pdf\Reports\` — extend `MasterReportTemplate<TModel>`.
4. **Register the report** in `ReportService.Generate()` — add a `case` for the new report name.
5. **Add a Razor Page** in `Reporting.WebDemo\Pages\` for the filter form and iframe.

---

## Architecture Notes

- `Reporting.Core` and `Reporting.Pdf` target `netstandard2.0` so they are compatible with both .NET Framework 4.8 and .NET 8.
- `Reporting.WebDemo` targets `net8.0` and uses Kestrel — no IIS Express or `applicationhost.config` required.
- `WindowsFontResolver` is registered once via `PdfReportRenderer`'s static constructor and must be set before any `PdfDocumentRenderer` is created.
- `PdfReportRenderer.BuildAndRender<TModel>()` keeps the web layer decoupled from MigraDoc — it never needs to reference the `Document` type directly.
- Table column headers repeat on every page break (`HeadingFormat = true`).
- Group header rows use `KeepWith = 2` to prevent orphaned headers at page boundaries.
