namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Abstractions
{
    /// <summary>
    /// Interface to define the Conflict Output Port (HTTP 409).
    /// Use this when a business rule is violated or there's a conflict with the current state.
    /// </summary>
    public interface IOutputPortConflict
    {
        /// <summary>
        /// Informs that the request conflicts with the current state or business rules.
        /// This should map to HTTP 409 Conflict response.
        /// </summary>
        /// <param name="message">Description of what business rule was violated.</param>
        /// <example>
        /// <para>"Customer with email 'john@example.com' already exists".</para>
        /// <para>"Vehicle '1234-ABC' is not available for rent. Current status: Rented".</para>
        /// <para>"Customer 'John Doe' already has an active rental".</para>
        /// </example>
        void ConflictHandle(string message);
    }
}
