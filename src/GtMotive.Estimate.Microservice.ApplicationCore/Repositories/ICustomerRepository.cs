#nullable enable

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Entities;

namespace GtMotive.Estimate.Microservice.ApplicationCore.Repositories
{
    /// <summary>
    /// Repository interface for managing customers.
    /// </summary>
    public interface ICustomerRepository
    {
        /// <summary>
        /// Adds a new customer to the repository.
        /// </summary>
        /// <param name="customer">The customer to add.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AddAsync(Customer customer, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a customer by its unique identifier.
        /// </summary>
        /// <param name="id">The customer identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The customer if found, null otherwise.</returns>
        Task<Customer?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a customer by email address.
        /// </summary>
        /// <param name="email">The customer email.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The customer if found, null otherwise.</returns>
        Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a customer by driver license number.
        /// </summary>
        /// <param name="driverLicenseNumber">The driver license number.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The customer if found, null otherwise.</returns>
        Task<Customer?> GetByDriverLicenseAsync(string driverLicenseNumber, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all customers in the system with optional filter.
        /// </summary>
        /// <param name="hasActiveRental">Optional filter by active rental status.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of customers.</returns>
        Task<List<Customer>> GetAllAsync(bool? hasActiveRental = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing customer.
        /// </summary>
        /// <param name="customer">The customer to update.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdateAsync(Customer customer, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a customer by its identifier (soft delete).
        /// </summary>
        /// <param name="id">The customer identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    }
}
