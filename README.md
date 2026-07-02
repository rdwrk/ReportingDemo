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
| `IReportRenderer` | Renders a `Document` to a `byte[]` PDF, or directly to a `Stream` to avoid an intermediate buffer copy |

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
| `PdfReportRenderer` | Renders a `Document` → `byte[]` or directly to a `Stream`; exposes `BuildAndRender<TModel>()`, `BuildAndRenderToStream<TModel>()`, and their `*Async` counterparts so callers never reference MigraDoc types directly |
| `WindowsFontResolver` | `IFontResolver` implementation; maps Calibri, Arial and Courier New (regular/bold/italic) to TTF files in `C:\Windows\Fonts` |
| `MasterReportTemplate<TModel>` | Abstract base for all report builders; handles page setup, repeating header, and three-column footer |
| `ReportStyles` | Single source of truth for every visual property — colours, font name, font sizes, and spacing. Edit this file only to restyle any report. |
| `ReportTableBuilder` | Fluent builder for data tables with proportional column widths, alternating row shading, group headers, total rows, and repeating column headers on page breaks |
| `SummaryPanel` | Renders a key/value summary block as a two-pair-per-row table |
| `LogoProvider` | Extracts the embedded default logo PNG to a temp file and returns its path for MigraDoc image loading |
| `SalesReportBuilder` | Landscape A4; filter-driven detail table with summary panel; renders empty table when `Items` is null or empty |
| `InvoiceReportBuilder` | Landscape A4; invoices grouped by customer with subtotals; overdue lines get orange/red highlighting; renders empty table when `CustomerGroups` is null or empty |
| `RegionSummaryReportBuilder` | Portrait A4; parameter-free overview aggregated by region; renders empty table when `Rows` is null or empty |

### Reporting.Tests

xUnit test project targeting net8.0. Covers `Reporting.Core` and `Reporting.Pdf` with 186 tests across 18 files.

| Folder | What is tested |
|--------|----------------|
| `Core/` | Default property values, computed expressions (`Revenue`, `GrossProfit`, `GrossAmount`, `Margin`, derived totals), and null-safety guards on all model and template classes |
| `Pdf/` | `WindowsFontResolver` face-key mapping and TTF loading; `ReportStyles` constant values and style registration; `ReportTableBuilder` row creation, alternating shading, group headers, total rows, and the column-count bounds guard; `SummaryPanel` even/odd item layouts; `LogoProvider` extraction and caching; all three report builders across every header/date/filter/data branch; `PdfReportRenderer` end-to-end `%PDF` output |

### Reporting.WebDemo

ASP.NET Core 8 Razor Pages web application hosted on Kestrel.

| Type | Purpose |
|------|---------|
| `Program.cs` | Registers services, maps Razor Pages, and exposes `GET /reports/region-summary` for the parameter-free region PDF |
| `ReportService` | Typed PDF renderer — `GenerateSales`, `GenerateInvoice`, `GenerateRegionSummary`; accepts a pre-built Core model and returns `(byte[], fileName)` |
| `SalesReportDataService` | DAL service — `GetLines(from, to, region)` returns `List<SalesLineItem>` |
| `InvoiceReportDataService` | DAL service — `GetCustomerGroups(from, to)` returns `InvoiceData` (groups + pre-computed grand totals) |
| `RegionSummaryDataService` | DAL service — `GetRows()` returns `List<RegionRow>` |
| `SalesReport.cshtml` | Filter form (date range, region, prepared-by); form targets the named iframe so the POST response is the PDF |
| `InvoiceReport.cshtml` | Filter form (date range, prepared-by); same iframe-target pattern |
| `RegionSummary.cshtml` | No form — iframe src points to `GET /reports/region-summary` |

### Reporting.WebFormsDemo

ASP.NET Web Forms application targeting .NET Framework 4.8, hosted on IIS Express.

