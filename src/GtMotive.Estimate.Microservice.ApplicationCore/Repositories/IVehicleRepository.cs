#nullable enable

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Entities;

namespace GtMotive.Estimate.Microservice.ApplicationCore.Repositories
{
    /// <summary>
    /// Repository interface for managing vehicles.
    /// </summary>
    public interface IVehicleRepository
    {
        /// <summary>
        /// Adds a new vehicle to the repository.
        /// </summary>
        /// <param name="vehicle">The vehicle to add.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AddAsync(Vehicle vehicle, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a vehicle by its unique identifier.
        /// </summary>
        /// <param name="id">The vehicle identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The vehicle if found, null otherwise.</returns>
        Task<Vehicle?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a vehicle by its license plate.
        /// </summary>
        /// <param name="licensePlate">The license plate to search for.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The vehicle if found, null otherwise.</returns>
        Task<Vehicle?> GetByLicensePlateAsync(string licensePlate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets vehicles filtered by status.
        /// </summary>
        /// <param name="status">The status to filter by. If null, returns all vehicles.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of vehicles matching the status filter.</returns>
        Task<List<Vehicle>> GetVehiclesByStatusAsync(VehicleStatus? status, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing vehicle.
        /// </summary>
        /// <param name="vehicle">The vehicle to update.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdateAsync(Vehicle vehicle, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a vehicle by its unique identifier.
        /// </summary>
        /// <param name="id">The vehicle identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    }
}
