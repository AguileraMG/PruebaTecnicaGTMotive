using System;
using System.Threading;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.ApplicationCore.Repositories;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Abstractions;
using GtMotive.Estimate.Microservice.Domain;
using GtMotive.Estimate.Microservice.Domain.Entities;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Customers.CreateCustomer
{
    /// <summary>
    /// Use case for creating a new customer in the system.
    /// Implements the business logic and validation rules for customer registration.
    /// </summary>
    public sealed class CreateCustomerUseCase(
        ICreateCustomerOutputPort outputPort,
        ICustomerRepository customerRepository) : IUseCase<CreateCustomerInput>
    {
        private readonly ICreateCustomerOutputPort _outputPort = outputPort ?? throw new ArgumentNullException(nameof(outputPort));
        private readonly ICustomerRepository _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));

        /// <summary>
        /// Executes the create customer use case.
        /// </summary>
        /// <param name="input">The input containing customer information.</param>
        /// <param name="ct">A token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task ExecuteAsync(CreateCustomerInput input, CancellationToken ct)
        {
            ArgumentNullException.ThrowIfNull(input);

            // Conflict: Email already exists
            var existingCustomer = await _customerRepository.GetByEmailAsync(input.Email, ct);
            if (existingCustomer != null)
            {
                _outputPort.ConflictHandle($"A customer with email '{input.Email}' already exists.");
                return;
            }

            // Conflict: Driver license already exists
            var existingByLicense = await _customerRepository.GetByDriverLicenseAsync(input.DriverLicenseNumber, ct);
            if (existingByLicense != null)
            {
                _outputPort.ConflictHandle($"A customer with driver license '{input.DriverLicenseNumber}' already exists.");
                return;
            }

            // Create customer using factory method (with domain validations)
            Customer customer;
            try
            {
                customer = Customer.Create(
                    name: input.Name,
                    email: input.Email,
                    phoneNumber: input.PhoneNumber,
                    driverLicenseNumber: input.DriverLicenseNumber);
            }
            catch (DomainException ex)
            {
                // Domain validation failed - return as BadRequest
                _outputPort.ConflictHandle(ex.Message);
                return;
            }

            await _customerRepository.AddAsync(customer, ct);

            var output = new CreateCustomerOutput
            {
                Id = customer.Id,
                Name = customer.Name,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                DriverLicenseNumber = customer.DriverLicenseNumber,
                HasActiveRental = customer.HasActiveRental,
                CreatedAt = customer.CreatedAt
            };

            _outputPort.StandardHandle(output);
        }
    }
}
