using System;
using System.IO;
using Reporting.Pdf;
using Xunit;

namespace Reporting.Tests.Pdf
{
    /// <summary>
    /// Verifies that <see cref="WindowsFontResolver"/> correctly maps font family names to
    /// face keys and loads TrueType font bytes from the Windows font directory.
    /// </summary>
    public class WindowsFontResolverTests
    {
        private readonly WindowsFontResolver _resolver = new WindowsFontResolver();

        // ResolveTypeface strips spaces and appends bold/italic suffixes but preserves
        // the original casing of the family name. GetFont's Map is case-insensitive so
        // "Arialb" and "arialb" resolve to the same TTF file.
        [Theory]
        [InlineData("Calibri",     false, false, "Calibri")]
        [InlineData("Calibri",     true,  false, "Calibrib")]
        [InlineData("Calibri",     false, true,  "Calibrii")]
        [InlineData("Calibri",     true,  true,  "Calibribi")]
        [InlineData("Arial",       false, false, "Arial")]
        [InlineData("Arial",       true,  false, "Arialb")]
        [InlineData("Arial",       false, true,  "Ariali")]
        [InlineData("Arial",       true,  true,  "Arialbi")]
        [InlineData("Courier New", false, false, "CourierNew")]
        [InlineData("Courier New", true,  false, "CourierNewb")]
        [InlineData("Courier New", false, true,  "CourierNewi")]
        [InlineData("Courier New", true,  true,  "CourierNewbi")]
        public void ResolveTypeface_ReturnsFaceKeyWithOriginalCasing(
            string family, bool bold, bool italic, string expectedKey)
        {
            var info = _resolver.ResolveTypeface(family, bold, italic);
            Assert.Equal(expectedKey, info.FaceName);
        }

        [Fact]
        public void ResolveTypeface_StripsSpacesFromFamilyName()
        {
            // "Courier New" strips to "CourierNew"; bold suffix appends "b".
            var info = _resolver.ResolveTypeface("Courier New", true, false);
            Assert.Equal("CourierNewb", info.FaceName);
        }

        // GetFont — mapped keys (Map uses OrdinalIgnoreCase so lowercase lookups work)
        [Theory]
        [InlineData("calibri")]
        [InlineData("calibrib")]
        [InlineData("calibrii")]
        [InlineData("calibribi")]
        [InlineData("arial")]
        [InlineData("arialb")]
        [InlineData("ariali")]
        [InlineData("arialbi")]
        [InlineData("couriernew")]
        [InlineData("couriernewb")]
        [InlineData("couriernewi")]
        [InlineData("couriernewbi")]
        public void GetFont_ReturnsNonEmptyBytesForAllMappedFaces(string faceName)
        {
            var bytes = _resolver.GetFont(faceName);
            Assert.NotNull(bytes);
            Assert.NotEmpty(bytes);
        }

        [Fact]
        public void GetFont_IsCaseInsensitiveForMappedKeys()
        {
            // Map uses StringComparer.OrdinalIgnoreCase — upper-case lookup must succeed.
            var lower = _resolver.GetFont("calibri");
            var upper = _resolver.GetFont("CALIBRI");
            Assert.Equal(lower.Length, upper.Length);
        }

        // GetFont — fallback direct TTF lookup
        [Fact]
        public void GetFont_FallsBackToDirectTtfFile_WhenFaceNotInMap()
        {
            // "tahoma" is not in the explicit Map but tahoma.ttf ships with Windows.
            // If it is absent on this machine the test passes vacuously.
            var fontsFolder = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
            var tahomaPath  = Path.Combine(fontsFolder, "tahoma.ttf");
            if (!File.Exists(tahomaPath))
                return;

            var bytes = _resolver.GetFont("tahoma");
            Assert.NotNull(bytes);
            Assert.NotEmpty(bytes);
        }

        // GetFont — error path
        [Fact]
        public void GetFont_ThrowsInvalidOperationException_ForCompletelyUnknownFace()
        {
            var ex = Assert.Throws<InvalidOperationException>(
                () => _resolver.GetFont("unknownFontXYZ999"));

            Assert.Contains("unknownFontXYZ999", ex.Message);
        }

        [Fact]
        public void GetFont_ExceptionMessage_ContainsFontsFolderPath()
        {
            var expectedFolder = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
            var ex = Assert.Throws<InvalidOperationException>(
                () => _resolver.GetFont("unknownFontXYZ999"));

            Assert.Contains(expectedFolder, ex.Message);
        }

        [Fact]
        public void GetFont_ExceptionMessage_ContainsMapInstructions()
        {
            var ex = Assert.Throws<InvalidOperationException>(
                () => _resolver.GetFont("unknownFontXYZ999"));

            Assert.Contains("WindowsFontResolver.Map", ex.Message);
        }
    }
}
