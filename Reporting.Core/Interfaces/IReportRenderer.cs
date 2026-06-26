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
    }
}
