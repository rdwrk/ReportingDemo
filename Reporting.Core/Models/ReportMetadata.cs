using System;
using System.Collections.Generic;

namespace Reporting.Core.Models
{
    /// <summary>
    /// Descriptive metadata rendered in the report header and footer.
    /// Populated by each builder's <c>GetMetadata</c> override.
    /// </summary>
    public class ReportMetadata
    {
        /// <summary>Main title displayed prominently in the page header.</summary>
        public string ReportTitle { get; set; }

        /// <summary>
        /// Optional lines rendered below the title in the page header.
        /// Each entry becomes one subtitle-styled line. Leave empty for a clean title-only header.
        /// The top margin auto-expands by 0.5 cm per entry so content is never crowded.
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
    }
}
