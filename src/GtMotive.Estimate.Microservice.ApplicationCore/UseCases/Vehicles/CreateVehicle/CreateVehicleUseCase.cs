using System;
using System.Threading;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.ApplicationCore.Repositories;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Abstractions;
using GtMotive.Estimate.Microservice.Domain;
using GtMotive.Estimate.Microservice.Domain.Entities;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.CreateVehicle
{
    /// <summary>
    /// Use case for creating a new vehicle in the system.
    /// Implements business logic and validation rules for vehicle registration.
    /// </summary>
    public sealed class CreateVehicleUseCase(
        ICreateVehicleOutputPort outputPort,
        IVehicleRepository vehicleRepository) : IUseCase<CreateVehicleInput>
    {
        private readonly ICreateVehicleOutputPort _outputPort = outputPort ?? throw new ArgumentNullException(nameof(outputPort));
        private readonly IVehicleRepository _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));

        /// <summary>
        /// Executes the create vehicle use case.
        /// </summary>
        /// <param name="input">The input containing vehicle information.</param>
        /// <param name="ct">A token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task ExecuteAsync(CreateVehicleInput input, CancellationToken ct)
        {
            ArgumentNullException.ThrowIfNull(input);

            // Conflict: License plate already exists
            var existingVehicle = await _vehicleRepository.GetByLicensePlateAsync(input.LicensePlate, ct);
            if (existingVehicle != null)
            {
                _outputPort.ConflictHandle($"A vehicle with license plate '{input.LicensePlate}' already exists.");
                return;
            }

            // Create vehicle using factory method (with domain validations)
            Vehicle vehicle;
            try
            {
                vehicle = Vehicle.Create(
                    brand: input.Brand,
                    model: input.Model,
                    year: input.Year,
                    licensePlate: input.LicensePlate,
                    kilometersDriven: input.KilometersDriven);
            }
            catch (DomainException ex)
            {
                _outputPort.BadRequestHandle(ex.Message);
                return;
            }

            await _vehicleRepository.AddAsync(vehicle, ct);

            var output = new CreateVehicleOutput
            {
                Id = vehicle.Id,
                Brand = vehicle.Brand,
                Model = vehicle.Model,
                Year = vehicle.Year,
                LicensePlate = vehicle.LicensePlate,
                KilometersDriven = vehicle.KilometersDriven,
                Status = vehicle.Status.ToString(),
                CreatedAt = vehicle.CreatedAt
            };

            _outputPort.StandardHandle(output);
        }
    }
}
