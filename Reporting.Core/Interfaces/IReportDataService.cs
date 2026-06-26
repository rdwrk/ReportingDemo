using Reporting.Core.Models;

namespace Reporting.Core.Interfaces
{
    /// <summary>
    /// Retrieves and shapes data into a strongly-typed report model.
    /// Implement one service per report type.
    /// </summary>
    /// <typeparam name="TModel">The report model produced by this service.</typeparam>
    public interface IReportDataService<TModel> where TModel : IReportModel
    {
        /// <summary>
        /// Builds the report model from the supplied request parameters.
        /// </summary>
        /// <param name="request">Date range, filters, and any additional parameters.</param>
        /// <returns>A populated model ready to pass to an <see cref="IReportBuilder{TModel}"/>.</returns>
        TModel GetModel(ReportRequest request);
    }
}
