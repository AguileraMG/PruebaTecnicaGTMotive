namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Abstractions
{
    /// <summary>
    /// Interface to define the Not Found Output Port (HTTP 404).
    /// Use this when a required resource (entity) is not found in the system.
    /// </summary>
    public interface IOutputPortNotFound
    {
        /// <summary>
        /// Informs that a required resource was not found.
        /// This should map to HTTP 404 Not Found response.
        /// </summary>
        /// <param name="message">Description of what resource was not found.</param>
        /// <example>
        /// <para>"Vehicle with ID '123' not found".</para>
        /// <para>"Customer with email 'john@example.com' not found".</para>
        /// </example>
        void NotFoundHandle(string message);
    }
}
