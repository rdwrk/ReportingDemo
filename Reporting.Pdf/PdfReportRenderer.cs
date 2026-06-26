using System.IO;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using PdfSharp.Fonts;
using Reporting.Core.Interfaces;

namespace Reporting.Pdf
{
    /// <summary>
    /// Renders MigraDoc <see cref="Document"/> instances to PDF bytes using PDFsharp.
    /// Registers <see cref="WindowsFontResolver"/> on first use so that system fonts
    /// (Calibri, Arial, etc.) are available on .NET Core / .NET 8.
    /// </summary>
    public class PdfReportRenderer : IReportRenderer
    {
        static PdfReportRenderer()
        {
            if (GlobalFontSettings.FontResolver == null)
                GlobalFontSettings.FontResolver = new WindowsFontResolver();
        }

        /// <summary>
        /// Renders the supplied MigraDoc document to a PDF byte array.
        /// </summary>
        /// <param name="document">Fully built MigraDoc document.</param>
        /// <returns>Raw PDF bytes ready to stream to a browser or write to disk.</returns>
        public byte[] Render(Document document)
        {
            var renderer = new PdfDocumentRenderer { Document = document };
            renderer.RenderDocument();
            using (var ms = new MemoryStream())
            {
                renderer.PdfDocument.Save(ms, false);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Convenience method: builds and renders in one call so callers
        /// never need to reference the MigraDoc <see cref="Document"/> type directly.
        /// </summary>
        /// <typeparam name="TModel">Report model type.</typeparam>
        /// <param name="builder">Builder responsible for constructing the document.</param>
        /// <param name="model">Populated model to pass to the builder.</param>
        /// <returns>Raw PDF bytes.</returns>
        public byte[] BuildAndRender<TModel>(IReportBuilder<TModel> builder, TModel model)
            where TModel : IReportModel
        {
            return Render(builder.Build(model));
        }
    }
}
