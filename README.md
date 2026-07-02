# ReportingDemo

A PDF reporting solution built with **MigraDoc + PDFsharp 6.2.4**. The core PDF generation library targets `netstandard2.0` and is shared between two web front-ends: a modern ASP.NET Core 8 Razor Pages application and a classic ASP.NET Web Forms application targeting .NET Framework 4.8.

---

## Solution Structure

```
ReportingDemo.sln
├── Reporting.Core           (netstandard2.0)  – Contracts and models
├── Reporting.Pdf            (netstandard2.0)  – PDF builders and renderer
├── Reporting.Tests          (net8.0)          – xUnit test project for Core and Pdf
├── Reporting.WebDemo        (net8.0)          – ASP.NET Core Razor Pages host
└── Reporting.WebFormsDemo   (net4.8)          – ASP.NET Web Forms host
```

---

## Projects

### Reporting.Core

Defines the shared contracts used across all projects.

**Interfaces** (`Reporting.Core/Interfaces/`)

| Type | Purpose |
|------|---------|
| `IReportModel` | Marker interface for all report model classes |
| `IReportBuilder<TModel>` | Converts a model into a MigraDoc `Document` |
| `IReportRenderer` | Renders a `Document` to a `byte[]` PDF |

**Models** (`Reporting.Core/Models/`) — raw data entities, one class per file

| Type | Purpose |
|------|---------|
| `ReportMetadata` | Title, optional header lines, logo path, app name/version, timestamp |
| `SalesLineItem` | One line of sales data; exposes `Revenue` and `GrossProfit` display expressions |
| `InvoiceLine` | One invoice line; exposes `GrossAmount` display expression |
| `CustomerInvoiceGroup` | Customer with pre-calculated subtotals (net, VAT, gross) |
| `RegionRow` | Aggregated region data; exposes `Margin` display expression |

**Templates** (`Reporting.Core/Templates/`) — `IReportModel` implementations, one class per report

| Type | Purpose |
|------|---------|
| `SalesReportModel` | Sales report root — holds `List<SalesLineItem>` and derived totals |
| `InvoiceReportModel` | Invoice report root — holds customer groups and pre-stored grand totals |
| `RegionSummaryReportModel` | Region report root — holds `List<RegionRow>` and derived grand totals |

### Reporting.Pdf

Implements PDF generation on top of MigraDoc/PDFsharp.

| Type | Purpose |
|------|---------|
| `PdfReportRenderer` | Renders a `Document` → `byte[]`; exposes `BuildAndRender<TModel>()` so callers never reference MigraDoc types directly |
| `WindowsFontResolver` | `IFontResolver` implementation; maps Calibri, Arial and Courier New (regular/bold/italic) to TTF files in `C:\Windows\Fonts` |
| `MasterReportTemplate<TModel>` | Abstract base for all report builders; handles page setup, repeating header, and three-column footer |
| `ReportStyles` | Centralised MigraDoc style definitions (colours, fonts, paragraph styles) |
| `ReportTableBuilder` | Fluent builder for data tables with proportional column widths, alternating row shading, group headers, total rows, and repeating column headers on page breaks |
| `SummaryPanel` | Renders a key/value summary block as a two-pair-per-row table |
| `LogoProvider` | Extracts the embedded default logo PNG to a temp file and returns its path for MigraDoc image loading |
| `SalesReportBuilder` | Landscape A4; filter-driven detail table with summary panel; renders empty table when `Items` is null or empty |
| `InvoiceReportBuilder` | Landscape A4; invoices grouped by customer with subtotals; overdue lines get orange/red highlighting; renders empty table when `CustomerGroups` is null or empty |
| `RegionSummaryReportBuilder` | Portrait A4; parameter-free overview aggregated by region; renders empty table when `Rows` is null or empty |

### Reporting.Tests

xUnit test project targeting net8.0. Covers `Reporting.Core` and `Reporting.Pdf` with 156 tests across 18 files.