| Type | Purpose |
|------|---------|
| `SalesPdf.ashx` | HTTP handler for the Sales Summary Report — fetches lines from `SalesReportDataService`, assembles the model, renders PDF |
| `InvoicePdf.ashx` | HTTP handler for the Invoice Summary Report — fetches groups from `InvoiceReportDataService`, assembles the model, renders PDF |
| `RegionPdf.ashx` | HTTP handler for the Region Performance Overview — fetches rows from `RegionSummaryDataService`, assembles the model, renders PDF |
| `ReportService` | Typed PDF renderer — `GenerateSales`, `GenerateInvoice`, `GenerateRegionSummary`; accepts a pre-built Core model and returns a `ReportResult` value object |
| `SalesReportDataService` | DAL service — `GetLines(from, to, region)` returns `List<SalesLineItem>` |
| `InvoiceReportDataService` | DAL service — `GetCustomerGroups(from, to)` returns `InvoiceData` (groups + pre-computed grand totals) |
| `RegionSummaryDataService` | DAL service — `GetRows()` returns `List<RegionRow>` |
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

## PDF Endpoints

### Reporting.WebDemo

| Method | Path | Description |
|--------|------|-------------|
| `POST` | `/SalesReport` | Form submission — fetches sales lines, assembles model, returns PDF |
| `POST` | `/InvoiceReport` | Form submission — fetches invoice groups, assembles model, returns PDF |
| `GET`  | `/reports/region-summary` | Parameter-free — fetches region rows, assembles model, returns PDF |

The Sales and Invoice Razor Pages use `<form method="post" target="pdfFrame">` so the POST response (the raw PDF) loads directly into the named iframe. No separate stream URL is needed.

### Reporting.WebFormsDemo

| Method | Path | Description |
|--------|------|-------------|
| `GET` | `/SalesPdf.ashx` | Fetches sales lines, assembles model, streams Sales Summary PDF |
| `GET` | `/InvoicePdf.ashx` | Fetches invoice groups, assembles model, streams Invoice Summary PDF |
| `GET` | `/RegionPdf.ashx` | Fetches region rows, assembles model, streams Region Overview PDF |

**`SalesPdf.ashx` parameters:**

| Parameter | Description |
|-----------|-------------|
| `inline` | `true` = display in browser; `false` = force download |
| `dateFrom` | ISO date (yyyy-MM-dd) for period start |
| `dateTo` | ISO date (yyyy-MM-dd) for period end |
| `filter` | Optional region filter; capped at 100 characters |
| `preparedBy` | Name shown in the report header; capped at 150 characters |

**`InvoicePdf.ashx` parameters:** same as above minus `filter`.

**`RegionPdf.ashx`:** no parameters required.

Response: `Content-Type: application/pdf` with `Content-Disposition: inline` or `attachment`.

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

