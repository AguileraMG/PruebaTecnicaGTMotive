using System;
using System.Threading;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.ApplicationCore.Repositories;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Abstractions;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Rentals.GetRentalByLicensePlate
{
    /// <summary>
    /// Use case for getting an active rental by vehicle license plate.
    /// </summary>
    public sealed class GetRentalByLicensePlateUseCase(
        IGetRentalByLicensePlateOutputPort outputPort,
        IVehicleRepository vehicleRepository,
        IRentalRepository rentalRepository) : IUseCase<GetRentalByLicensePlateInput>
    {
        private readonly IGetRentalByLicensePlateOutputPort _outputPort = outputPort ?? throw new ArgumentNullException(nameof(outputPort));
        private readonly IVehicleRepository _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
        private readonly IRentalRepository _rentalRepository = rentalRepository ?? throw new ArgumentNullException(nameof(rentalRepository));

        /// <summary>
        /// Executes the use case to get a rental by vehicle license plate.
        /// </summary>
        /// <param name="input">The input containing the license plate.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task ExecuteAsync(GetRentalByLicensePlateInput input, CancellationToken ct)
        {
            ArgumentNullException.ThrowIfNull(input);

            // Validation: Input data
            if (string.IsNullOrWhiteSpace(input.LicensePlate))
            {
                _outputPort.BadRequestHandle("License plate cannot be empty.");
                return;
            }

            // Not Found: Vehicle doesn't exist
            var vehicle = await _vehicleRepository.GetByLicensePlateAsync(input.LicensePlate, ct);
            if (vehicle == null)
            {
                _outputPort.NotFoundHandle($"Vehicle with license plate '{input.LicensePlate}' not found.");
                return;
            }

            // Not Found: No active rental for this vehicle
            var rental = await _rentalRepository.GetActiveRentalByVehicleIdAsync(vehicle.Id, ct);
            if (rental == null)
            {
                _outputPort.NotFoundHandle($"No active rental found for vehicle with license plate '{input.LicensePlate}'.");
                return;
            }

            var output = new GetRentalByLicensePlateOutput
            {
                RentalId = rental.Id,
                VehicleId = vehicle.Id,
                VehicleBrand = vehicle.Brand,
                VehicleModel = vehicle.Model,
                VehicleLicensePlate = vehicle.LicensePlate,
                RentalDate = rental.RentalDate,
                ReturnDate = rental.ReturnDate,
                ExpectedReturnDate = rental.ExpectedReturnDate,
                Status = rental.Status.ToString(),
                Notes = rental.Notes
            };

            _outputPort.StandardHandle(output);
        }
    }
}
