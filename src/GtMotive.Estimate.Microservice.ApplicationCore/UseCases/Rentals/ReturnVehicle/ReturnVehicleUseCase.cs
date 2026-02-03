using System;
using System.Threading;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.ApplicationCore.Repositories;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Abstractions;
using GtMotive.Estimate.Microservice.Domain;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Rentals.ReturnVehicle
{
    /// <summary>
    /// Use case for returning a rented vehicle.
    /// Implements the business logic and validation rules for the return process.
    /// </summary>
    public sealed class ReturnVehicleUseCase(
        IReturnVehicleOutputPort outputPort,
        IVehicleRepository vehicleRepository,
        ICustomerRepository customerRepository,
        IRentalRepository rentalRepository) : IUseCase<ReturnVehicleInput>
    {
        private readonly IReturnVehicleOutputPort _outputPort = outputPort ?? throw new ArgumentNullException(nameof(outputPort));
        private readonly IVehicleRepository _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
        private readonly ICustomerRepository _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
        private readonly IRentalRepository _rentalRepository = rentalRepository ?? throw new ArgumentNullException(nameof(rentalRepository));

        /// <summary>
        /// Executes the use case for returning a rented vehicle.
        /// </summary>
        /// <param name="input">The input data required to process the vehicle return.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task ExecuteAsync(ReturnVehicleInput input, CancellationToken ct)
        {
            ArgumentNullException.ThrowIfNull(input);

            // Not Found: Rental doesn't exist
            var rental = await _rentalRepository.GetByIdAsync(input.RentalId, ct);
            if (rental == null)
            {
                _outputPort.NotFoundHandle($"Rental with ID '{input.RentalId}' not found.");
                return;
            }

            // Conflict: Rental is not active (business rule)
            if (!rental.IsActive())
            {
                _outputPort.ConflictHandle($"Rental with ID '{input.RentalId}' is not active. Current status: {rental.Status}");
                return;
            }

            // Not Found: Vehicle doesn't exist
            var vehicle = await _vehicleRepository.GetByIdAsync(rental.VehicleId, ct);
            if (vehicle == null)
            {
                _outputPort.NotFoundHandle($"Vehicle with ID '{rental.VehicleId}' not found.");
                return;
            }

            // Not Found: Customer doesn't exist
            var customer = await _customerRepository.GetByIdAsync(rental.CustomerId, ct);
            if (customer == null)
            {
                _outputPort.NotFoundHandle($"Customer with ID '{rental.CustomerId}' not found.");
                return;
            }

            // Update kilometers using domain method (with validation)
            try
            {
                vehicle.SetKilometers(input.CurrentKilometers);
            }
            catch (DomainException ex)
            {
                // Bad Request: Odometer validation failed
                _outputPort.BadRequestHandle(ex.Message);
                return;
            }

            // Calculate kilometers driven during this rental
            var kilometersDriven = input.CurrentKilometers - vehicle.KilometersDriven;

            try
            {
                customer.MarkAsNotRenting();
                await _customerRepository.UpdateAsync(customer, ct);

                if (vehicle.IsEligibleForFleet())
                {
                    vehicle.MarkAsAvailable();
                }
                else
                {
                    vehicle.MarkAsRetired();
                }

                await _vehicleRepository.UpdateAsync(vehicle, ct);

                rental.CompleteRental();
                if (!string.IsNullOrWhiteSpace(input.Notes))
                {
                    rental.AddNote(input.Notes);
                }

                await _rentalRepository.UpdateAsync(rental, ct);
            }
            catch (DomainException ex)
            {
                _outputPort.ConflictHandle(ex.Message);
                return;
            }

            var output = new ReturnVehicleOutput
            {
                RentalId = rental.Id,
                VehicleId = vehicle.Id,
                VehicleBrand = vehicle.Brand,
                VehicleModel = vehicle.Model,
                VehicleLicensePlate = vehicle.LicensePlate,
                CustomerId = customer.Id,
                CustomerName = customer.Name,
                RentalDate = rental.RentalDate,
                ReturnDate = rental.ReturnDate!.Value,
                ExpectedReturnDate = rental.ExpectedReturnDate,
                Status = rental.Status.ToString(),
                KilometersDriven = kilometersDriven,
                VehicleStatus = vehicle.Status.ToString(),
                Notes = rental.Notes
            };

            _outputPort.StandardHandle(output);
        }
    }
}