1. **Define the model** in `Reporting.Core\Models\` — add any new data entity classes and a new `IReportModel` implementation in `Reporting.Core\Templates\`.
2. **Create a builder** in `Reporting.Pdf\Reports\` — extend `MasterReportTemplate<TModel>`.
   - Override `Orientation` if landscape is needed.
   - In `GetMetadata()`, set `HeaderLines` with any per-report context lines, and set `LogoPath = LogoProvider.GetPath()` to include the logo.
   - In `BuildContent()`, pass `GetContentWidthCm()` to `ReportTableBuilder.Create()` and `SummaryPanel.Add()`.
3. **Create a DAL service** in the web project's `Services\` folder. The method returns raw data entities — **not** a pre-built Core model. Example: `GetLines(DateTime? from, DateTime? to, string region)` returning `List<MyLineItem>`. The caller assembles the Core model from the returned data and any form metadata (dates, prepared-by, etc.).
4. **Add a typed render method** to `ReportService` in each web project: `GenerateMyReport(MyReportModel model)` returning `(byte[], fileName)` or `ReportResult`.
5. **Add a page:**
   - **WebDemo:** a Razor Page (`.cshtml` + `.cshtml.cs`). Inject the DAL service and `ReportService`. `OnPost` fetches raw data, assembles the Core model, calls `GenerateMyReport`, and returns `File(bytes, "application/pdf")`. Set `target="pdfFrame"` on the form and add `<iframe name="pdfFrame">` so the POST response loads in the iframe. For a parameter-free report, register a `GET` endpoint in `Program.cs` instead and point the iframe `src` at it.
   - **WebFormsDemo:** an `.aspx` page + code-behind + designer (register all in the `.csproj`); a dedicated `.ashx` + `.ashx.cs` pair (also register both in the `.csproj`). The ASHX handler calls the DAL service, assembles the Core model with a `ReportMetadata`, and calls the typed `ReportService` method. The ASPX code-behind builds the ASHX URL and points the iframe at it — no generic dispatcher involved.

---

## Architecture Notes

- `Reporting.Core` and `Reporting.Pdf` target `netstandard2.0`, making them compatible with both .NET Framework 4.8 and .NET 8 without modification.
- `WindowsFontResolver` is registered once in `PdfReportRenderer`'s static constructor. PDFsharp on .NET Core does not read GDI+ system fonts automatically, so this is required.
- `PdfReportRenderer.BuildAndRender<TModel>()` keeps web layers fully decoupled from MigraDoc — neither web project needs to reference the `Document` type directly. `BuildAndRenderToStream<TModel>()` writes directly to the caller's stream (e.g. an HTTP response stream) with no intermediate `byte[]` allocation. `BuildAndRenderAsync<TModel>()` and `BuildAndRenderToStreamAsync<TModel>()` wrap the CPU-bound render in `Task.Run`, freeing the calling thread during rendering — this is thread-pool offload, not true async I/O.
- Data services have no shared interface — each exposes a typed `Get*()` method (e.g. `GetLines`, `GetCustomerGroups`, `GetRows`) that returns raw data entities. The calling code (Razor Page handler or ASHX handler) assembles the Core model from those entities plus any user-supplied metadata such as `PreparedBy` or date range. `ReportService` in each web project then accepts the fully-populated Core model and renders it to PDF bytes — it never touches the database.
- This separation mirrors the real-world pattern where a DAL layer returns query results (rows, groups, aggregates) and the application layer maps them into the domain model before handing off to a downstream service.
- `Reporting.Core/Models/` contains raw data entities (one class per file). `Reporting.Core/Templates/` contains `IReportModel` implementations that hold the report-level aggregations. This split scales cleanly to 50+ reports without any single file growing large.
- `Reporting.WebFormsDemo` must reference `PDFsharp-MigraDoc` directly (in addition to the project references) because old-style `.csproj` files do not propagate transitive NuGet dependencies from `netstandard2.0` projects.
- `Reporting.WebFormsDemo` uses an old-style WAP `.csproj` that requires every `.cs` file to have an explicit `<Compile>` entry — unlike the SDK-style WebDemo project which discovers `.cs` files automatically.
- Table column headers repeat on every page break (`HeadingFormat = true`).
- Group header rows use `KeepWith = 2` to prevent orphaned headers at page boundaries.
- `ReportStyles` is the single file to edit when changing visual properties. Font name (`FontBody`), font sizes (`SizeTitle`, `SizeTable`, etc.), spacing (`SpaceSectionBefore`, etc.), and all colour hex strings are declared as named constants there and referenced by `Apply()` — no magic numbers are embedded in the method body.
- `MasterReportTemplate.GetContentWidthCm()` assumes A4 page dimensions (29.7 × 21.0 cm). If you add a report that overrides `PageFormat` to a non-A4 size, update this method accordingly.
- `ReportTableBuilder.AddRow()` silently ignores values beyond the declared column count, preventing `IndexOutOfRangeException` if a caller passes too many cells.
- All report builders guard against a null `Items` / `CustomerGroups` / `Rows` collection — an empty model renders a valid PDF with no data rows rather than throwing.
