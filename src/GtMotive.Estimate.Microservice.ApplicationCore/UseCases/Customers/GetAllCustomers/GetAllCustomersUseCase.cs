using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.ApplicationCore.Repositories;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Abstractions;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Customers.GetAllCustomers
{
    /// <summary>
    /// Use case for getting all customers with optional filter.
    /// </summary>
    public sealed class GetAllCustomersUseCase(
        IGetAllCustomersOutputPort outputPort,
        ICustomerRepository customerRepository) : IUseCase<GetAllCustomersInput>
    {
        private readonly IGetAllCustomersOutputPort _outputPort = outputPort ?? throw new ArgumentNullException(nameof(outputPort));
        private readonly ICustomerRepository _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));

        /// <summary>
        /// Executes the use case to get all customers.
        /// </summary>
        /// <param name="input">The input with optional filter.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task ExecuteAsync(GetAllCustomersInput input, CancellationToken ct)
        {
            ArgumentNullException.ThrowIfNull(input);

            var customers = await _customerRepository.GetAllAsync(input.HasActiveRental, ct);

            var customerDtos = customers.Select(v => new CustomerDto
            {
                Id = v.Id,
                Name = v.Name,
                Email = v.Email,
                PhoneNumber = v.PhoneNumber,
                DriverLicenseNumber = v.DriverLicenseNumber,
                HasActiveRental = v.HasActiveRental,
                CreatedAt = v.CreatedAt
            }).ToList();

            var output = new GetAllCustomersOutput
            {
                Customers = new Collection<CustomerDto>(customerDtos),
                TotalCount = customerDtos.Count
            };

            _outputPort.StandardHandle(output);
        }
    }
}
