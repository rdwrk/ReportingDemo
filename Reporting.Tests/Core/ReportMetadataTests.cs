using System;
using System.Collections.Generic;
using Reporting.Core.Models;
using Xunit;

namespace Reporting.Tests.Core
{
    /// <summary>
    /// Verifies default property values and settability of <see cref="ReportMetadata"/>.
    /// </summary>
    public class ReportMetadataTests
    {
        [Fact]
        public void ReportTitle_DefaultsToEmptyString()
        {
            Assert.Equal(string.Empty, new ReportMetadata().ReportTitle);
        }

        [Fact]
        public void HeaderLines_DefaultsToEmptyList()
        {
            var meta = new ReportMetadata();
            Assert.NotNull(meta.HeaderLines);
            Assert.Empty(meta.HeaderLines);
        }

        [Fact]
        public void LogoPath_DefaultsToNull()
        {
            Assert.Null(new ReportMetadata().LogoPath);
        }

        [Fact]
        public void LogoWidthCm_DefaultsToThree()
        {
            Assert.Equal(3.0, new ReportMetadata().LogoWidthCm);
        }

        [Fact]
        public void AppName_DefaultsToReportingDemo()
        {
            Assert.Equal("Reporting Demo", new ReportMetadata().AppName);
        }

        [Fact]
        public void AppVersion_DefaultsToOnePointZero()
        {
            Assert.Equal("1.0", new ReportMetadata().AppVersion);
        }

        [Fact]
        public void GeneratedDate_DefaultsToApproximatelyNow()
        {
            var before = DateTime.Now.AddSeconds(-1);
            var meta   = new ReportMetadata();
            var after  = DateTime.Now.AddSeconds(1);
            Assert.InRange(meta.GeneratedDate, before, after);
        }

        [Fact]
        public void AllPropertiesCanBeOverridden()
        {
            var lines = new List<string> { "Line A", "Line B" };
            var stamp = new DateTime(2024, 6, 1, 9, 0, 0);
            var meta  = new ReportMetadata
            {
                ReportTitle   = "My Report",
                HeaderLines   = lines,
                LogoPath      = @"C:\logo.png",
                LogoWidthCm   = 4.5,
                AppName       = "TestApp",
                AppVersion    = "3.0",
                GeneratedDate = stamp,
            };

            Assert.Equal("My Report",    meta.ReportTitle);
            Assert.Equal(lines,          meta.HeaderLines);
            Assert.Equal(@"C:\logo.png", meta.LogoPath);
            Assert.Equal(4.5,            meta.LogoWidthCm);
            Assert.Equal("TestApp",      meta.AppName);
            Assert.Equal("3.0",          meta.AppVersion);
            Assert.Equal(stamp,          meta.GeneratedDate);
        }

        // ── Report context fields ─────────────────────────────────────────────────

        [Fact]
        public void PreparedBy_DefaultsToNull()
        {
            Assert.Null(new ReportMetadata().PreparedBy);
        }

        [Fact]
        public void PeriodFrom_DefaultsToNull()
        {
            Assert.Null(new ReportMetadata().PeriodFrom);
        }

        [Fact]
        public void PeriodTo_DefaultsToNull()
        {
            Assert.Null(new ReportMetadata().PeriodTo);
        }

        [Fact]
        public void Filter_DefaultsToNull()
        {
            Assert.Null(new ReportMetadata().Filter);
        }

        [Fact]
        public void ContextFields_CanBeSet()
        {
            var from = new DateTime(2024, 1, 1);
            var to   = new DateTime(2024, 3, 31);
            var meta = new ReportMetadata
            {
                PreparedBy = "Alice",
                PeriodFrom = from,
                PeriodTo   = to,
                Filter     = "North",
            };

            Assert.Equal("Alice", meta.PreparedBy);
            Assert.Equal(from,    meta.PeriodFrom);
            Assert.Equal(to,      meta.PeriodTo);
            Assert.Equal("North", meta.Filter);
        }
    }
}
