using System.IO;
using MigraDoc.DocumentObjectModel;

namespace Reporting.Core.Interfaces
{
    /// <summary>
    /// Renders a MigraDoc <see cref="Document"/> to a binary format (PDF).
    /// Decouples document construction from the underlying PDF engine.
    /// </summary>
    public interface IReportRenderer
    {
        /// <summary>
        /// Renders the document to a PDF byte array suitable for streaming to a browser.
        /// </summary>
        /// <param name="document">The fully built MigraDoc document.</param>
        /// <returns>Raw PDF bytes.</returns>
        byte[] Render(Document document);

        /// <summary>
        /// Renders the document directly into <paramref name="outputStream"/>, eliminating
        /// the intermediate byte array copy incurred by <see cref="Render(Document)"/>.
        /// Preferred when writing straight to an HTTP response stream or file stream.
        /// </summary>
        /// <param name="document">The fully built MigraDoc document.</param>
        /// <param name="outputStream">Writable stream that receives the PDF bytes.</param>
        void Render(Document document, Stream outputStream);
    }
}
