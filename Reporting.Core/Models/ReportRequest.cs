using System;
using System.Collections.Generic;

namespace Reporting.Core.Models
{
    /// <summary>
    /// Carries the user-supplied parameters for a report request.
    /// Passed from the web layer to a data service to scope the data retrieved.
    /// </summary>
    public class ReportRequest
    {
        /// <summary>Start of the date range to include (inclusive). Null means no lower bound.</summary>
        public DateTime? DateFrom { get; set; }

        /// <summary>End of the date range to include (inclusive). Null means no upper bound.</summary>
        public DateTime? DateTo { get; set; }

        /// <summary>Optional free-text or category filter (e.g. region name).</summary>
        public string Filter { get; set; }

        /// <summary>
        /// Additional named parameters that individual reports may require
        /// (e.g. "preparedBy", "region").
        /// </summary>
        public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
    }
}