| Folder | What is tested |
|--------|----------------|
| `Core/` | Default property values, computed expressions (`Revenue`, `GrossProfit`, `GrossAmount`, `Margin`, derived totals), and null-safety guards on all model and template classes |
| `Pdf/` | `WindowsFontResolver` face-key mapping and TTF loading; `ReportStyles` constant values and style registration; `ReportTableBuilder` row creation, alternating shading, group headers, total rows, and the column-count bounds guard; `SummaryPanel` even/odd item layouts; `LogoProvider` extraction and caching; all three report builders across every header/date/filter/data branch; `PdfReportRenderer` end-to-end `%PDF` output |

### Reporting.WebDemo

ASP.NET Core 8 Razor Pages web application hosted on Kestrel.

| Type | Purpose |
|------|---------|
| `Program.cs` | Registers services, maps Razor Pages, exposes `/reports/stream` minimal API endpoint |
| `ReportService` | Orchestrates data retrieval and PDF rendering; returns `(byte[], fileName)` |
| `SalesReportDataService` | Generates 120 seeded random sales lines |
| `InvoiceReportDataService` | Generates 5 customer groups with 8–20 invoices each |
| `RegionSummaryDataService` | Aggregates seeded sales data into per-region totals |
| `SalesReport.cshtml` | Filter form (date range, region, prepared-by) + inline PDF iframe |
| `InvoiceReport.cshtml` | Filter form (date range, prepared-by) + inline PDF iframe |
| `RegionSummary.cshtml` | No form — PDF renders automatically on page load |

### Reporting.WebFormsDemo

ASP.NET Web Forms application targeting .NET Framework 4.8, hosted on IIS Express.

| Type | Purpose |
|------|---------|
| `StreamPdf.ashx` | Generic HTTP handler — streams PDF bytes with `Content-Disposition: inline` or `attachment` |
| `ReportService` | Orchestrates data retrieval and PDF rendering; returns a `ReportResult` value object |
| `SalesReportDataService` | Generates 120 seeded random sales lines |
| `InvoiceReportDataService` | Generates 5 customer groups with 8–20 invoices each |
| `RegionSummaryDataService` | Aggregates seeded sales data into per-region totals |
| `SalesReport.aspx` | Filter form (date range, region, prepared-by) + inline PDF iframe |
| `InvoiceReport.aspx` | Filter form (date range, prepared-by) + inline PDF iframe |
| `RegionSummary.aspx` | No form — PDF renders automatically on page load |
| `Site.Master` | Master page with navy nav bar and custom CSS |

---

## Testing

`Reporting.Tests` is an xUnit project targeting net8.0 that provides near-complete coverage of `Reporting.Core` and `Reporting.Pdf`.

### Running the tests

```bash
cd Reporting.Tests
dotnet test
```

Or from the repository root:

```bash
dotnet test Reporting.Tests/Reporting.Tests.csproj
```

### Running with coverage

```bash
dotnet test Reporting.Tests/Reporting.Tests.csproj --collect:"XPlat Code Coverage"
```

