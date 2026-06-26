using System;

namespace Reporting.Core.Models
{
    /// <summary>
    /// Descriptive metadata rendered in the report header and footer.
    /// Populated by each builder's <c>GetMetadata</c> override.
    /// </summary>
    public class ReportMetadata
    {
        /// <summary>Main title displayed prominently at the top of each page header.</summary>
        public string ReportTitle { get; set; }

        /// <summary>Secondary line beneath the title (e.g. date range or filter summary).</summary>
        public string Subtitle { get; set; }

        /// <summary>Security or sensitivity label shown in the header and footer (e.g. "Finance — Confidential").</summary>
        public string Classification { get; set; }

        /// <summary>Name or team that prepared the report, shown in the header.</summary>
        public string PreparedBy { get; set; }

        /// <summary>Unique report reference code (e.g. "RPT-SLS-001") for audit purposes.</summary>
        public string ReportReference { get; set; }

        /// <summary>Timestamp embedded in the footer. Defaults to the current date/time.</summary>
        public DateTime GeneratedDate { get; set; } = DateTime.Now;

        /// <summary>Optional file path to a logo image placed in the header.</summary>
        public string LogoPath { get; set; }

        /// <summary>Width (cm) to render the logo at. Defaults to 3 cm.</summary>
        public double LogoWidthCm { get; set; } = 3.0;
    }
}
