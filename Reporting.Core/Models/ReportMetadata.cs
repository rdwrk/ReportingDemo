using System;
using System.Collections.Generic;

namespace Reporting.Core.Models
{
    /// <summary>
    /// Descriptive metadata rendered in the report header and footer.
    /// <para>
    /// <b>Static fields</b> (<see cref="ReportTitle"/>, <see cref="LogoPath"/>, <see cref="AppName"/>, etc.)
    /// are either set by the builder or left at their defaults.
    /// </para>
    /// <para>
    /// <b>Context fields</b> (<see cref="PreparedBy"/>, <see cref="PeriodFrom"/>, <see cref="PeriodTo"/>,
    /// <see cref="Filter"/>) are set by the calling code — a page handler or HTTP handler — from
    /// user-supplied input and passed to the builder, which formats them into <see cref="HeaderLines"/>.
    /// </para>
    /// </summary>
    public class ReportMetadata
    {
        // ── Static header fields ─────────────────────────────────────────────────

        /// <summary>Main title displayed prominently in the page header.</summary>
        public string ReportTitle { get; set; } = string.Empty;

        /// <summary>
        /// Optional lines rendered below the title in the page header.
        /// Each entry becomes one subtitle-styled line. Leave empty for a clean title-only header.
        /// The top margin auto-expands by 0.5 cm per entry so content is never crowded.
        /// Populated by the builder's <c>GetMetadata</c> implementation from the context fields below.
        /// </summary>
        public List<string> HeaderLines { get; set; } = new List<string>();

        /// <summary>Absolute file path to a logo image rendered right-aligned in the header.</summary>
        public string LogoPath { get; set; }

        /// <summary>Width in centimetres at which to render the logo. Defaults to 3.0 cm.</summary>
        public double LogoWidthCm { get; set; } = 3.0;

        /// <summary>Application name shown on the left of the page footer. Defaults to "Reporting Demo".</summary>
        public string AppName { get; set; } = "Reporting Demo";

        /// <summary>Application version shown alongside <see cref="AppName"/> in the footer. Defaults to "1.0".</summary>
        public string AppVersion { get; set; } = "1.0";

        /// <summary>Timestamp embedded in the footer right column. Defaults to the current date/time.</summary>
        public DateTime GeneratedDate { get; set; } = DateTime.Now;

        // ── Report context fields (set by the caller, formatted by the builder) ──

        /// <summary>
        /// Person or team that requested the report. Shown in the page header when non-null/non-empty.
        /// Set by the calling code from user input before passing the model to the renderer.
        /// </summary>
        public string PreparedBy { get; set; }

        /// <summary>
        /// Start of the reporting period. Used by the builder to format a date-range header line.
        /// Set by the calling code from user input. Null means no lower bound.
        /// </summary>
        public DateTime? PeriodFrom { get; set; }

        /// <summary>
        /// End of the reporting period. Used by the builder to format a date-range header line.
        /// Set by the calling code from user input. Null means no upper bound.
        /// </summary>
        public DateTime? PeriodTo { get; set; }

        /// <summary>
        /// Optional free-text filter applied to the data (e.g. a region name for the Sales report).
        /// Used by the builder to format a filter description in the header. Null or empty means no filter.
        /// </summary>
        public string Filter { get; set; }
    }
}