Coverage reports land in `Reporting.Tests/TestResults/`. Use [ReportGenerator](https://github.com/danielpalme/ReportGenerator) to convert them to HTML:

```bash
reportgenerator -reports:"Reporting.Tests/TestResults/**/*.xml" -targetdir:"coverage-report" -reporttypes:Html
```

### Test organisation

| Folder | What is tested |
|--------|----------------|
| `Core/` | Default property values, computed expressions (`Revenue`, `GrossProfit`, `GrossAmount`, `Margin`, totals), and null-safety guards on all model and template classes |
| `Pdf/` | `WindowsFontResolver` face-key mapping and TTF loading; `ReportStyles` constant values and `Apply()` registration; `ReportTableBuilder` row creation, alternating shading, group headers, total rows, and the column-count bounds guard; `SummaryPanel` even/odd item layouts; `LogoProvider` extraction and caching; all three report builders across every header/date/filter/data branch; `PdfReportRenderer` end-to-end PDF byte output |

Builder tests render actual PDFs (via `PdfReportRenderer`) and assert the output starts with the `%PDF` magic bytes, verifying the full generation pipeline rather than just document assembly.

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

Then open `http://localhost:5000/SalesReport`, `http://localhost:5000/InvoiceReport`, or `http://localhost:5000/RegionSummary`.

### Reporting.WebFormsDemo (Web Forms / .NET 4.8)

1. Right-click **Reporting.WebFormsDemo** → **Set as Startup Project**.
2. Press **F5** — IIS Express launches and opens the home page.
3. Navigate to **Sales Report**, **Invoice Report**, or **Region Summary** in the nav bar.

---

## PDF Stream Endpoints

Both web projects expose equivalent endpoints that accept the same query-string parameters.

| Project | Endpoint |
|---------|----------|
| WebDemo | `GET /reports/stream` |
| WebFormsDemo | `GET /StreamPdf.ashx` |

| Parameter | Values | Description |
|-----------|--------|-------------|
| `report` | `sales`, `invoice`, `region-summary` | Which report to generate |
| `inline` | `true`, `false` | `true` = display in browser; `false` = force download |
| `dateFrom` | `yyyy-MM-dd` | Start of reporting period (sales and invoice only) |
| `dateTo` | `yyyy-MM-dd` | End of reporting period (sales and invoice only) |
| `filter` | string | Region filter (sales report only) |
| `preparedBy` | string | Name shown in the report header (sales and invoice only) |

Response: `Content-Type: application/pdf` with `Content-Disposition: inline` or `attachment`.

**Validation:** `report` is required — omitting it returns `400 Bad Request`. `filter` is capped at 100 characters and `preparedBy` at 150 characters; values exceeding these limits are silently truncated.

---

## Available Reports

### Sales Summary Report

- **Orientation:** Landscape A4
- **Reference:** RPT-SLS-001
- **Header lines:** Filter summary (region + date range), prepared-by, ref, classification
- **Summary panel:** Total lines, total revenue, gross profit, margin %
- **Detail table:** Date, product, sales rep, region, units, unit price, revenue, gross profit — paginated with repeating column headers

### Invoice Summary Report

- **Orientation:** Landscape A4
- **Reference:** RPT-INV-001
- **Header lines:** Date range subtitle, prepared-by, ref, classification
- **Summary panel:** Total customers, total invoices, total value, outstanding value
- **Detail table:** Grouped by customer — invoice number, date, due date, description, net, VAT, gross, status — per-customer subtotals and a grand total row

### Region Performance Overview

- **Orientation:** Portrait A4
- **Reference:** RPT-RGN-001
- **Parameters:** None — generates automatically with no user input
- **Summary panel:** Region count, total lines, total revenue, overall margin
- **Detail table:** One row per region showing lines, revenue, gross profit, margin %

---

## Page Header Design

Every report shares the same master header built by `MasterReportTemplate`:

- **Left:** Report title (large, navy) followed by any per-report `HeaderLines` (smaller blue text)
- **Right:** Company logo (embedded PNG, right-aligned)
- **Divider:** No divider line — content begins immediately below

`HeaderLines` is a `List<string>` on `ReportMetadata`. Each builder populates it with whatever context lines that report needs (date range, prepared-by, classification, etc.). Reports that need no extra context leave it empty. The page top margin auto-expands by **0.5 cm per header line** so content is never crowded regardless of how many lines a report adds.

## Page Footer Design

Three-column layout on every page:

| Left | Centre | Right |
|------|--------|-------|
| Application name + version | Page X of Y | Generated date/time |

Both `AppName` and `AppVersion` are properties on `ReportMetadata` with defaults of `"Reporting Demo"` and `"1.0"`.

---

## Dynamic Column Widths

`ReportTableBuilder.Create()` accepts **relative weights** rather than fixed centimetre values. Each column's actual width is computed as:

```
actualWidth = (weight / totalWeight) × totalWidthCm
```

`totalWidthCm` is provided by `MasterReportTemplate.GetContentWidthCm()`, which derives the available width from the page size, orientation, and margins. Tables always span the full content area regardless of page size, orientation, or margin changes — no manual width calculations needed when adding a new report.

---

## Logo

The default logo is a 200×56 px PNG (navy rounded rectangle, "RD" in white, "REPORTING DEMO" in gold) embedded as a resource in `Reporting.Pdf.dll`. `LogoProvider.GetPath()` extracts it to a temp file on first use and caches the path for the lifetime of the process, so no filesystem configuration is required. To use a custom logo, set `ReportMetadata.LogoPath` to an absolute file path and `ReportMetadata.LogoWidthCm` to the desired rendered width.

---

## Adding a New Report

1. **Define the model** in `Reporting.Core\Models\` — implement `IReportModel`.
2. **Create a builder** in `Reporting.Pdf\Reports\` — extend `MasterReportTemplate<TModel>`.
   - Override `Orientation` if landscape is needed.
   - In `GetMetadata()`, set `HeaderLines` with any per-report context lines, and set `LogoPath = LogoProvider.GetPath()` to include the logo.
   - In `BuildContent()`, pass `GetContentWidthCm()` to `ReportTableBuilder.Create()` and `SummaryPanel.Add()`.
3. **Create a data service** in the web project's `Services\` folder. Give `GetModel()` explicit typed parameters matching what the report needs (e.g. `GetModel(DateTime? dateFrom, DateTime? dateTo, string region)`). No interface to implement — the method signature is whatever the report requires.
4. **Register the report** — add a `case` to `ReportService.Generate()` in each web project.
5. **Add a page:**
   - WebDemo: a Razor Page (`.cshtml` + `.cshtml.cs`). For a parameter-free report, set `PdfUrl` in `OnGet()` and render the iframe directly — no form needed.
   - WebFormsDemo: an `.aspx` page + code-behind + designer; register all three files in the `.csproj`. For a parameter-free report, set `pdfFrame.Src` in `Page_Load`.

---

## Architecture Notes

- `Reporting.Core` and `Reporting.Pdf` target `netstandard2.0`, making them compatible with both .NET Framework 4.8 and .NET 8 without modification.
- `WindowsFontResolver` is registered once in `PdfReportRenderer`'s static constructor. PDFsharp on .NET Core does not read GDI+ system fonts automatically, so this is required.
- `PdfReportRenderer.BuildAndRender<TModel>()` keeps web layers fully decoupled from MigraDoc — neither web project needs to reference the `Document` type directly.
- Data services have no shared interface — each exposes a `GetModel()` method with explicit typed parameters matching what that report needs. In real usage, data comes from a database query and is mapped directly to the report model; there is no generic request bag or parameter dictionary.
- `Reporting.Core/Models/` contains raw data entities (one class per file). `Reporting.Core/Templates/` contains `IReportModel` implementations that hold the report-level aggregations. This split scales cleanly to 50+ reports without any single file growing large.
- `Reporting.WebFormsDemo` must reference `PDFsharp-MigraDoc` directly (in addition to the project references) because old-style `.csproj` files do not propagate transitive NuGet dependencies from `netstandard2.0` projects.
- `Reporting.WebFormsDemo` uses an old-style WAP `.csproj` that requires every `.cs` file to have an explicit `<Compile>` entry — unlike the SDK-style WebDemo project which discovers `.cs` files automatically.
- Table column headers repeat on every page break (`HeadingFormat = true`).
- Group header rows use `KeepWith = 2` to prevent orphaned headers at page boundaries.
- `MasterReportTemplate.GetContentWidthCm()` assumes A4 page dimensions (29.7 × 21.0 cm). If you add a report that overrides `PageFormat` to a non-A4 size, update this method accordingly.
- `ReportTableBuilder.AddRow()` silently ignores values beyond the declared column count, preventing `IndexOutOfRangeException` if a caller passes too many cells.
- All report builders guard against a null `Items` / `CustomerGroups` / `Rows` collection — an empty model renders a valid PDF with no data rows rather than throwing.
