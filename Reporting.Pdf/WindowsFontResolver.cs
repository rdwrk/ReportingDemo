using System;
using System.Collections.Generic;
using System.IO;
using PdfSharp.Fonts;

namespace Reporting.Pdf
{
    /// <summary>
    /// Resolves font requests to TrueType files in the Windows system font directory.
    /// Required on .NET Core / .NET 8 because PDFsharp no longer reads GDI+ fonts automatically.
    /// </summary>
    /// <remarks>
    /// Add entries to <see cref="Map"/> whenever a new font family is referenced in report styles.
    /// Key format: lowercase family name with spaces removed, suffixed with "b" (bold) and/or "i" (italic).
    /// </remarks>
    public class WindowsFontResolver : IFontResolver
    {
        private static readonly string FontFolder =
            Environment.GetFolderPath(Environment.SpecialFolder.Fonts);

        /// <summary>
        /// Maps logical face keys (e.g. "calibrib") to physical .ttf filenames
        /// in <c>C:\Windows\Fonts</c>.
        /// </summary>
        private static readonly Dictionary<string, string> Map =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["calibri"]      = "calibri.ttf",
                ["calibrib"]     = "calibrib.ttf",
                ["calibrii"]     = "calibrii.ttf",
                ["calibribi"]    = "calibriz.ttf",
                ["arial"]        = "arial.ttf",
                ["arialb"]       = "arialbd.ttf",
                ["ariali"]       = "ariali.ttf",
                ["arialbi"]      = "arialbi.ttf",
                ["couriernew"]   = "cour.ttf",
                ["couriernewb"]  = "courbd.ttf",
                ["couriernewi"]  = "couri.ttf",
                ["couriernewbi"] = "courbi.ttf",
            };

        /// <summary>
        /// Maps a font family name and style flags to an internal face key used by <see cref="GetFont"/>.
        /// </summary>
        /// <param name="familyName">Font family (e.g. "Calibri").</param>
        /// <param name="bold">Whether the bold variant is requested.</param>
        /// <param name="italic">Whether the italic variant is requested.</param>
        /// <returns>A <see cref="FontResolverInfo"/> containing the face key.</returns>
        public FontResolverInfo ResolveTypeface(string familyName, bool bold, bool italic)
        {
            string key = familyName.Replace(" ", "") + (bold ? "b" : "") + (italic ? "i" : "");
            return new FontResolverInfo(key);
        }

        /// <summary>
        /// Loads the TrueType font bytes for the given face key produced by <see cref="ResolveTypeface"/>.
        /// </summary>
        /// <param name="faceName">Face key (e.g. "calibrib").</param>
        /// <returns>Raw TTF bytes.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the font cannot be found. Add the mapping to <see cref="Map"/> to fix.
        /// </exception>
        public byte[] GetFont(string faceName)
        {
            if (Map.TryGetValue(faceName, out string fileName))
            {
                string path = Path.Combine(FontFolder, fileName);
                if (File.Exists(path))
                    return File.ReadAllBytes(path);
            }

            string direct = Path.Combine(FontFolder, faceName + ".ttf");
            if (File.Exists(direct))
                return File.ReadAllBytes(direct);

            throw new InvalidOperationException(
                $"Font '{faceName}' not found in {FontFolder}. Add it to WindowsFontResolver.Map.");
        }
    }
}
