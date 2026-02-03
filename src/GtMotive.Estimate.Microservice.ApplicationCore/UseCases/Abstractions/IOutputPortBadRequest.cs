namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Abstractions
{
    /// <summary>
    /// Interface to define the Bad Request Output Port (HTTP 400).
    /// Use this when the input data is invalid, empty, or malformed.
    /// </summary>
    public interface IOutputPortBadRequest
    {
        /// <summary>
        /// Informs that the request contains invalid or missing data.
        /// This should map to HTTP 400 Bad Request response.
        /// </summary>
        /// <param name="message">Description of what validation failed.</param>
        /// <example>
        /// <para>"License plate cannot be empty".</para>
        /// <para>"Expected return date must be in the future".</para>
        /// <para>"Kilometers cannot be negative".</para>
        /// </example>
        void BadRequestHandle(string message);
    }
}
