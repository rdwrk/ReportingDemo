using System.IO;
using System.Threading.Tasks;
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
        /// Renders the supplied MigraDoc document directly into <paramref name="outputStream"/>.
        /// Preferred over <see cref="Render(Document)"/> when writing to an HTTP response or file
        /// stream because no intermediate byte array is allocated.
        /// </summary>
        /// <param name="document">Fully built MigraDoc document.</param>
        /// <param name="outputStream">Writable stream that receives the PDF bytes.</param>
        public void Render(Document document, Stream outputStream)
        {
            var renderer = new PdfDocumentRenderer { Document = document };
            renderer.RenderDocument();
            renderer.PdfDocument.Save(outputStream, false);
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

        /// <summary>
        /// Builds and renders directly into <paramref name="outputStream"/>, eliminating the
        /// intermediate byte array copy. Prefer this overload when writing straight to an HTTP
        /// response stream or <see cref="FileStream"/>.
        /// </summary>
        /// <typeparam name="TModel">Report model type.</typeparam>
        /// <param name="builder">Builder responsible for constructing the document.</param>
        /// <param name="model">Populated model to pass to the builder.</param>
        /// <param name="outputStream">Writable stream that receives the PDF bytes.</param>
        public void BuildAndRenderToStream<TModel>(IReportBuilder<TModel> builder, TModel model, Stream outputStream)
            where TModel : IReportModel
        {
            Render(builder.Build(model), outputStream);
        }

        /// <summary>
        /// Offloads <see cref="BuildAndRender{TModel}"/> to a thread-pool thread so that the
        /// calling thread (e.g. an ASP.NET request thread) is not blocked during CPU-bound
        /// PDF rendering. This is a thread-pool offload, not true async I/O — the work still
        /// occupies a thread; it just isn't the caller's thread.
        /// </summary>
        /// <typeparam name="TModel">Report model type.</typeparam>
        /// <param name="builder">Builder responsible for constructing the document.</param>
        /// <param name="model">Populated model to pass to the builder.</param>
        /// <returns>A task that completes with raw PDF bytes.</returns>
        public Task<byte[]> BuildAndRenderAsync<TModel>(IReportBuilder<TModel> builder, TModel model)
            where TModel : IReportModel
        {
            return Task.Run(() => BuildAndRender(builder, model));
        }

        /// <summary>
        /// Offloads <see cref="BuildAndRenderToStream{TModel}"/> to a thread-pool thread.
        /// Combines zero-copy stream writing with non-blocking rendering for the calling thread.
        /// This is a thread-pool offload, not true async I/O.
        /// </summary>
        /// <typeparam name="TModel">Report model type.</typeparam>
        /// <param name="builder">Builder responsible for constructing the document.</param>
        /// <param name="model">Populated model to pass to the builder.</param>
        /// <param name="outputStream">Writable stream that receives the PDF bytes.</param>
        /// <returns>A task that completes when the PDF has been written to the stream.</returns>
        public Task BuildAndRenderToStreamAsync<TModel>(IReportBuilder<TModel> builder, TModel model, Stream outputStream)
            where TModel : IReportModel
        {
            return Task.Run(() => BuildAndRenderToStream(builder, model, outputStream));
        }
    }
}
