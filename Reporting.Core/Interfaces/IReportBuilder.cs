using MigraDoc.DocumentObjectModel;

namespace Reporting.Core.Interfaces
{
    /// <summary>
    /// Converts a report model into a MigraDoc <see cref="Document"/> ready for rendering.
    /// Each report type has its own builder that controls layout, styles, and content.
    /// </summary>
    /// <typeparam name="TModel">The report model this builder accepts.</typeparam>
    public interface IReportBuilder<TModel> where TModel : IReportModel
    {
        /// <summary>
        /// Constructs the complete MigraDoc document for the given model.
        /// </summary>
        /// <param name="model">Populated report model from a data service.</param>
        /// <returns>A fully configured <see cref="Document"/> instance.</returns>
        Document Build(TModel model);
    }
}
