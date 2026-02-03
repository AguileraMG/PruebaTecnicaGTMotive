using System;
using System.Threading;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.ApplicationCore.Repositories;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Abstractions;
using GtMotive.Estimate.Microservice.Domain;
using GtMotive.Estimate.Microservice.Domain.Entities;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Rentals.RentVehicle
{
    /// <summary>
    /// Use case for renting a vehicle to a customer.
    /// Implements the business logic and validation rules for the rental process.
    /// </summary>
    public sealed class RentVehicleUseCase(
        IRentVehicleOutputPort outputPort,
        IVehicleRepository vehicleRepository,
        ICustomerRepository customerRepository,
        IRentalRepository rentalRepository) : IUseCase<RentVehicleInput>
    {
        private readonly IRentVehicleOutputPort _outputPort = outputPort ?? throw new ArgumentNullException(nameof(outputPort));
        private readonly IVehicleRepository _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
        private readonly ICustomerRepository _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
        private readonly IRentalRepository _rentalRepository = rentalRepository ?? throw new ArgumentNullException(nameof(rentalRepository));

        /// <summary>
        /// Executes the rent vehicle use case.
        /// </summary>
        /// <param name="input">The input containing rental information.</param>
        /// <param name="ct">A token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task ExecuteAsync(RentVehicleInput input, CancellationToken ct)
        {
            ArgumentNullException.ThrowIfNull(input);

            // Not Found: Vehicle doesn't exist
            var vehicle = await _vehicleRepository.GetByIdAsync(input.VehicleId, ct);
            if (vehicle == null)
            {
                _outputPort.NotFoundHandle($"Vehicle with ID '{input.VehicleId}' not found.");
                return;
            }

            // Conflict: Vehicle is not available (already rented)
            if (!vehicle.IsAvailable())
            {
                _outputPort.ConflictHandle(
                    $"Vehicle '{vehicle.LicensePlate}' is not available for rent. Current status: {vehicle.Status}");
                return;
            }

            // Bad Request: Vehicle is too old (business rule)
            if (!vehicle.IsEligibleForFleet())
            {
                _outputPort.BadRequestHandle(
                    $"Vehicle '{vehicle.LicensePlate}' is no longer eligible for rent (too old).");
                return;
            }

            // Not Found: Customer doesn't exist
            var customer = await _customerRepository.GetByIdAsync(input.CustomerId, ct);
            if (customer == null)
            {
                _outputPort.NotFoundHandle($"Customer with ID '{input.CustomerId}' not found.");
                return;
            }

            // Conflict: Customer already has active rental (business rule)
            if (!customer.CanRentVehicle())
            {
                _outputPort.ConflictHandle(
                    $"Customer '{customer.Name}' cannot rent a vehicle because he already has an active rental.");
                return;
            }

            // Create rental using factory method (with domain validations)
            Rental rental;
            try
            {
                rental = Rental.Create(
                    vehicleId: vehicle.Id,
                    customerId: customer.Id,
                    expectedReturnDate: input.ExpectedReturnDate,
                    notes: input.Notes);
            }
            catch (DomainException ex)
            {
                _outputPort.BadRequestHandle(ex.Message);
                return;
            }

            try
            {
                vehicle.MarkAsRented();
                await _vehicleRepository.UpdateAsync(vehicle, ct);

                customer.MarkAsRenting();
                await _customerRepository.UpdateAsync(customer, ct);
            }
            catch (DomainException ex)
            {
                _outputPort.ConflictHandle(ex.Message);
                return;
            }

            // Save the rental
            await _rentalRepository.AddAsync(rental, ct);

            // Prepare the output response
            var output = new RentVehicleOutput
            {
                RentalId = rental.Id,
                VehicleId = vehicle.Id,
                VehicleBrand = vehicle.Brand,
                VehicleModel = vehicle.Model,
                VehicleLicensePlate = vehicle.LicensePlate,
                CustomerId = customer.Id,
                CustomerName = customer.Name,
                RentalDate = rental.RentalDate,
                ExpectedReturnDate = rental.ExpectedReturnDate,
                Status = rental.Status.ToString(),
                Notes = rental.Notes
            };

            _outputPort.StandardHandle(output);
        }
    }
}
