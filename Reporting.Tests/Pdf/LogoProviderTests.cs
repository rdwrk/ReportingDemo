using System.IO;
using Reporting.Pdf.Assets;
using Xunit;

namespace Reporting.Tests.Pdf
{
    /// <summary>
    /// Verifies that <see cref="LogoProvider.GetPath"/> extracts the embedded logo and caches
    /// the result across multiple calls.
    /// </summary>
    public class LogoProviderTests
    {
        [Fact]
        public void GetPath_ReturnsNonEmptyPath()
        {
            var path = LogoProvider.GetPath();
            Assert.NotNull(path);
            Assert.NotEmpty(path);
        }

        [Fact]
        public void GetPath_ReturnsPathToExistingFile()
        {
            var path = LogoProvider.GetPath();
            Assert.True(File.Exists(path), $"Expected logo file to exist at: {path}");
        }

        [Fact]
        public void GetPath_ReturnsSamePath_OnSubsequentCalls()
        {
            // The provider caches the extraction path for the process lifetime.
            var path1 = LogoProvider.GetPath();
            var path2 = LogoProvider.GetPath();
            Assert.Equal(path1, path2);
        }

        [Fact]
        public void GetPath_ReturnedFile_IsNonEmpty()
        {
            var path = LogoProvider.GetPath();
            var info = new FileInfo(path);
            Assert.True(info.Length > 0, "Expected logo file to contain PNG bytes.");
        }
    }
}
