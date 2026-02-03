using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.ApplicationCore.Repositories;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Abstractions;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.GetVehiclesByStatus
{
    /// <summary>
    /// Use case for retrieving vehicles filtered by status.
    /// </summary>
    public sealed class GetVehiclesByStatusUseCase(
        IGetVehiclesByStatusOutputPort outputPort,
        IVehicleRepository vehicleRepository) : IUseCase<GetVehiclesByStatusInput>
    {
        private readonly IGetVehiclesByStatusOutputPort _outputPort = outputPort ?? throw new ArgumentNullException(nameof(outputPort));
        private readonly IVehicleRepository _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));

        /// <summary>
        /// Executes the use case to get vehicles by status.
        /// </summary>
        /// <param name="input">The input containing the status filter.</param>
        /// <param name="ct">A token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task ExecuteAsync(GetVehiclesByStatusInput input, CancellationToken ct)
        {
            ArgumentNullException.ThrowIfNull(input);

            // Get vehicles from repository filtered by status
            var vehicles = await _vehicleRepository.GetVehiclesByStatusAsync(input.Status, ct);
            var vehicleItems = vehicles
       .Select(v => new VehicleItem
       {
           Id = v.Id,
           Brand = v.Brand,
           Model = v.Model,
           Year = v.Year,
           LicensePlate = v.LicensePlate,
           KilometersDriven = v.KilometersDriven,
           Status = v.Status.ToString(),
           CreatedAt = v.CreatedAt
       })
       .ToList();

            // Prepare output
            var output = new GetVehiclesByStatusOutput
            {
                TotalCount = vehicles.Count,
                FilterApplied = input.Status?.ToString() ?? "All",
                Vehicles = new Collection<VehicleItem>(vehicleItems)
            };

            // Notify output port
            _outputPort.StandardHandle(output);
        }
    }
}
