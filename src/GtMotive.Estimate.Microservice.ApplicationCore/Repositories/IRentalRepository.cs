#nullable enable

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Entities;

namespace GtMotive.Estimate.Microservice.ApplicationCore.Repositories
{
    /// <summary>
    /// Repository interface for managing rentals.
    /// </summary>
    public interface IRentalRepository
    {
        /// <summary>
        /// Adds a new rental to the repository.
        /// </summary>
        /// <param name="rental">The rental to add.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AddAsync(Rental rental, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a rental by its unique identifier.
        /// </summary>
        /// <param name="id">The rental identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The rental if found, null otherwise.</returns>
        Task<Rental?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all active rentals for a specific customer.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of active rentals for the customer.</returns>
        Task<List<Rental>> GetActiveRentalsByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all rentals (active and completed) for a specific customer.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of all rentals for the customer.</returns>
        Task<List<Rental>> GetRentalsByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the active rental for a specific vehicle (if any).
        /// </summary>
        /// <param name="vehicleId">The vehicle identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The active rental for the vehicle, or null if not rented.</returns>
        Task<Rental?> GetActiveRentalByVehicleIdAsync(string vehicleId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all active rentals in the system.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of all active rentals.</returns>
        Task<List<Rental>> GetActiveRentalsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all overdue rentals (active rentals past expected return date).
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of overdue rentals.</returns>
        Task<List<Rental>> GetOverdueRentalsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all rentals in the system filtered by status.
        /// </summary>
        /// <param name="status">Optional status filter. If null, returns all rentals.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of rentals.</returns>
        Task<List<Rental>> GetAllRentalsAsync(RentalStatus? status = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing rental.
        /// </summary>
        /// <param name="rental">The rental to update.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdateAsync(Rental rental, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a rental by its identifier.
        /// </summary>
        /// <param name="id">The rental identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    }
}
